using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Soap : MonoBehaviour
{
    public BathObject bathObject;
    public ParticleEffectsManager particleEffectsManager;

    private bool isDragging = false;

    RaycastHit hit;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    void OnMouseDown()
    {
        isDragging = true;
        PetCareStateManager.instance.StopIdleTimer();
        bathObject.petCareUIManager.ManagePetCareBtns(false);
    }

    void OnMouseUp()
    {
        isDragging = false;
        transform.DOMove(startPos, 0.5f);

        //Check if all foam bubbles generated
        if (bathObject.isReadyForBath && !bathObject.isSoapUsed && particleEffectsManager.IsAllFoamBubbleaGenerated())
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
            // Update the object's position
            transform.position = GetMouseWorldPosition();

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
        mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
