using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OpenWebsiteButton : MonoBehaviour
{
    public string url = "http://192.168.1.67:3000/login";

    // Custom URL scheme to redirect back to the app
    public string appUrlScheme = "myapp://";

    void Start()
    {
        // Get the button component and set up the listener
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OpenWebsite);
        }
    }

    public void OpenWebsite()
    {
        // Open the URL in the device's default web browser
        /*Application.OpenURL($"{url}?redirect={appUrlScheme}");*/

        Debug.Log("Attempting to open URL: " + url);
        Application.OpenURL($"{url}");
    }
}
