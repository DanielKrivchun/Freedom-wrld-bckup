using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    public GameObject AccountFlow;

    // Start is called before the first frame update
    void Start()
    {
        //Button button = GetComponent<Button>();
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
}
