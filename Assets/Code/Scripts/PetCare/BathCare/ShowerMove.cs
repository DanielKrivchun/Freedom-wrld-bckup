using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowerMove : MonoBehaviour
{
    public float xOffset = 5f;
    public float yOffset = 5f;

    public PetCareStateManager petCareStateManager;

    private Vector3 posOffset;
    private bool isDragging = false;

    void OnMouseDown()
    {
        posOffset = transform.localPosition - GetMouseWorldPosition();
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;

        if (petCareStateManager.isReadyForBath)
        {
            petCareStateManager.ManageCleanlinessDataFiller(25);
            petCareStateManager.isReadyForBath = false;
        }
    }

    void Update()
    {
        if (isDragging && petCareStateManager.isReadyForBath)
        {
            // Calculate the new position based on mouse movement
            Vector3 newPosition = GetMouseWorldPosition() + posOffset;

            // Clamp the new position to the specified range
            newPosition.x = Mathf.Clamp(newPosition.x, -xOffset, xOffset);
            newPosition.y = Mathf.Clamp(newPosition.y, -2, 0);

            // Update the object's position
            transform.localPosition = newPosition;            
        }
    }

    //Get mouse position in world
    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
