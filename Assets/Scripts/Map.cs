using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    // Start is called before the first frame update

    Vector2 startPos;
    Vector2 endPos;
    float elapsedTime = 0f;
    float duration = 1f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    private List<GameObject> rooms = new List<GameObject>();

    public Sprite[] sprites;
    public GameObject PlayerSprite;
    Dictionary<Vector2, GameObject> collectibleMapSpots = new Dictionary<Vector2, GameObject>();
    Dictionary<Room, GameObject> healthMapSpots = new Dictionary<Room, GameObject>();


    void Start()
    {
        startPos = Vector2.zero;
        endPos = Vector2.zero;
    }

    private void Update()
    {
        
    }

    //Function to be called when crossing doors
    public void playerMovement(float x, float y) // x is number of rooms to the right and Y is number of room upwards
    {
        StopAllCoroutines();
        StartCoroutine(InterpolatePosition(x, y));
       
    }
    
    

    IEnumerator InterpolatePosition(float x, float y)
    {
                elapsedTime = 0f;
        duration = 1f;
        RectTransform transform = PlayerSprite.GetComponent<RectTransform>();
        if (endPos != Vector2.zero) transform.anchoredPosition = endPos;

        startPos = transform.localPosition;
        endPos = transform.localPosition;
        endPos.x += (x * 120);
        endPos.y += (y * 120);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.anchoredPosition = endPos;
    }


    public enum roomTheme { Default, Puzzle, TimedPuzzle, Boss };

    public void LoadMap()
    {
        Canvas canvas = GetComponent<Canvas>();

        Vector3 SpritePosition = new Vector3(0, 0, 0);
        // Instantiate the first sprite
        //
        



        LevelGenManager LevelGenRef = GameObject.Find("Level Manager").GetComponent<LevelGenManager>(); //Use this

        List<SceneData> sceneData= LevelGenRef.sceneDataList;

        

        //Logic is in theory done

        //MISSING:
        //-Obtaining Scene data and assiging it to room Theme DONE
        //-Room sprites and rectangles by zahir.
        //-Check if the room contains collectibles or not.
        //-Player transition between rooms DONE

        int roomWidth = 120;
        float maxX = -100;
        float minY =  100;
        
        foreach (List<Room> roomList in LevelGenRef.rooms)
        {
            foreach (Room room in roomList)
            {

                if (room != null)
                {
                    Vector2 curCoords = room.Coordinates;
                    List<Door> curDoors = room.Doors;
                    GameObject RoomSprite = new GameObject("Room "+curCoords.x+" "+curCoords.y);
                    if(curCoords.x>maxX) maxX= curCoords.x;
                    if (curCoords.y < minY) minY= curCoords.y;
                    // Get room type through scene data

                    SceneData.roomTheme roomTheme = room.selectedSceneData.roomtheme; ;

                    switch (roomTheme)
                    {
                        case (SceneData.roomTheme.Default): //Default
                            RoomSprite.AddComponent<Image>().sprite = sprites[0];
                            break;
                        case (SceneData.roomTheme.Puzzle): //Untimed Puzzle
                            RoomSprite.AddComponent<Image>().sprite = sprites[1];
                            break;
                        case (SceneData.roomTheme.TimedPuzzle): //Timed Puzzle
                            RoomSprite.AddComponent<Image>().sprite = sprites[2];
                            break;
                        case (SceneData.roomTheme.Boss): //Boss
                            RoomSprite.AddComponent<Image>().sprite = sprites[3];
                            break;
                        default:
                            RoomSprite.AddComponent<Image>().sprite = sprites[0];
                            break;

                    }
                    RectTransform trans = RoomSprite.GetComponent<RectTransform>();
                    RoomSprite.transform.SetParent(canvas.transform);
                    RoomSprite.transform.localScale = new Vector3(1f, 1f, 1f); ;
                    SpritePosition.x = roomWidth * curCoords.x;
                    SpritePosition.y = roomWidth * curCoords.y;
                    trans.anchoredPosition = SpritePosition;



                    if (LevelGenRef.collectibleCoords.Contains(room.Coordinates))
                    {
                        GameObject SmallSquare = new GameObject("CollectibleIndicator");
                        //SmallSquare.tag = "MMcollectible";

                        collectibleMapSpots[room.Coordinates] = SmallSquare;

                        Image squareImage = SmallSquare.AddComponent<Image>();
                        squareImage.color = Color.cyan; 

                        RectTransform smallTrans = SmallSquare.GetComponent<RectTransform>();
                        SmallSquare.transform.SetParent(RoomSprite.transform); // Attach to RoomSprite
                        SmallSquare.transform.localScale = Vector3.one; // Normal scale

                        // Position in top-left corner relative to RoomSprite's center
                        float offset = roomWidth / 2 - 10; // Slightly inset from edges
                        smallTrans.anchoredPosition = new Vector2(-offset, offset);

                        // Size for the small square
                        smallTrans.sizeDelta = new Vector2(20f, 20f); // Width and height of the square
                    }

                    if (LevelGenRef.roomsWithHealth.Contains(room))
                    {
                        GameObject SmallSquare = new GameObject("HealthIndicator");
                        //SmallSquare.tag = "MMhealth";

                        healthMapSpots[room] = SmallSquare;

                        Image squareImage = SmallSquare.AddComponent<Image>();
                        squareImage.color = Color.red;

                        RectTransform smallTrans = SmallSquare.GetComponent<RectTransform>();
                        SmallSquare.transform.SetParent(RoomSprite.transform); // Attach to RoomSprite
                        SmallSquare.transform.localScale = Vector3.one; // Normal scale

                        // Position in top-left corner relative to RoomSprite's center
                        float offset = roomWidth / 2 - 10; // Slightly inset from edges
                        smallTrans.anchoredPosition = new Vector2(offset, offset);

                        // Size for the small square
                        smallTrans.sizeDelta = new Vector2(20f, 20f); // Width and height of the square
                    }


                    //Debug.Log("Created room Sprite for " + curCoords.x + " " + curCoords.y);

                    foreach (Door door in curDoors)
                    {
                        GameObject DoorSprite = new GameObject("Door");
                        DoorSprite.AddComponent<Image>().sprite = sprites[4];
                        
                        trans = DoorSprite.GetComponent<RectTransform>();

                        DoorSprite.transform.SetParent(canvas.transform);
                        DoorSprite.transform.localScale = new Vector3(0.2f, 0.2f, 1f); ;
                        

                        if(door.direction=="North"){
                            SpritePosition.x = roomWidth * curCoords.x;
                            SpritePosition.y = roomWidth * curCoords.y + 60;
                        }
                        else if (door.direction == "South")
                        {
                            SpritePosition.x = roomWidth * curCoords.x;
                            SpritePosition.y = roomWidth * curCoords.y -60;
                        }
                        else if (door.direction == "West")
                        {
                            SpritePosition.x = roomWidth * curCoords.x -60;
                            SpritePosition.y = roomWidth * curCoords.y;
                        }
                        else if (door.direction == "East")
                        {
                            SpritePosition.x = roomWidth * curCoords.x + 60;
                            SpritePosition.y = roomWidth * curCoords.y;
                        }

                        trans.anchoredPosition = SpritePosition;

                    }
                    
                }
            }
            
        }

        PlayerSprite = new GameObject();
        PlayerSprite.name = "Player";
        PlayerSprite.AddComponent<Image>().sprite = sprites[5]; // Player Sprite
        RectTransform transPlayer = PlayerSprite.GetComponent<RectTransform>();
        PlayerSprite.transform.SetParent(canvas.transform);
        PlayerSprite.transform.localScale = new Vector3(0.5f, 0.5f, 1f); ;
        SpritePosition.x = 0;
        SpritePosition.y = 120;
        transPlayer.anchoredPosition = SpritePosition;

        Vector3 position=this.GetComponent<RectTransform>().localPosition;
        position.x -= maxX * 120;
        position.y -= (minY-1) * 120;
        this.GetComponent<RectTransform>().localPosition = position;
    }

    public void ReloadCollectible(Vector2 coordinates)
    {
        LevelGenManager LevelGenRef = GameObject.Find("Level Manager").GetComponent<LevelGenManager>();
        foreach (List<Room> roomList in LevelGenRef.rooms)
        {
            foreach (Room room in roomList)
            {

                if (room != null)
                {
                    if(room.Coordinates == coordinates)
                    {
                        GameObject collectibleIcon = collectibleMapSpots[room.Coordinates];
                        Destroy(collectibleIcon);
                    }
                }
            }
        }
    }
    public void ReloadHealthContainer(Vector2 coordinates)
    {
        LevelGenManager LevelGenRef = GameObject.Find("Level Manager").GetComponent<LevelGenManager>();
        foreach (List<Room> roomList in LevelGenRef.rooms)
        {
            foreach (Room room in roomList)
            {

                if (room != null)
                {
                    if (room.Coordinates == coordinates)
                    {
                        GameObject HealthIcon = healthMapSpots[room];
                        Destroy(HealthIcon);
                    }
                }
            }
        }
    }
}

