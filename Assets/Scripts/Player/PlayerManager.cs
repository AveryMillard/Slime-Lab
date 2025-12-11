using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour,IDamageable
{
    [SerializeField] GameObject levelManager;
    [SerializeField] static public string GameOverScene = "GameOverScene";
    [SerializeField, ReadOnly] public Vector2 playerCoords;
    [SerializeField] public static int lives = 3;
    [SerializeField] public int collectibles;
    [SerializeField] int winAmount;
    HealthBar HealthRef;
    public int roomChangeFlag = 0;

    private static PlayerManager _instance;
    public static playerMovement pMovementRef;
    private Tip Tip;
    //private PlayerLocomotion pMovementRef;

    //new player movement 
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;

    [SerializeField] public bool isOnIce;
    public TMP_Text collectibleText;
    public static PlayerManager Instance
    {
        get
        {
            return _instance;
        }
    }

    void Start()
    {
        collectibleText = GameObject.Find("CollectibleCounter").GetComponent<TextMeshProUGUI>();

        lives = 3;
        HealthRef = GetComponent<HealthBar>();
        pMovementRef = GetComponent<playerMovement>();
        Tip = GameObject.Find("Tip").GetComponent<Tip>();

        playerCoords = new Vector2(0, 1);
        collectibles = 0;
        winAmount = 3;
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }

        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
        LevelGenManager lvlManager = levelManager.GetComponent<LevelGenManager>();
        playerCoords = new Vector2(0, 1);

        //player movement and ice
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        isOnIce = false;
    }


    private void Update()
    {
        inputManager.HandleAllInputs(); //new player movement
    }

    private void FixedUpdate()
    {
        //if (!isOnIce)
        // playerLocomotion.HandleAllMovement(); //new player movement 

    }
    public void Die()
    {
        TakeLives(1);
        Respawn();
    }
    public void IncreaseCollectibleCount()
    {
        collectibles++;
        if (collectibles == 1)
        {
            pMovementRef.currentspeed = pMovementRef.playerSpeed * 1.2f;
        }
        else if (collectibles == 2)
        {
            pMovementRef.playerSpeed = pMovementRef.playerSpeed * 1.5f;
        }
        else
        {
            pMovementRef.playerSpeed = pMovementRef.playerSpeed * 2f;
        }

        Tip.setText("Movement speed increased!");
        collectibleText.text = collectibles + "/3";
        /* Win state is no longer collectibles, but beating the boss.
        if (collectibles >= winAmount)
        {
            //Execute Win State
        }
        */

    }

    public void TakeLives(int lives)
    {
        HealthRef.TakeLives(lives);
    }

    public void takeDamage(int dmg)
    {
        TakeLives(1);
    }

    public void GetHeartContainer()
    {
        HealthRef.numOfHearts++;
        HealthRef.health = HealthRef.numOfHearts;
        Tip.setText("Health restored and increased by one point!");
    }


    public void Respawn()
    {
        this.transform.position = pMovementRef.spawnPos;
    }

    public int GetLives()
    {
        return lives;
    }

    static public void GameOver()
    {
        SceneManager.sceneLoaded -= pMovementRef.OnSceneLoaded;
        SceneManager.LoadScene(GameOverScene);
    }

}
