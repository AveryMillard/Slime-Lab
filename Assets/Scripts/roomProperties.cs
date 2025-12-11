using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class roomProperties : MonoBehaviour
{
    [SerializeField] public string roomName;
    public enum roomType { OneDoor, TwoDoorOP, TwoDoorADJ, ThreeDoor, FourDoor }

    public enum Direction { North = 1, East, South, West }
    public enum rotation { Zero = 0, Ninety = 90, OneEighty = 180, TwoSeventy = 270 }

    [SerializeField] public roomType RoomType;
    [SerializeField] public rotation Orientation;

    [SerializeField, ReadOnly] private int doorCount;
    private string flip;

    private GameObject levelGen;
    private LevelGenManager levelGenManager;
    private GameObject player;


    void Start()
    {
        //Debug.Log("Start Script ran");

        levelGen = GameObject.FindWithTag("LevelGen");
        levelGenManager = levelGen.GetComponent<LevelGenManager>();


        player = GameObject.FindWithTag("Player");
        Vector2 newCoords = player.GetComponent<PlayerManager>().playerCoords;

        if (levelGenManager.generate)
        {
            //Debug.Log("levelGen.generate is on!");
            Room currentRoom = levelGenManager.getRoom(levelGenManager.rooms, newCoords);
            Orientation = currentRoom.roomDir;
            flip = currentRoom.flip;
        }
        else
        {
            //Debug.Log("levelGen.generate is off!");
            rotateDoors((int)Orientation);
        }

      

        

        switch (RoomType)
        {
            case roomType.OneDoor: doorCount = 1; break;
            case roomType.TwoDoorOP: doorCount = 2; break;
            case roomType.TwoDoorADJ: doorCount = 2; break;
            case roomType.ThreeDoor: doorCount = 3; break;
            case roomType.FourDoor: doorCount = 4; break;

        }





        
        
    }


    public void rotateDoors(int degrees)
    {

        //Debug.Log(degrees);
        switch ((rotation)degrees)
        {
            case rotation.Zero: transform.rotation = Quaternion.Euler(0, 0, 0); break;
            case rotation.Ninety: transform.rotation = Quaternion.Euler(0, 90, 0); break;
            case rotation.OneEighty: transform.rotation = Quaternion.Euler(0, 180, 0); break;
            case rotation.TwoSeventy: transform.rotation = Quaternion.Euler(0, 270, 0); break;

        }
        List<GameObject> doors;

        doors = GameObject.FindGameObjectsWithTag("Door").ToList();
        //Debug.Log("Rotate Doors!");
        int turns = degrees / 90;   //Degrees is expected to always be a 90 multiple)

        while (turns > 0)
        {
            //Debug.Log("cur turns is " + turns);
            turns--;
            //Debug.Log("This room has " + doors.Count + "Doors");
            foreach (GameObject door in doors)
            {
                int newDir = (int)door.GetComponent<doorLogic>().Direction;
                newDir++;
                if (newDir == 5) newDir = 1;
                //Debug.Log("This door has newDir of :" + (doorLogic.direction)newDir);
                door.GetComponent<doorLogic>().Direction = (doorLogic.direction)newDir;
                //Debug.Log(door.GetComponent<doorLogic>().Direction);
            }

        }
    }

    public void flipRoom(string direction)
    {

        List<GameObject> doors;

        doors = GameObject.FindGameObjectsWithTag("Door").ToList();

        if (direction == "Horizontal")
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;

            foreach (GameObject door in doors)
            {
                int newDir = (int)door.GetComponent<doorLogic>().Direction;

                //East becomes west, west becomes east, north and south dont change

                if(newDir == 2) newDir = 4;
                if (newDir == 4) newDir = 2;
                door.GetComponent<doorLogic>().Direction = (doorLogic.direction)newDir;
            }
        }
        if (direction == "Vertical")
        {
            Vector3 scale = transform.localScale;
            scale.z *= -1;
            transform.localScale = scale;

            foreach (GameObject door in doors)
            {
                int newDir = (int)door.GetComponent<doorLogic>().Direction;

                //Same but for north and south

                if (newDir == 1) newDir = 3;
                else if (newDir == 3) newDir = 1;
                door.GetComponent<doorLogic>().Direction = (doorLogic.direction)newDir;
            }

        }
    }

}
