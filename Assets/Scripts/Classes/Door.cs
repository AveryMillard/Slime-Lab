using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door
{
    public string direction;
    public Vector2 doorCoords; //The room coodinated this door belongs to

    public string status;
    /* Door Statuses"
     * Disconnected: Generated but no connection to another room, any disconnected will be erased in room gen
     * Connected: Has a room connection
     * Blocked: Rolled bad and is not able to be generated. These will be re-queued if needed
     */

    public Door(string direction, Vector2 doorCoords)
    {
        this.direction = direction;
        this.doorCoords = doorCoords;
        this.status = "Disconnected";
    }   

    public Door(Vector2 doorCoords)
    {
        this.direction = null;
        this.doorCoords = doorCoords;
        this.status = "Disconnected";
    }
}
