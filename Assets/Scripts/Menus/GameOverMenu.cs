using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] Button mainMenuButton;
    [SerializeField] private string MainMenuScene;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Joystick1Button0))
        {
            OnMainMenuClicked();
        }
    }

    void OnMainMenuClicked()
    {
        Debug.Log("Quit triggered");
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            Destroy(obj);
        }
        SceneManager.LoadScene(MainMenuScene);
    }
}
