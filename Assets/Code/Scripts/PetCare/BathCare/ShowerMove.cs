using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowerMove : MonoBehaviour
{
    public float xOffset;
    public float yOffset;

    [Space]
    public ParticleSystem waterShowerEffect;

    [Space]
    public BathObject bathObject;
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

        //Checking for Soap used and all foam bubble cleared or not
        /*if (bathObject.isSoapUsed && !bathObject.isShowerUsed && particleEffectsManager.IsAllFoamCleared())
        {
            bathObject.isShowerUsed = true;
            PetCareStateManager.instance.ManageCleanlinessDataFiller(particleEffectsManager.numOfFoamBubbles * bathObject.cleanlinessMultiplier);
            particleEffectsManager.numOfFoamBubbles = 0;

            //Reset player position
            StartCoroutine(bathObject.ResetPlayerToMainPosition());
        }*/
    }

    void Update()
    {
        if (isDragging && bathObject.isSoapUsed && !bathObject.isShowerUsed)
        {
            // Calculate the new position based on mouse movement
            Vector3 newPosition = GetMouseWorldPosition() + posOffset;

            // Clamp the new position to the specified range
            newPosition.x = Mathf.Clamp(newPosition.x, -xOffset, xOffset);
            newPosition.y = Mathf.Clamp(newPosition.y, -yOffset, 0);

            // Update the object's position
            transform.localPosition = newPosition;

            //Off foam bubble particles using raycast
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1000f))
            {
                if (hit.collider.CompareTag(_Strings.FoamBubble))
                {
                    particleEffectsManager.CheckAndStopFoamBubbleEffect(hit.collider.gameObject.GetComponent<ParticleSystem>());
                }
            }

            //Play shower water effect
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
