using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeRemaining;
    public bool timeisRunning = true;
    public bool paused;
    public float initialTime;
    float newTimetoDisplay;
    public TMP_Text timeText;
    GameObject analyticsObjectRef;
    AnalyticsManager analyticsRef;
    // Start is called before the first frame update

    void Start()
    {
        analyticsObjectRef = GameObject.Find("AnalyticsManager");
        analyticsRef = analyticsObjectRef.GetComponent<AnalyticsManager>();
        timeisRunning = false;
        paused = false;
        initialTime = 181;
        timeText.enabled = false;
    }

    public void PrepareStartTimedRoom(float time)
    {
        timeText.enabled = true;
        initialTime = time + 1;
        timeisRunning = false;
        paused = false;
        timeRemaining = 0;
        newTimetoDisplay = initialTime;
        DisplayTime(initialTime);
        timeText.color = Color.white;
    }

    public void PauseTimer()
    {
        timeisRunning = false;
        paused = true;
        timeText.color = Color.red;
    }

    public void UnpauseTimer()
    {
        timeisRunning = true;
        paused = false;
        timeText.color = Color.white;
    }

    public void LeaveTimedRoom(string RoomName)
    {
        timeText.enabled = false;
        timeisRunning = false;
        analyticsRef.SendRemainingTime(newTimetoDisplay, RoomName);
        Debug.Log("Sent time: " + newTimetoDisplay);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (timeisRunning)
        {
            if (timeRemaining >= 0)
            {
                timeRemaining += Time.deltaTime;
                newTimetoDisplay = initialTime - timeRemaining;
                DisplayTime(newTimetoDisplay);
            }
            return;
        }

        if (Input.anyKey && !paused)
        {
            timeisRunning = true;
        }
     
    }
    void DisplayTime (float newTimeToDisplay)
    {
        float minutes = Mathf.FloorToInt(newTimetoDisplay / 60);
        float seconds = Mathf.FloorToInt(newTimetoDisplay % 60);
        timeText.text = string.Format("{0:0} : {1:00}", minutes, seconds);
    }

    public float getTime() { return newTimetoDisplay; }

}
