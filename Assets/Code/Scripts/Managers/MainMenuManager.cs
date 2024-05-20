using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;

public class MainMenuScript : MonoBehaviour
{
    public GameObject AccountFlow;
    public GameObject settingsMenu;
    public GameObject mainMenu;
    public TMP_Dropdown graphicsDropdown;
    public Scene scene;
    // Start is called before the first frame update
    void Start()
    {
        //Button button = GetComponent<Button>();
    }

    public void OpenGame(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        // Quit the application (works in standalone builds)
        Application.Quit();
        Debug.Log("Quitting game");
    }

    public void ToggleAccountFlow()
    {
        if (!AccountFlow.activeInHierarchy)
        {
            AccountFlow.SetActive(true);
        }
        else
        {
            AccountFlow.SetActive(false);
        }
    }


    /*------------------------- Settings functions ---------------------*/
    public void OpenSettings()
    {
        if (!settingsMenu.activeInHierarchy)
        {
            mainMenu.SetActive(false);
            settingsMenu.SetActive(true);
        }
        else
        {
            mainMenu.SetActive(true);
            settingsMenu.SetActive(false);
        }
    }

    public void ChangeGraphicsQuality()
    {
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
    }
}
