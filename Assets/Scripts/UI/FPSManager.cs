using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FPSManager : MonoBehaviour
{
    public float framesAffectingAverage = 90;

    private Text text;
    private float FPS = 60.0f;
    private float averageFPS = 0.0f;
    private int dataPoints = 0;

    private void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        
        if (dataPoints < framesAffectingAverage)
        {
            dataPoints++;
        }
        else
        {
            FPS = 1 / Time.deltaTime;
            averageFPS = ((averageFPS * dataPoints + FPS) / (dataPoints + 1));
            text.text = "FPS: " + (int)(averageFPS);
        }

    }
}
