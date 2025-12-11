using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourRoomMechanics : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerManager pManagerRef;

    Timer timerRef;
    Tip tipRef;
    GameObject timerObjectRef;
    GameObject playerObjectRef;
    GameObject tipObjectRef;
    void Start()
    {
       
        timerObjectRef = GameObject.Find("Timer");
        playerObjectRef = GameObject.Find("Player");
        tipObjectRef=GameObject.Find("Tip");
        timerRef = timerObjectRef.GetComponent<Timer>();
        tipRef = tipObjectRef.GetComponent<Tip>();
        pManagerRef = playerObjectRef.GetComponent<PlayerManager>();
        
        timerRef.PrepareStartTimedRoom(30);
    }

    // Update is called once per frame
    void Update()
    {
   //     pManagerRef = playerObjectRef.GetComponent<PlayerManager>();

        if (playerObjectRef.transform.position.y < -2 
            || timerRef.getTime() <= 0)
        {
            pManagerRef.TakeLives(1);
            pManagerRef.Respawn();
            tipRef.setText("Tip: The timer will not start until you press a key.");
            timerRef.PrepareStartTimedRoom(30);
        }
    }
}
