using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room 
{
    public Vector2 Coordinates { get; private set; }
    public int numDoors;

    public string selectedScene;
    public SceneData selectedSceneData;

    public List<Door> Doors = new List<Door>();
    public roomProperties.roomType roomType;
    public roomProperties.rotation roomDir;
    public string entryDir;
    public string flip;

    public string flag;


    public Room(string flag)
    {
        if(flag == "starter")
        {
            this.Coordinates = new Vector2(0 ,1);
            this.numDoors = 3;
            
            Door North = new Door("North", this.Coordinates);
            Door East = new Door("East", this.Coordinates);
            Door West = new Door("West", this.Coordinates);

            Doors.Add(North);
            Doors.Add(East);
            Doors.Add(West);

            this.roomType = roomProperties.roomType.ThreeDoor;

            this.flag = "starter";
            this.flip = null;
        }

        if (flag == "spawn")
        {
            this.Coordinates = new Vector2(0, 0);
            this.numDoors = 1;

            Door North = new Door("North", this.Coordinates);

            Doors.Add(North);

        }

    }

    public Room(int numDoors, Vector2 coordinates)
    {
        this.Coordinates = coordinates;
        this.numDoors = numDoors;
        for(int i = 0; i < numDoors; i++)
        {
            Door newDoor = new Door(this.Coordinates);
            Doors.Add(newDoor);
        }
        this.flag = null;
        this.flip = null;
    }
    public Room()
    {
        Coordinates = Vector2.zero;
        numDoors = 0;
        this.flag = null;
    }
}
