using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerTextMinSec; // Second timer text for minutes and seconds
    private float startTime;
    private bool isEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnded)
        {
            float t = Time.time - startTime;

            // For the second timer, formatted as '5m 24.34 sec'
            string minutesMinSec = ((int)t / 60).ToString() + "m ";
            string secondsMinSec = (t % 60).ToString("f2") + "s";
            timerTextMinSec.text = minutesMinSec + secondsMinSec;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "GameEnd")
        {
            isEnded = true;
        }
    }
}
