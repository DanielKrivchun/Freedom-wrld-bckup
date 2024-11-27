using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimate : MonoBehaviour
{
    [Header("Manually Assign Buttons to Animate")]
    public List<_ButtonHolder> m_buttons = new List<_ButtonHolder>();

    // Initialize the assigned buttons by setting up their click events
    public void InitializeButtons()
    {
        foreach (var buttonHolder in m_buttons)
        {
            if (buttonHolder != null && buttonHolder.m_t != null)
            {
                // Check if the Transform has a Button component
                Button button = buttonHolder.m_t.GetComponent<Button>();
                if (button != null)
                {
                    // Add a click event listener to the button
                    button.onClick.AddListener(() => AnimateButton(buttonHolder.m_t));
                }
                else
                {
                    Debug.LogWarning($"Transform '{buttonHolder.m_t.name}' does not have a Button component.");
                }
            }
        }
    }

    // Animate the button when clicked
    private void AnimateButton(Transform buttonTransform)
    {
        Utils._DoButtonAnimation(buttonTransform);
    }

    // Optionally initialize on start
    private void Start()
    {
        InitializeButtons();
    }
}