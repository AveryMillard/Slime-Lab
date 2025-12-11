using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button quitButton;
    [SerializeField] private string LevelManagerScene="LevelGenScene";

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    void Update()
    {
        if (Input.GetButtonUp("Cancel"))
        {
            OnQuitClicked();
        }
    }

    void OnStartClicked()
    {
        Debug.Log("Play triggered");
        SceneManager.LoadScene(LevelManagerScene);
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            Destroy(obj);
        }
        LevelGenManager.Instance.Initalize();
    }

    void OnQuitClicked()
    {
        Debug.Log("Quit triggered");
        Application.Quit();
    }

}
