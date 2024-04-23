using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
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
}
