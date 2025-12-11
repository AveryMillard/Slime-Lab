using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenManager : MonoBehaviour
{
    Room spawn, starterRoom;
    [SerializeField] int levelSize;
    private Queue<Door> doorQueue = new Queue<Door>();
    private List<string> directions = new List<string> { "North", "East", "South", "West" };
    private int maxDim;
    public  List<List<Room>> rooms;
    private List<Door> failedDoors = new List<Door>();
    public List<SceneData> sceneDataList = new List<SceneData>();
    public Vector2 curCoordinates;
    private Vector2 newCoords;
    [SerializeField] public bool generate = true;
    private int failsafe;
    int offSet = 0;
    public static LevelGenManager _instance;
    [SerializeField] int seed = -1;

    public List<Vector2> collectibleCoords = new List<Vector2>();
    public List<Room> roomsWithHealth = new List<Room>();
    public static LevelGenManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake() //Runs before Start(), needed to send data to playerManager
    {
        curCoordinates = new Vector2(0, 1);


        sceneDataList = Resources.LoadAll<SceneData>("SceneData").ToList();

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (seed == -1)
        {
            seed = (int)DateTime.Now.Ticks;
        }
        
        UnityEngine.Random.InitState(seed);
    }


    private void Start()
    {
        if (!generate)
        {
            return;
        }

        int beginningRoom = 1;
        int debugFlag = 0;
        Room newRoom;
        string entryDir = "";
        failsafe = 0;
        //Initializes the 2d plane of empty rooms
        //Used later to check for if a room exists to avoid duplication
        //Debug.Log(maxDim + "; " + levelSize);
        maxDim = levelSize * 2 + 1;

        rooms = new List<List<Room>>(maxDim);
        for (int i = 0; i < maxDim; i++)
        {
            List<Room> roomRow = new List<Room>(maxDim);
            for (int j = 0; j < maxDim; j++)
            {
                roomRow.Add(null);
            }
            rooms.Add(roomRow);
        }

        //Debug.Log(rooms.Count);
        offSet = maxDim / 2;
        starterRoom = new Room("starter");
        curCoordinates = starterRoom.Coordinates;
        rooms[(int)curCoordinates.x + offSet][(int)curCoordinates.y + offSet] = starterRoom;
        //Manager should now add these 3 newly added slots for new rooms into a queue

        foreach (Door door in starterRoom.Doors)
        {
            doorQueue.Enqueue(door);
        }

        //Run through said queue, do a coin flip, if heads, continue, if tails, go to next in queue
        int temp = levelSize;
        while (temp > 0)
        {
            string queue = "";

            foreach (Door door in doorQueue)
            {
                queue += door.doorCoords.ToString() + " " + door.direction + " \n";
            }
            //Debug.Log(queue);
            queue = "";

            //Debug.Log(doorQueue.Count());
            if (UnityEngine.Random.Range(0, 2) == 1)
            {
                //Logic to see if the next room that will be added already exists

                //Check if this door leads to a room that exists 


                //Debug.Log("A room could be added here!");

                //We have confirmed a room could be added, minus 1 from the size
                temp--;
                //Determine how many doors this new room will have (1 = deadend, 2 = one entry, 3 = two entries, 4 = 3 entries) 
                //Assuming levelSize isnt restricting the randomizer
                int maxDoors = temp < 4 ? temp : 4; //This line needs to account for negatives
                int newDoors = UnityEngine.Random.Range(1, maxDoors + 1);

                if (newDoors == 1 && temp > 1 && doorQueue.Count() == 1)
                    newDoors = UnityEngine.Random.Range(2, maxDoors + 1); //Rerolling if we roll a dead end with more level size left.

                //Determine what coordinate this room will be located based on door direction
                Door curDoor = doorQueue.Peek();
                newCoords = curDoor.doorCoords;
                string doorDirection = curDoor.direction;


                switch (doorDirection)
                {
                    case "North":
                        {
                            entryDir = "South";
                            newCoords.y++;
                            break;
                        }
                    case "South":
                        {
                            entryDir = "North";
                            newCoords.y--;
                            break;
                        }
                    case "East":
                        {
                            entryDir = "West";
                            newCoords.x++;
                            break;
                        }
                    case "West":
                        {
                            entryDir = "East";
                            newCoords.x--;
                            break;
                        }
                    default:
                        {
                            entryDir = "North";
                            newCoords.y++;
                            break;
                        }
                }

                //Check here for existing coords


                //Debug.Log((int)newCoords.x +" " + offSet +  " " + (int)newCoords.y + " " + offSet);
                if (rooms[(int)newCoords.x + offSet][(int)newCoords.y + offSet] != null)
                {
                    //Debug.Log("This room exists!!!");
                    temp++;
                    failsafe++;
                    if (failsafe > 100)
                    {
                        Debug.Log("Failsafe triggered");
                        break;
                    }

                    //Theres more complex ways to deal with this, but for simplicity sake, disalow the door to exist
                    curDoor = doorQueue.Dequeue();

                    //We also need to do the failed doors check here

                    if (doorQueue.Count == 0 && temp > 0)
                    {
                        //Getting in here means we are about to run out of doors without completing the level size requirements, so we skip the dequeue to force-add a door


                        //Add all previously blocked doors to the queue
                        //Debug.Log("Attemtping to add failedDoors back (inside roomexists)");
                        foreach (Door door in failedDoors)
                        {
                            doorQueue.Enqueue(door);
                        }


                        if (failedDoors.Count == 0)
                        {

                            debugFlag = 1;
                            newRoom = new Room(4, newCoords);
                            rooms[(int)newCoords.x + offSet][(int)newCoords.y + offSet] = newRoom;
                            newRoom.Doors[0].direction = entryDir;

                            List<string> otherDirs = directions.Where(d => d != entryDir).ToList();

                            newRoom.Doors[1].direction = otherDirs[0];
                            newRoom.Doors[2].direction = otherDirs[1];
                            newRoom.Doors[3].direction = otherDirs[2];

                            foreach (Door door in newRoom.Doors)
                            {
                                if (door.direction != entryDir)
                                    doorQueue.Enqueue(door);
                            }
                        }

                        failedDoors.Clear();
                        // continue;
                    }

                    continue;



                }
                //Now we can properly dequeue knowing that this is a confirmed room that can be made
                curDoor = doorQueue.Dequeue();
                curDoor.status = "Connected";

                //Debug.Log("New room will be at " + newCoords + " with " + newDoors +" doors with entry in the " + entryDir);

                //Create new room based on doors and coords
                newRoom = new Room(newDoors, newCoords);
                newRoom.entryDir = entryDir;
                rooms[(int)newCoords.x + offSet][(int)newCoords.y + offSet] = newRoom;
                curCoordinates = newCoords;
                int usableDoors = newDoors - 1; //Doors that could become new rooms

                //Now we need to determine where we came from (IE: if we enter a door from north, we come out of SOUTH in the next room) (Done above with entryDir)
                //entrydir is where we DONT create a new door queue as its where we entered this room
                //Adding entrydir door
                newRoom.Doors[0].direction = entryDir;
                //This door is already connected, so we can connect it
                newRoom.Doors[0].status = "Connected";

                int doornum = 1;

                //Declaring list out here so it doesnt get re-made for each loop
                List<string> alteredDirections = directions.Where(d => d != entryDir).ToList();


                while (usableDoors > 0)
                {
                    //Debug.Log(usableDoors + " usable doors left!");
                    //Here we need to add each door 1 by one
                    //First we determine the direction of this new door at random (not the entrydir)

                    string newdir = alteredDirections[UnityEngine.Random.Range(0, alteredDirections.Count)];
                    //Debug.Log(string.Join(" ", alteredDirections));
                    alteredDirections.Remove(newdir);
                    //From here we need to edit the loop in order to do this right, but we select random directions and enqueue until newdoors dont exist anymore.
                    //Debug.Log(newdir);
                    newRoom.Doors[doornum].direction = newdir;
                    doornum++;
                    usableDoors--;
                }

                //Enqueue new doors

                foreach (Door door in newRoom.Doors)
                {
                    if (door.direction != entryDir)
                        doorQueue.Enqueue(door);
                }


                failsafe++;
                if (failsafe > 100)
                {
                    Debug.Log("Failsafe triggered");
                    break;
                }

                //continue;



            }
            else
            {

                //This Dequeue is only reached if the initial random chance fails
                Door failedDoor = doorQueue.Dequeue();
                failedDoor.status = "Blocked";
                failedDoors.Add(failedDoor);
            }

            if (doorQueue.Count == 0 && temp > 0)
            {
                //Getting in here means we are about to run out of doors without completing the level size requirements, so we skip the dequeue to force-add a door


                //Add all previously blocked doors to the queue
                //Debug.Log("Attemtping failedDoors back");
                foreach (Door door in failedDoors)
                {
                    doorQueue.Enqueue(door);
                }


                //There is a small chance that failedDoors will still be empty after this. 
                //This means that the queue is empty and all previously failed doors are also empty, meaning we are on the last step before deadlock
                //When this happens, we are going to make the current room a 4 door to try and get out in any way possible
                if (failedDoors.Count == 0)
                {
                    debugFlag = 1;
                    newRoom = new Room(4, newCoords);
                    rooms[(int)newCoords.x + offSet][(int)newCoords.y + offSet] = newRoom;
                    newRoom.Doors[0].direction = entryDir;

                    List<string> otherDirs = directions.Where(d => d != entryDir).ToList();

                    newRoom.Doors[1].direction = otherDirs[0];
                    newRoom.Doors[2].direction = otherDirs[1];
                    newRoom.Doors[3].direction = otherDirs[2];

                    foreach (Door door in newRoom.Doors)
                    {
                        if (door.direction != entryDir)
                            doorQueue.Enqueue(door);
                    }
                }

                failedDoors.Clear();




                // continue;
            }






        }
        Debug.Log("Finished with doorCount of " + doorQueue.Count + " and remaining levelsize of " + temp + "\n NOTE: level size of 0 means this worked! Doorqueue size does not matter!");

        string gridOutput = "";

        for (int j = maxDim - 1; j >= 0; j--)
        {
            for (int i = 0; i < maxDim; i++)
            {
                if (rooms[i][j] != null)
                {
                    //Debug.Log(rooms[i][j].flag + " at " + rooms[i][j].Coordinates);
                    if (rooms[i][j].flag == "starter")
                        gridOutput += "S ";
                    else
                        gridOutput += "R ";
                }
                else
                {
                    gridOutput += "0 ";
                }
            }
            gridOutput += "\n"; // Add a newline after each row
        }

        Debug.Log(gridOutput);



        //Room generation is complete, we now have a valid 2d grid of how the rooms will look
        //Now we assign rooms based on what the grid says they should look like

        int totalrooms = 0;
        string message = "";
        int bossRoom = 1;
        foreach (List<Room> roomList in rooms)
        {
            foreach (Room room in roomList)
            {

                if (room != null)
                {
                    string roomStats = "Room located at: ";
                    roomStats = roomStats + room.Coordinates;
                    roomStats = roomStats + " with these doors: ";

                    //We are also fixing and recounting the doorNum as the one generated in part 1 is likely to no longer be accurate. (Not every generated door leads to a room)
                    room.numDoors = 0;
                    List<Door> badDoors = new List<Door>(); //Cant remove items from the same list you are running a for-each on, so i had to make this
                    foreach (Door door in room.Doors)
                    {
                        if (door.status == "Connected")
                        {
                            room.numDoors++;
                            roomStats = roomStats + door.direction + " ";
                        }
                        else
                        {
                            badDoors.Add(door);
                            roomStats = roomStats + door.direction + "F ";
                        }
                    }

                    foreach (Door door in badDoors)
                    {
                        room.Doors.Remove(door);
                    }


                    switch (room.numDoors)
                    {
                        case 1: room.roomType = roomProperties.roomType.OneDoor; break;
                        case 2:
                            {
                                //Debug.Log("Two Doors Room Detected: " + room.Doors[0].direction + " " + room.Doors[1].direction);
                                switch (room.Doors[0].direction)
                                {

                                    case "North": if (room.Doors[1].direction == "South") room.roomType = roomProperties.roomType.TwoDoorOP; else room.roomType = roomProperties.roomType.TwoDoorADJ; break;
                                    case "South": if (room.Doors[1].direction == "North") room.roomType = roomProperties.roomType.TwoDoorOP; else room.roomType = roomProperties.roomType.TwoDoorADJ; break;
                                    case "East": if (room.Doors[1].direction == "West") room.roomType = roomProperties.roomType.TwoDoorOP; else room.roomType = roomProperties.roomType.TwoDoorADJ; break;
                                    case "West": if (room.Doors[1].direction == "East") room.roomType = roomProperties.roomType.TwoDoorOP; else room.roomType = roomProperties.roomType.TwoDoorADJ; break;
                                }
                                //Debug.Log(room.roomType);
                                break;
                            }
                        case 3: room.roomType = roomProperties.roomType.ThreeDoor; break;
                        case 4: room.roomType = roomProperties.roomType.FourDoor; break;
                    }
                    List<SceneData> filteredList = new List<SceneData>();
                    if (room.Coordinates == new Vector2(0, 1))
                    {
                        //Debug.Log("Found room: " + room.Coordinates);
                        filteredList = sceneDataList.Where(sceneData => sceneData.roomType == room.roomType && sceneData.roomtheme == SceneData.roomTheme.Default).ToList();
                        beginningRoom = 0;
                        string list = "";
                        //Debug.Log(room.Coordinates);
                        //Debug.Log(filteredList.Count());
                        foreach (SceneData data in filteredList)
                        {
                            list = list + data.sceneName;
                        }
                        //Debug.Log("List: " + list);
                    }
                    else
                    {
                        
                        if(room.roomType == roomProperties.roomType.OneDoor && bossRoom == 1)
                        {
                            //Debug.Log("Boss Room");
                            filteredList = sceneDataList.Where(sceneData => sceneData.roomType == room.roomType && sceneData.roomtheme == SceneData.roomTheme.Boss).ToList();
                            bossRoom = 0;
                        }
                        else
                        {
                            filteredList = sceneDataList.Where(sceneData => sceneData.roomType == room.roomType && sceneData.roomtheme != SceneData.roomTheme.Boss).ToList();
                        }

                        

                    }

                    int randomNum = UnityEngine.Random.Range(0, filteredList.Count);  //picks randomly from the specified filered list
                    roomStats = roomStats + " Picked room: " + filteredList[randomNum].sceneName + " Filter Count: " + filteredList.Count;
                    SceneData scene = filteredList[randomNum];
                    room.selectedScene = filteredList[randomNum].sceneName;
                    room.selectedSceneData = scene;

                    //Debug.Log("Selected scene is: " + room.selectedScene);

                    //Assuming the rooms are set up correctly, this orients them in the right direction
                    //Gonna use a sepreate switch statement for orientation for easier readability

                    roomProperties.rotation rotation = roomProperties.rotation.Zero;

                    switch (room.roomType)
                    {
                        //For a one door, entrance Preference does not matter
                        case roomProperties.roomType.OneDoor:
                            {
                                switch (room.Doors[0].direction)
                                {
                                    case "North": rotation = roomProperties.rotation.Zero; break;
                                    case "East": rotation = roomProperties.rotation.Ninety; break;
                                    case "South": rotation = roomProperties.rotation.OneEighty; break;
                                    case "West": rotation = roomProperties.rotation.TwoSeventy; break;
                                }
                                break;
                            }
                        case roomProperties.roomType.TwoDoorOP:
                            {
                                switch (room.Doors[0].direction)
                                {

                                    //We should be checking the entrydir here instead, 
                                    //Every room has an assigned entrydir, which is based on where the room generated from. Our job is to match this entrydir with a door that the player
                                    //has assigned as an entrance. 

                                    //EX: A 2 door op room where the player enters in the north. Based on the 


                                    //Technically, NS, and EW have the same rotations here since the room is symmetrical.
                                    case "North": rotation = roomProperties.rotation.Zero; break;
                                    case "East": rotation = roomProperties.rotation.Ninety; break;
                                    case "South": rotation = roomProperties.rotation.OneEighty; break;
                                    case "West": rotation = roomProperties.rotation.TwoSeventy; break;
                                }
                                break;
                            }
                        case roomProperties.roomType.TwoDoorADJ:
                            {



                                //Debug.Log("The grid wants a room with an entrance in: " + room.entryDir);
                                //Debug.Log("The chosen scene has an entrance selected for: " + scene.entranceDir);

                                //We should only do the entrance selecting if there is an entrance dir
                                if (scene.entranceDir != SceneData.EntranceDir.Unassigned)
                                {
                                    //First we need to determine how much to rotate by in order to match up
                                    switch (scene.entranceDir)
                                    {
                                        //In this case, a properly set up room will only have north or east as its direciton
                                        case SceneData.EntranceDir.North:
                                            {
                                                //If the scene desires an entrance in the north, it means the NORTH door is the entrance door, so we must rotate to match entrydir
                                                switch (room.entryDir)
                                                {
                                                    case "North":
                                                        {
                                                            //North means no rotation but we may need to flip
                                                            //We know from earlier code that doors[0] will always be the entry door
                                                            if (room.Doors[1].direction == "West") room.flip = "Horizontal";
                                                            break;
                                                        }

                                                    case "East":
                                                        {
                                                            if (room.Doors[1].direction == "North") room.flip = "Horizontal";
                                                            rotation = roomProperties.rotation.Ninety; break;
                                                        }
                                                    case "South":
                                                        {
                                                            if (room.Doors[1].direction == "East") room.flip = "Horizontal";
                                                            rotation = roomProperties.rotation.OneEighty; break;
                                                        }

                                                    case "West":
                                                        {
                                                            if (room.Doors[1].direction == "South") room.flip = "Horizontal";
                                                            rotation = roomProperties.rotation.TwoSeventy; break;
                                                        }

                                                }
                                                break;
                                            }
                                        case SceneData.EntranceDir.East:
                                            {
                                                //If the scene desires an entrance in the north, it means the NORTH door is the entrance door, so we must rotate to match entrydir
                                                switch (room.entryDir)
                                                {
                                                    case "North":
                                                        {
                                                            if (room.Doors[1].direction == "East") room.flip = "Horizontal";
                                                            rotation = roomProperties.rotation.TwoSeventy; break;
                                                        }

                                                    case "East":
                                                        {
                                                            if (room.Doors[1].direction == "South") room.flip = "Vertical";
                                                            break;
                                                        }
                                                    case "South":
                                                        {
                                                            if (room.Doors[1].direction == "West") room.flip = "Horizontal";
                                                            rotation = roomProperties.rotation.Ninety; break;
                                                        }
                                                    case "West":
                                                        {
                                                            if (room.Doors[1].direction == "North") room.flip = "Vertucal";
                                                            rotation = roomProperties.rotation.OneEighty; break;
                                                        }

                                                }
                                                break;
                                            }
                                        default: Debug.LogError("Room: " + scene.name + " is not properly set up! Make sure the room follows rotation standards"); break;

                                    }


                                    break;
                                }


                                switch (room.Doors[0].direction)
                                {
                                    case "North":
                                        {
                                            //Since this ADJ we know the other door will be either east or west
                                            if (room.Doors[1].direction == "East") rotation = roomProperties.rotation.Zero; else rotation = roomProperties.rotation.TwoSeventy;
                                            break;
                                        }
                                    case "South":
                                        {
                                            if (room.Doors[1].direction == "East") rotation = roomProperties.rotation.Ninety; else rotation = roomProperties.rotation.OneEighty;
                                            break;
                                        }
                                    case "East":
                                        {
                                            //Same as before, but north and south
                                            if (room.Doors[1].direction == "North") rotation = roomProperties.rotation.Zero; else rotation = roomProperties.rotation.Ninety;
                                            break;
                                        }
                                    case "West":
                                        {
                                            if (room.Doors[1].direction == "North") rotation = roomProperties.rotation.TwoSeventy; else rotation = roomProperties.rotation.OneEighty;
                                            break;
                                        }

                                }
                                break;
                            }
                        case roomProperties.roomType.ThreeDoor:
                            {
                                //Its easier here if we figure out the door we DONT have

                                string dirWithoutDoor = directions
                                .Where(d => d != room.Doors[0].direction &&
                                d != room.Doors[1].direction &&
                                d != room.Doors[2].direction)
                                .FirstOrDefault();

                                switch (dirWithoutDoor)
                                {
                                    case "North": rotation = roomProperties.rotation.OneEighty; break;
                                    case "East": rotation = roomProperties.rotation.TwoSeventy; break;
                                    case "South": rotation = roomProperties.rotation.Zero; break;
                                    case "West": rotation = roomProperties.rotation.Ninety; break;
                                }

                                break;
                            }

                        //Four door rotation doesnt matter so we skip
                        case roomProperties.roomType.FourDoor: rotation = roomProperties.rotation.Zero; break;

                    }

                    room.roomDir = rotation;

                    roomStats = roomStats + " With rotation: " + rotation.ToString();

                    if (room.flip != null) roomStats = roomStats + " And Flip set to: " + room.flip;



                    message = message + roomStats + "\n";
                }


            }
        }

        Debug.Log(message);

        //Debug.Log(debugFlag);



        //Secondary runthrough of rooms to add collectibles and bosses.


        List<Room> roomsWithCollectible = new List<Room>();


        foreach (List<Room> roomList in rooms)
        {
            foreach (Room room in roomList)
            {

                if (room != null)
                {

                    if (room.selectedSceneData.hasCollectible)
                    {
                        roomsWithCollectible.Add(room);
                    }

                    if (room.roomType == roomProperties.roomType.OneDoor && room.selectedSceneData.roomtheme != SceneData.roomTheme.Boss)
                    {
                        roomsWithHealth.Add(room);
                    }
                }
            }
        }

        List<int> randomNumbers = new List<int>();

        while (randomNumbers.Count < 3)
        {
            int randomNumber = UnityEngine.Random.Range(0, roomsWithCollectible.Count);
            if (!randomNumbers.Contains(randomNumber))
            {
                randomNumbers.Add(randomNumber);
            }
        }


        

        collectibleCoords.Add(roomsWithCollectible[randomNumbers[0]].Coordinates);
        collectibleCoords.Add(roomsWithCollectible[randomNumbers[1]].Coordinates);
        collectibleCoords.Add(roomsWithCollectible[randomNumbers[2]].Coordinates);

        foreach(Vector2 coord in collectibleCoords)
        {
            //Debug.Log(coord);
        }

        //Finally we load the first scene

        Room firstRoom = getRoom(rooms, new Vector2(0, 1));

        SceneManager.LoadScene(firstRoom.selectedScene);

        //LoadMap
        GameObject.Find("miniMapRaw").GetComponent<Map>().LoadMap();

    }

    public bool roomExists(List<List<Room>> rooms, Vector2 coordinates)
    {
        int x = (int)coordinates.x + offSet;
        int y = (int)coordinates.y + offSet;

        return rooms[x][y] != null;
    }

    public Room getRoom(List<List<Room>> rooms, Vector2 coordinates)
    {
        int x = (int)coordinates.x + offSet;
        int y = (int)coordinates.y + offSet;

        return rooms[x][y];
    }

    public void Initalize()
    {
        generate = true;
    }
    
    public int GetSeed()
    {
        return seed;
    }
}
