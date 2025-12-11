using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerLocomotion : MonoBehaviour
{

    InputManager inputManager;

    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody playerRigidbody;

    //player movement variables
    public float playerSpeed = 7;
    public float currentspeed;
    public float rotationSpeed = 15;


    //room spawning
    int firstScene = 0;
    public string entrydir;
    public Vector3 spawnPos;
    
    private Collider playerCollider;

    //happens before Start()
    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        cameraObject = Camera.main.transform;
    }


    private void Start()
    {
        //SceneManager.sceneLoaded += OnSceneLoaded;
        currentspeed = playerSpeed;
        entrydir = "North";
    }


    public void HandleAllMovement()
    { 
            HandleMovement();
            HandleRotation();
    }
 
    
    private void HandleMovement()
    {
        moveDirection = new Vector3(cameraObject.forward.x, 0f, cameraObject.forward.z) * inputManager.verticalInput;
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;
        moveDirection = moveDirection * currentspeed;

        Vector3 movementVelocity = moveDirection;
        playerRigidbody.velocity = movementVelocity;
    }


    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        //keeps rotation of player when not moving
        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (firstScene == 1)
        {
            GameObject roomPrefab = GameObject.FindGameObjectWithTag("Room");
            roomProperties properties = roomPrefab.GetComponent<roomProperties>();
            GameObject lvlManager = GameObject.FindGameObjectWithTag("LevelGen");
            LevelGenManager lvlManagerScript = lvlManager.GetComponent<LevelGenManager>();
            PlayerManager playermanager = gameObject.GetComponent<PlayerManager>();
            Room curRoom = lvlManagerScript.getRoom(lvlManagerScript.rooms, playermanager.playerCoords);

            Debug.Log(roomPrefab.gameObject.name);
            Debug.Log(properties.roomName);

            properties.rotateDoors((int)curRoom.roomDir);

            if (curRoom.flip != null)
            {
                properties.flipRoom(curRoom.flip);
            }

            List<GameObject> doors = GameObject.FindGameObjectsWithTag("Door").ToList();
            GameObject entryDoor = null;
            foreach (GameObject door in doors)
            {
                doorLogic logic = door.GetComponent<doorLogic>();
                //Debug.Log(logic.Direction.ToString() + " " +  entrydir + doors.Count());
                if (logic.Direction.ToString() == entrydir)
                {
                    //Debug.Log("Found entry dir");
                    entryDoor = door;
                    break;
                }
            }

            // Position the player slightly in front of the door

            transform.position = entryDoor.transform.position + (entryDoor.transform.up * 2) + new Vector3(0, 2, 0);
            //Debug.Log(entryDoor.transform.position);
            //Debug.Log(entryDoor.transform.up);
            spawnPos = transform.position;
        }

        firstScene = 1;

    }
}
