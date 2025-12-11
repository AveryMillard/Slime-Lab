using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossRoomMechanics : MonoBehaviour
{
    Timer timerRef;
    Tip tipRef;
    PlayerManager pManRef;
    bool isTimeSet = false;
    GameObject timerObjectRef;
    GameObject tipObjectRef;
    GameObject pObjectRef;
    EncounterStarter EncRef;
    // Start is called before the first frame update
    void Start()
    {
        EncRef = GameObject.Find("BossFightStartCollider").GetComponent<EncounterStarter>();
        timerObjectRef = GameObject.Find("Timer");
        tipObjectRef = GameObject.Find("Tip");
        pObjectRef = GameObject.Find("Player");


        if (timerObjectRef != null)
        {
            timerRef = timerObjectRef.GetComponent<Timer>();
        }



        if (tipObjectRef != null)
        {
            tipRef = tipObjectRef.GetComponent<Tip>();
            if (pObjectRef != null)
            {
                pManRef = pObjectRef.GetComponent<PlayerManager>();
                tipRef.setText("Will you be prepared, little one?");
            }
        }

       


    }

    void PlayerDies()
    {
        EncRef.Entrance.SetActive(true);
        pManRef.TakeLives(1);
        pManRef.Respawn();
        GameObject.Find("Doorway_002").GetComponent<doorLogic>().doorToggle();
    }

    public void ResetEncounter()
    {
        //Potentially more stuff
        tipRef.setText("Was that all you had?");
        GameObject.Find("Boss").GetComponent<Boss>().ResetEncounter();
        timerRef.PauseTimer();
        GameObject.Find("BossFightStartCollider").GetComponent<EncounterStarter>().reenable();
    }

    public void BeginEncounter()
    {
        if (timerObjectRef != null)
        {
            GameObject.Find("Doorway_002").GetComponent<doorLogic>().doorToggle();
            GameObject.Find("Boss").GetComponent<Boss>().StartCoroutine("SpellQueue");
            timerRef = timerObjectRef.GetComponent<Timer>();
            timerRef.PrepareStartTimedRoom(90f);
            isTimeSet = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isTimeSet = true && timerRef.getTime() <= 0)
        {
            Debug.Log(timerRef.getTime());
            timerRef.LeaveTimedRoom("Boss Room");
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                Destroy(obj);
            }
            SceneManager.LoadScene("WinScene");
        }

        if (pObjectRef.transform.position.y < -2 || pObjectRef.transform.position.y > 20f)
        {
            PlayerDies();
            ResetEncounter();
        }

    }
}
