using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectPopUp : MonoBehaviour, IPointerClickHandler
{
    public GameObject popUpPanel;     // The pop-up panel to show/hide
    public GameObject parentUIPanel;  // The parent UI panel containing the object
    public GameObject blurredImage;

    private void Start()
    {
        // Initially hide the pop-up panel
        popUpPanel.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if the parent UI panel and blurred image are active
        if (parentUIPanel.activeSelf && blurredImage.activeSelf)
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
