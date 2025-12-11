using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Threading;
using System;

public class RoomChangerLogic : MonoBehaviour
{
    private Collider collider;
    private GameObject door;
    private GameObject player;
    private playerMovement playerManager;
    private string direction;
    private GameObject levelGen;
    private LevelGenManager levelGenManager;
    private Map mapRef;

    void Start()
    {
        mapRef = GameObject.Find("miniMapRaw").GetComponent<Map>();
        collider = GetComponent<Collider>();
        door = transform.parent.gameObject;
        direction = door.GetComponent<doorLogic>().Direction.ToString();
        player = GameObject.FindWithTag("Player");
        levelGen = GameObject.FindWithTag("LevelGen");
        levelGenManager = levelGen.GetComponent<LevelGenManager>();
        //changed <PlayerMovement> -> <PlayerLocomotion>
        playerManager = player.GetComponent<playerMovement>();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            player = collision.gameObject;
            Vector2 coordinateChange = Vector2.zero;

            switch (direction)
            {
                case "North": coordinateChange = new Vector2(0, 1); player.GetComponent<playerMovement>().entrydir = "South"; break;
                case "South": coordinateChange = new Vector2(0, -1); player.GetComponent<playerMovement>().entrydir = "North"; break;
                case "East": coordinateChange = new Vector2(1, 0); player.GetComponent<playerMovement>().entrydir = "West"; break;
                case "West": coordinateChange = new Vector2(-1, 0); player.GetComponent<playerMovement>().entrydir = "East"; break;

            }
            Debug.Log("Player entry dir is now: " + player.GetComponent<playerMovement>().entrydir);

            Vector2 currentPlayerCoords = player.GetComponent<PlayerManager>().playerCoords;
            player.GetComponent<PlayerManager>().playerCoords = currentPlayerCoords + coordinateChange;
            Vector2 newCoords = player.GetComponent<PlayerManager>().playerCoords;

          // Debug.Log(levelGenManager.roomExists(levelGenManager.rooms, newCoords) + " " + newCoords);
            Room nextRoom = levelGenManager.getRoom(levelGenManager.rooms, newCoords);

            GameObject Timer = GameObject.Find("Timer");
            Timer timerScript = Timer.GetComponent<Timer>();

            Room curRoom = levelGenManager.getRoom(levelGenManager.rooms, currentPlayerCoords);

            if(curRoom.selectedSceneData.roomtheme == SceneData.roomTheme.TimedPuzzle)
            {
                timerScript.PauseTimer();
                timerScript.LeaveTimedRoom(curRoom.selectedScene);
            }

            mapRef.playerMovement(coordinateChange.x, coordinateChange.y);


            

            SceneManager.LoadScene(nextRoom.selectedScene);
        }
    }
}
