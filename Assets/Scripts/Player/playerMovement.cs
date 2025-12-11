using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

public class playerMovement : MonoBehaviour
{
    private Rigidbody rb;
    public float currentspeed;
    private Collider collider;
    [SerializeField] public bool iceToggle;
    [SerializeField] private bool iceCache;
    private int flag;
    [SerializeField] private Vector3 currentDir = Vector3.zero;
    private Vector3 dir;
    public string entrydir;
    public roomProperties.Direction spawnLocation;
    [SerializeField] bool collisionCheck;
    int firstScene = 0;
    public GameObject lastIceObject;
    public GameObject pauseMenuObject;
    public Vector3 spawnPos;
    float velocityMagnitude = 0;
    float horizontal;
    float vertical;

    // For pausing
    bool isFrozen;

    Transform cameraObject;
    InputManager inputManager;

    [SerializeField] public float rotationSpeed;
    [SerializeField] public float playerSpeed;
    // Start is called before the first frame update
    void Start()
    {
        inputManager = GetComponent<InputManager>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        currentspeed = playerSpeed;
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        iceToggle = false;
        collisionCheck = false;
        iceCache = false;
        entrydir = "North";
        flag = 0;
        
        
        

        pauseMenuObject = GameObject.FindGameObjectWithTag("PauseMenu");
        pauseMenuObject.SetActive(false);
    }


    private void Update()
    {   
        
        if (Input.GetButtonUp("Cancel"))
        {
            pauseMenuObject.SetActive(true);
            FreezePlayer(true);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetButtonDown("Debug Reset"))
        {
            Debug.Log("Debug Pressed");
            PlayerManager.GameOver();
        }
        horizontal = Input.GetAxis("Horizontal"); //A-D buttons
        vertical = Input.GetAxis("Vertical"); //W-S buttons
        

        dir = new Vector3(horizontal, 0, vertical); //Make a vector based on player input

        if (horizontal != 0 && vertical != 0 && iceCache == true)
        {
            dir = Vector3.zero;
        }

        //Debug.Log($"Direction: {dir}, Current Speed: {currentspeed}");
        
        //If ice toggle is off, meaning the player is not on ice, continue at normal speed. Otherwise, the velocit  y should stay as is, without any dir vector (input from player)

        velocityMagnitude = rb.velocity.magnitude;

        if (isFrozen)
        {
            /* do absolutely nothing */
            return;
        }
        else if (!iceToggle)
        {
            // Debug.Log(rb.velocity);
            Vector3 velocity = rb.velocity;
            velocity.x = dir.x * currentspeed;
            velocity.z = dir.z * currentspeed;
            rb.velocity = velocity;
        }
        else
        {
            if (velocityMagnitude <= 0.1f)
            {
                iceToggle = false;
                iceCache = true;
                return;
            }
            float dirMagnitude = currentDir.magnitude;
            rb.velocity = Vector3.Normalize(rb.velocity) * currentspeed / 2;
            /*
            if (dirMagnitude < 0.1)
            {
                currentDir = dir;
                rb.velocity = currentDir * currentspeed * 15f;
            }
            else rb.velocity = currentDir * currentspeed;
            */
        }
        HandleRotation();
    }

    private void HandleRotation()
    {
        // Rotate only if there is movement input
        if (horizontal != 0 || vertical != 0)
        {
            // Calculate target direction based purely on input direction
            Vector3 targetDirection = new Vector3(dir.x, 0, dir.z);

            // Set rotation towards target direction
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)

    {
        // Debug.Log(collision.gameObject.name);
        Renderer renderer = collision.gameObject.GetComponent<Renderer>();
        Material material = null;
        if (renderer != null)
        {
            material = renderer.material;
            // Check the material properties here
        }

        if (collision.gameObject.tag != "IceFloor")
        {
            rb.velocity = Vector3.zero;

            if (iceToggle == true && material != null && (material.name != "Floor_001 (Instance)" && material.name != "wallMaterial (Instance)"))
            {
                iceCache = true;
                iceToggle = false;


            }
            else if (material !=null && (material.name == "Floor_001 (Instance)" || material.name == "wallMaterial (Instance)")) 
            {
                /*
                Collider[] colliders = Physics.OverlapSphere(transform.position, 1.2f);
                Boolean IsThereIce = false;
                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject.CompareTag("IceFloor"))
                    {
                        IsThereIce = true;
                    }
                    // Debug.Log(collider.name);
                }
                �

                if (!IsThereIce) iceToggle = false;
                */
                iceCache = false;
                //Debug.Log("Floor effect");
            }
            //Debug.Log(material.name);
            if (iceCache != true) collisionCheck = true;
            currentDir = dir;
        }
        else
        {
            collisionCheck = false;
            iceToggle = true;
            lastIceObject = collision.gameObject;
        }

    }
    //  trying to debufg walls (unsuccessful)
    private void OnCollisionStay(Collision collision)
    {
        if (iceCache == true && velocityMagnitude>=0.1)
        {
            if (collision.gameObject.tag == "IceFloor")
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, 1.2f);
                Boolean IsThereIce = false;
                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject.CompareTag("IceFloor"))
                    {
                        IsThereIce = true;
                    }
                    // Debug.Log(collider.name);
                }


                if (IsThereIce)
                {
                    iceToggle = true;
                    collisionCheck = false;

                }
                else
                {
                    iceToggle = false;
                    collisionCheck = true;
                }
            }
            else
            {
                iceCache = false;
                iceToggle = false;
            }

            //currentDir = dir;
        }
        if (collision.gameObject.CompareTag("IceFloor")) iceCache = true;

    }

    private void OnCollisionExit(Collision collision)

    {
        collisionCheck = false;

        // Debug.Log(collision.gameObject.name);
        /*
        else if (iceCache == true)
        {
            iceToggle = true;
            iceCache = false;
        }*/

    }
    /*
    private void OnTriggerEnter(Collider collision)

    {
        
        // Debug.Log(collision.gameObject.name);
        if (collision.CompareTag("IceFloor"))
        {
            rb.drag = 0;
            Debug.Log("Ice!");
            iceToggle = true;
            currentDir = dir;
        }
        else{
            collisionCheck = true;
       
 
    }

    private void OnTriggerExit(Collider collision)
    {
        
        if (collision.CompareTag("IceFloor"))
        {
                rb.drag = 1;
                iceToggle = false;
                currentspeed = playerSpeed;
                collisionCheck = false;
        }
        else  currentDir = dir;
    //    collisionCheck = false;
    }
*/
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("ObSceneLoaded");
        GameObject roomPrefab = GameObject.FindGameObjectWithTag("Room");
        roomProperties properties = roomPrefab.GetComponent<roomProperties>();
        GameObject lvlManager = GameObject.FindGameObjectWithTag("LevelGen");
        LevelGenManager lvlManagerScript = lvlManager.GetComponent<LevelGenManager>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerManager playermanager = player.GetComponent<PlayerManager>();
        entrydir = player.GetComponent<playerMovement>().entrydir;

        Room curRoom = lvlManagerScript.getRoom(lvlManagerScript.rooms, playermanager.playerCoords);


        properties.rotateDoors((int)curRoom.roomDir);

        if (curRoom.flip != null)
        {
            //Debug.Log("Room flip is not null, it is " + curRoom.flip);
            properties.flipRoom(curRoom.flip);
        }

        List<GameObject> doors = GameObject.FindGameObjectsWithTag("Door").ToList();
        GameObject entryDoor = null;
        foreach (GameObject door in doors)
        {
            doorLogic logic = door.GetComponent<doorLogic>();
            //Debug.Log(logic.Direction.ToString() + " " + entrydir + doors.Count());
            if (logic.Direction.ToString() == entrydir)
            {
                entryDoor = door;
                break;
            }
        }

        // Position the player slightly in front of the door


        player.transform.position = entryDoor.transform.position + (entryDoor.transform.forward * 2);
        //Debug.Log(entryDoor.transform.position);
        //Debug.Log(player.transform.position);
        spawnPos = player.transform.position;


    }

    public void FreezePlayer(bool freeze)
    {
        isFrozen = freeze;
    }
}
