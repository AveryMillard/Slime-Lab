using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthRoomMechanics : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerManager pManagerRef;

    Timer timerRef;
    Tip tipRef;
    GameObject timerObjectRef;
    GameObject playerObjectRef;
    GameObject tipObjectRef;
    [SerializeField] int time = 30;
    void Start()
    {

        timerObjectRef = GameObject.Find("Timer");
        playerObjectRef = GameObject.Find("Player");
        tipObjectRef = GameObject.Find("Tip");
        timerRef = timerObjectRef.GetComponent<Timer>();
        tipRef = tipObjectRef.GetComponent<Tip>();
        pManagerRef = playerObjectRef.GetComponent<PlayerManager>();

        timerRef.PrepareStartTimedRoom(time);
    }

    // Update is called once per frame

    void Update()
    {
        //     pManagerRef = playerObjectRef.GetComponent<PlayerManager>();

        if (playerObjectRef.transform.position.y < -2
            || timerRef.getTime() <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        pManagerRef.TakeLives(1);
        pManagerRef.Respawn();

        //Location of enemies should reset here // TO DO

        tipRef.setText("Tip: The timer will not start until you press a key.");
        timerRef.PrepareStartTimedRoom(time);
    }

}
