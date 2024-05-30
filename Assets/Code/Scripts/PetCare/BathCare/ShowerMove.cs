using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowerMove : MonoBehaviour
{
    public float xOffset = 5f;
    public float yOffset = 5f;

    [Space]
    public ParticleSystem waterShowerEffect;

    [Space]
    public CleanObject cleanObject;
    public ParticleEffectsManager particleEffectsManager;

    private Vector3 posOffset;
    private bool isDragging = false;

    RaycastHit hit;

    void OnMouseDown()
    {
        posOffset = transform.localPosition - GetMouseWorldPosition();
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;
        waterShowerEffect.Stop();

        if (cleanObject.isSoapUsed && !cleanObject.isShowerUsed && particleEffectsManager.IsAllFoamCleared())
        {
            cleanObject.isShowerUsed = true;
            Debug.Log("All FoamBubbles Cleared!");
            //PetCareStateManager.instance.ManageCleanlinessDataFiller(15);
        }
    }

    void Update()
    {
        if (isDragging && cleanObject.isSoapUsed && !cleanObject.isShowerUsed)
        {
            // Calculate the new position based on mouse movement
            Vector3 newPosition = GetMouseWorldPosition() + posOffset;

            // Clamp the new position to the specified range
            newPosition.x = Mathf.Clamp(newPosition.x, -xOffset, xOffset);
            newPosition.y = Mathf.Clamp(newPosition.y, -yOffset, 0);

            // Update the object's position
            transform.localPosition = newPosition;

            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1000f))
            {
                if (hit.collider.CompareTag("FoamBubble"))
                {
                    Debug.Log("Found an object: " + hit.collider.gameObject.name);
                    Debug.DrawRay(transform.position, Vector3.down, Color.red);

                    particleEffectsManager.CheckAndStopFoamBubbleEffect(hit.collider.gameObject.GetComponent<ParticleSystem>());
                    //isPlayerFound = true;
                }
            }

            if (!waterShowerEffect.isPlaying)
            {
                waterShowerEffect.Play();
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
