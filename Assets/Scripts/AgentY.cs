using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
public class AgentY : Agent
{
    public float moveSpeed = 5f;
    public float turnSpeed = 180f;
    private AreaY collectorarea;

    new private Rigidbody rigidbody;
    //public Animator animator;
    private Renderer rend;
    private GameObject redCon;
    private GameObject blueCon;

    private bool hasSphere;
    private bool hasRed;
    private bool hasBlue;

    
    //private Color ydefault = new Color(25f, 108, 133);
    private Color ydefault = new Color(0.09f, 0.42f, 0.52f, 1f);

    public override void Initialize()
    {
        Debug.Log("AgentY started!");
        base.Initialize();
        collectorarea = GetComponentInParent<AreaY>();
        redCon = collectorarea.RedContainer;
        blueCon = collectorarea.BlueContainer;
        rigidbody = GetComponent<Rigidbody>();

        rend = transform.Find("Alpha_Surface").GetComponent<Renderer>();
        //rend = GetComponent GetComponent<Renderer>();
        //animator.SetFloat("Speed", 1f);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Μετατροπή της πρώτης ενέργειας σε κίνηση εμπρός
        float forwardAmount = actionBuffers.DiscreteActions[0];

        // Μετατροπή της δεύτερης ενέργειας σε κίνηση αριστερά ή δεξιά
        float turnAmount = 0f;
        if (actionBuffers.DiscreteActions[1] == 1f)
        {
            turnAmount = -1f;
        }
        else if (actionBuffers.DiscreteActions[1] == 2f)
        {
            turnAmount = 1f;
        }

        // Εφαρμογή της κίνησης
        rigidbody.MovePosition(transform.position + transform.forward * forwardAmount * moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAmount * turnSpeed * Time.fixedDeltaTime);

        // Εφαρμογή μίας μικρής αρνητικής ανταμοιβής σε κάθε βήμα
        if (MaxStep > 0) AddReward(-1f / MaxStep);
    }

    /// <summary>
    /// Ανάγνωση εισόδων από πληκτρολόγιο και μετατροπή σε μία λίστα ενεργειών.
    /// Αυτό καλείται μόνο όταν ο παίκτης ελέγχει τον πράκτορα και έχει ορίσει 
    /// το Behavior Type σε «Heuristic Only» στο component του  Behavior Parameters.
    /// </summary>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int forwardAction = 0;
        int turnAction = 0;
        if (Input.GetKey(KeyCode.W))
        {
            // κίνηση εμπρός
            forwardAction = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            // κίνηση αριστερά
            turnAction = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // κίνηση δεξιά
            turnAction = 2;
        }

        // Μεταφορά των ενεργειών στον πίνακα
        actionsOut.DiscreteActions.Array[0] = forwardAction;
        actionsOut.DiscreteActions.Array[1] = turnAction;
    }

    /// <summary>
    /// Όταν ξεκινήσει ένα νέο επεισόδιο, επαναφέρει τον πράκτορα και την περιοχή
    /// </summary>
    public override void OnEpisodeBegin()
    {
        hasSphere = false;
        hasRed = false;
        hasBlue = false;

        collectorarea.ResetArea();
    }


    public override void CollectObservations(VectorSensor sensor)
    {

        sensor.AddObservation(hasSphere);
        sensor.AddObservation(hasRed);
        sensor.AddObservation(hasBlue);

        // Απόσταση μέχρι τον κόκκινο κάδο
        sensor.AddObservation(Vector3.Distance(redCon.transform.position, transform.position));
        // Απόσταση μέχρι τον μπλε κάδο
        sensor.AddObservation(Vector3.Distance(blueCon.transform.position, transform.position));

        // Κατεύθυνση προς τον κόκκινο κάδο
        sensor.AddObservation((redCon.transform.position - transform.position).normalized);


        // Κατεύθυνση προς τον μπλε κάδο
        sensor.AddObservation((blueCon.transform.position - transform.position).normalized);

        // Κατεύθυνση που κοιτάει ο πράκτορας - συλλέκτης
        sensor.AddObservation(transform.forward);

        // Σύνολο 1 + 1 + 1 + 3 + 3 + 3 + 3 + 3 = 18 παρατηρήσεις
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("RedSphere") | collision.transform.CompareTag("BlueSphere"))
        {
            // Δοκιμάζουμε να πάρουμε τη σφαίρα
            pickupSphere(collision.gameObject);
        }
        else if (collision.transform.CompareTag("RedContainer") | collision.transform.CompareTag("BlueContainer"))
        {
            // Δοκιμάζουμε να αφήσουμε τη σφαίρα

            dropSphere(collision.gameObject);
        }
    }

    private void pickupSphere(GameObject collectorObject)
    {
        if (hasSphere) return;
        hasSphere = true;
        if (collectorObject.tag == "RedSphere")
        {
            hasRed = true;
            rend.material.color = Color.red;
        }
        else
        {
            hasBlue = true;
            rend.material.color = Color.blue;
        }
        collectorarea.RemoveSpecificSphere(collectorObject);
        AddReward(1f);
    }

    private void dropSphere(GameObject containerObject)
    {
        if (!hasSphere) return;

        if ((hasRed & containerObject.tag == "BlueContainer") | (hasBlue & containerObject.tag == "RedContainer")) return;

        AddReward(1f);
        hasSphere = false;
        hasBlue = false;
        hasRed = false;
        rend.material.color = ydefault;

        if (collectorarea.spheresRemaining <= 0)
        {
            collectorarea.TotalEpisodesReward += GetCumulativeReward();
            EndEpisode();
        }
    }

}
