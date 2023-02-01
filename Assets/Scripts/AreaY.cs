using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AreaY : MonoBehaviour
{
    public AgentY collector;
    public GameObject RedContainer;
    public GameObject BlueContainer;
    public GameObject BlueSphere;
    public GameObject RedSphere;

    public TextMeshPro RewardText;
    public TextMeshPro TimeText;
    public TextMeshPro AvgRewardText;
    public TextMeshPro AvgTimeText;
    public TextMeshPro EpisodeTitle;

    private int bluenum =0;
    private int rednum =0;

    private float Timer;
    private int EpisodeNum = 0;
    private float TotalEpisodesTime = 0f;
    private float StartTime;
    private float ElapsedTime;
    public float TotalEpisodesReward;
    private float avgtime;
    private float avgreward;

    public List<GameObject> sphereList;

    public void ResetArea()
    {
        if (EpisodeNum != 0) CalculateAverages();
        
        EpisodeTitle.text = "Episode: " + (1 + EpisodeNum).ToString("0");
        EpisodeNum++;
        Timer = 0f;
        
        

        RemoveAllSpheres();
        PlaceCollector();
        PlaceContainers();
        SpawnSpheres(rednum, bluenum);
        //SpawnSpheres(4, 4);





    }

    public void RemoveSpecificSphere(GameObject sphereObject)
    {
        sphereList.Remove(sphereObject);
        Destroy(sphereObject);
    }

    public int spheresRemaining
    {
        get {return sphereList.Count; }
    }

    public static Vector3 ChooseRandomPosition(Vector3 center, float minAngle, float maxAngle, float minRadius, float maxRadius)
    {
        float angle = minAngle;
        float radius = minRadius;

        if(maxAngle > minAngle)
        {
            angle = UnityEngine.Random.Range(minAngle, maxAngle);
        }

        if(maxRadius > minRadius)
        {
            radius = UnityEngine.Random.Range(minRadius, maxRadius);
        }

        return center + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;

    }

    private void RemoveAllSpheres()
    {
        if(sphereList != null)
        {
            for (int i = 0; i < sphereList.Count; i++)
            {
                if (sphereList[i] != null)
                {
                    Destroy(sphereList[i]);
                }
            }
        }

        sphereList = new List<GameObject>();
    }


    private void PlaceCollector()
    {
        Rigidbody rigidbody = collector.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        collector.transform.position = ChooseRandomPosition(transform.position, 0f, 360f, 0f, 3f) + Vector3.up * 2f;
        collector.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
    }

    private void PlaceContainers()
    {
        Rigidbody rigidbodyRed= RedContainer.GetComponent<Rigidbody>();
        Rigidbody rigidbodyBlue= BlueContainer.GetComponent<Rigidbody>();

        rigidbodyRed.velocity = Vector3.zero;
        rigidbodyRed.angularVelocity = Vector3.zero;
        rigidbodyBlue.velocity = Vector3.zero;
        rigidbodyBlue.angularVelocity = Vector3.zero;

        rigidbodyRed.transform.position = ChooseRandomPosition(transform.position, -45f, 45f, 0f, 3f) + Vector3.up * .5f;
        rigidbodyRed.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        rigidbodyBlue.transform.position = ChooseRandomPosition(transform.position, -45f, 45f, 0f, 3f) + Vector3.up * .5f;
        rigidbodyBlue.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    private void SpawnSpheres(int countRed, int countBlue)
    {
        for (int i = 0; i< countRed; i++)
        {
            Vector3 center = Vector3.zero;
            GameObject red = Instantiate<GameObject>(RedSphere.gameObject);
            red.transform.position = ChooseRandomPosition(transform.position, 100f, 260f, -5f, 0f) + Vector3.up * .5f;
            red.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);

            red.transform.SetParent(transform);
            sphereList.Add(red);
        }

        for (int i = 0; i< countBlue; i++)
        {
            Vector3 center = Vector3.zero;
            GameObject blue = Instantiate<GameObject>(BlueSphere.gameObject);
            blue.transform.position = ChooseRandomPosition(transform.position, 100f, 260f, -5f, 0f) + Vector3.up * .5f;
            blue.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);

            blue.transform.SetParent(transform);
            sphereList.Add(blue);
        }


    }

    private void Start()
    {
        Debug.Log("AreaY started!");
        StartTime = Time.time;
        //ResetArea();
    }

    private void Update()
    {

        RewardText.text = collector.GetCumulativeReward().ToString("0.00");

        Timer += Time.deltaTime;
        //TotalEpisodesTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(Timer / 60F);
        int seconds = Mathf.FloorToInt(Timer % 60F);
        int milliseconds = Mathf.FloorToInt((Timer * 100F) % 100F);
        TimeText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + milliseconds.ToString("00");

        


    }

    public void CalculateAverages()
    {
        ElapsedTime = Time.time - StartTime;
        avgtime = (ElapsedTime / EpisodeNum);
        
        avgreward = (TotalEpisodesReward / EpisodeNum);
        
        AvgRewardText.text = avgreward.ToString("0.00");

        int minutes = Mathf.FloorToInt(avgtime / 60F);
        int seconds = Mathf.FloorToInt(avgtime % 60F);
        int milliseconds = Mathf.FloorToInt((avgtime * 100F) % 100F);

        AvgTimeText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + milliseconds.ToString("00");
    }

   public void SetNumberOfSphere(int red, int blue)
    {
        bluenum = blue;
        rednum = red;
        Debug.Log("elaba b=" + blue + " and red = " + rednum);
        gameObject.SetActive(true);
    }
   /* 
    public void testcall()
    {
        Debug.Log("kalesma komple");
    }
   */
}
