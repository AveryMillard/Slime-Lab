using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterStarter : MonoBehaviour
{
    // Start is called before the first frame update

    Boss bossObj;
    BossRoomMechanics bossRoomObj;
    doorLogic door;
    public GameObject Entrance;
    void Start()
    {
        door = GameObject.Find("Doorway_002").GetComponent<doorLogic>();
        Entrance = GameObject.Find("Entrance");
        bossObj = GameObject.Find("Boss").GetComponent<Boss>();
        bossRoomObj = GameObject.Find("Boss Room").GetComponent<BossRoomMechanics>();

    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();
        if (player != null)
        {
            GetComponent<Collider>().enabled = false;
            Debug.Log("player entered");
            bossObj.active = true;
            door.doorToggle();
            Debug.Log("door locked");
        }
        bossRoomObj.BeginEncounter();
        Entrance.SetActive(false);

    }

    public void reenable()
    {
        GetComponent<Collider>().enabled = true;
        door.doorToggle();
    }

}
