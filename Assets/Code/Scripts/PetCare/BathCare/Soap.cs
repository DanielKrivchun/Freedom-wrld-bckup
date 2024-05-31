using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Soap : MonoBehaviour
{
    public BathObject bathObject;
    public ParticleEffectsManager particleEffectsManager;

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

        if (bathObject.isReadyForBath && !bathObject.isSoapUsed)
        {
            bathObject.isReadyForBath = false;
            bathObject.isSoapUsed = true;
            PetCareStateManager.instance.ManageCleanlinessDataFiller(particleEffectsManager.numOfFoamBubbles * bathObject.cleanlinessMultiplier);
        }
    }

    void Update()
    {
        if (isDragging && bathObject.isReadyForBath && !bathObject.isSoapUsed)
        {
            // Calculate the new position based on mouse movement
            Vector3 newPosition = GetMouseWorldPosition() + posOffset;

            // Update the object's position
            transform.position = newPosition;

            //On foam bubble particles using raycast
            if (Physics.Raycast(transform.position, Vector3.forward, out hit, 100f))
            {
                if(hit.collider.CompareTag(_Strings.FoamBubble))
                {
                    particleEffectsManager.CheckAndStartFoamBubbleEffect(hit.collider.gameObject.GetComponent<ParticleSystem>());
                }
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
