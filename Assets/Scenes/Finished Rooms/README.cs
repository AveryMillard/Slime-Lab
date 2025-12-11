/*
 * HOW TO ADD A NEW ROOM
 * 
 * 1. All elements belonging to the room itself (Walls / doors / buttons / lighting) must be packed into one gameobject whos transform is at 0,0,0
 *    Additionally, use the following layouts
 *    1 room: Place the door on the north end
 *    2 room OP: North and south
 *    2 room ADJ: North and to the right
 *    3 room: South side should NOT have a door
 *    4 room: doesnt matter
 * 2. This gameobject must have the roomProperties component filled out
 * 3. Name the Scene the same thing you named the room in roomProperties
 * 4. Place the scene in the corresponding folder
 * 5. Right click, create -> SceneData
 * 6. Match the data in the SceneData object with the roomProperties component. (The name of the object in the files isnt too important, so just name it the same for clarity sake
 * 7. Place the sceneData in Assets -> Resources -> SceneData
 */