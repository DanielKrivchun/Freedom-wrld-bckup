using UnityEngine;

public class ObjectPopUp : MonoBehaviour
{
    public GameObject popUpPanel;     // The pop-up panel to show/hide
    public GameObject parentUIPanel;  // The parent UI panel containing the object

    private void Start()
    {
        // Initially hide the pop-up panel
        popUpPanel.SetActive(false);
    }

    private void OnMouseDown()
    {
        // Check if the parent UI panel is active before toggling the pop-up
        if (parentUIPanel.activeSelf)
        {
            // Toggle the pop-up panel's visibility
            popUpPanel.SetActive(!popUpPanel.activeSelf);
        }
        else
        {
            // Ensure the pop-up panel is hidden if the parent UI panel is inactive
            popUpPanel.SetActive(false);
        }
    }
}
