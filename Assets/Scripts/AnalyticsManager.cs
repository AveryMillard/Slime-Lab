using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();

    }

    // Update is called once per frame

    public void SendRemainingTime(float remaining, string name)
    {
            CustomEvent e = new CustomEvent("RemainingTime")
            {
                { "Time", remaining},
                { "PuzzleName", name},
            };
            AnalyticsService.Instance.RecordEvent(e);
         //   Debug.Log("Event recorded");
        
    }
}
