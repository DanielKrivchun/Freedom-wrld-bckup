using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Soap : MonoBehaviour
{
    public PetCareStateManager petCareStateManager;

    private Vector3 posOffset;
    private bool isDragging = false;
    Vector3 startPos;

    RaycastHit hit;

    private void Start()
    {
        startPos = transform.position;
    }

    void OnMouseDown()
    {
        posOffset = transform.position - GetMouseWorldPosition();
        isDragging = true;

        
    }

    void OnMouseUp()
    {
        isDragging = false;
        transform.DOMove(startPos, 0.5f);

        /*if (petCareStateManager.isReadyForBath)
        {
            petCareStateManager.ManageCleanlinessDataFiller(15);
        }*/
    }

    void Update()
    {
        if (isDragging && petCareStateManager.isReadyForBath)
        {
            // Calculate the new position based on mouse movement
            Vector3 newPosition = GetMouseWorldPosition() + posOffset;

            // Update the object's position
            transform.position = newPosition;

            if (Physics.Raycast(transform.position, Vector3.forward, out hit, 100f))
            {
                Debug.Log("Found an object: " + hit.collider.gameObject.name);
                Debug.DrawRay(transform.position, Vector3.forward, Color.yellow);
            }
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
