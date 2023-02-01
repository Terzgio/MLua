using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Preloader : MonoBehaviour
{
    public AreaY testingarea;

    public GameObject canvas;
    public TMP_InputField redinput;
    public TMP_InputField blueinput;
    
    private int rednum;
    private int bluenum;
    
   
    public void StartTest()
    {
        Debug.Log("Button clicked");

        rednum = Convert.ToInt32(redinput.text);
        bluenum = Convert.ToInt32(blueinput.text);

        testingarea.SetNumberOfSphere(rednum, bluenum);
        //testingarea.SetActive(true);
        canvas.SetActive(false);
        
        
    }
}
