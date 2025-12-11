using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] Button mainMenuButton;
    [SerializeField] private string LevelManagerScene;
    [SerializeField] private string MainMenuScene;
    private GameObject pauseMenuObject;

    // Start is called before the first frame update
    void Start()
    {
        continueButton.onClick.AddListener(OnContinueClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        pauseMenuObject = GameObject.FindGameObjectWithTag("PauseMenu");
    }

    void Update()
    {
        if (Input.GetButtonUp("Cancel"))
        {
            OnContinueClicked();
        }
    }

    void OnContinueClicked()
    {
        Debug.Log("Play triggered");
        GameObject.FindGameObjectWithTag("Player")
            .GetComponent<playerMovement>()
            .FreezePlayer(false);
        pauseMenuObject.SetActive(false);
    }

    void OnMainMenuClicked()
    {
        Debug.Log("Quit triggered");
        pauseMenuObject.SetActive(false);
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            Destroy(obj);
        }
        SceneManager.LoadScene(MainMenuScene);
    }

}
