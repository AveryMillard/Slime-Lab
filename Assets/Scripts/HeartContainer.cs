using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartContainer : MonoBehaviour
{

    public AudioSource collectSound;
    LevelGenManager lvlManagerScript;
    PlayerManager playermanager;
    Map map;
    GameObject mapObj;
    Room curRoom;
    private void Start()
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        GameObject lvlManager = GameObject.FindGameObjectWithTag("LevelGen");
        lvlManagerScript = lvlManager.GetComponent<LevelGenManager>();
        playermanager = Player.GetComponent<PlayerManager>();
        mapObj = GameObject.Find("miniMapRaw");
        map = mapObj.GetComponent<Map>();

        curRoom = lvlManagerScript.getRoom(lvlManagerScript.rooms, playermanager.playerCoords);
        foreach(Room room in lvlManagerScript.roomsWithHealth)
        {
            //Debug.Log(room.Coordinates);
        }
        //Debug.Log(curRoom.Coordinates + "coords");
        if (!lvlManagerScript.roomsWithHealth.Contains(curRoom))
        {
            gameObject.SetActive(false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();
        if (player != null)
        {
            player.GetHeartContainer();
            lvlManagerScript.roomsWithHealth.Remove(curRoom);
            map.ReloadHealthContainer(player.playerCoords);
            //gameObject.SetActive(false);
            StartCoroutine(DeactivateAfterSound());
        }
    }
    private IEnumerator DeactivateAfterSound()
    {
        collectSound.Play();
        //Debug.Log(collectSound.name + " played");
        yield return new WaitForSeconds(collectSound.clip.length / 2);
        gameObject.SetActive(false);
    }
}
