using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanObject : MonoBehaviour
{
    public Transform player;
    public GameObject foamBubblesHolder;

    [Space]
    public PetCareStateManager petCareStateManager;

    [HideInInspector]
    public bool isReadyForBath, isSoapUsed, isShowerUsed;

    private void OnEnable()
    {
        if (foamBubblesHolder != null)
        {
            foamBubblesHolder.SetActive(true);

            for (int i = 0; i < foamBubblesHolder.transform.childCount; i++)
            {
                foamBubblesHolder.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    private void OnDisable()
    {
        if( foamBubblesHolder != null )
        {
            foamBubblesHolder.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        /*if(petCareStateManager.petDataRef.petData.cleanliness < 100)
        {*/
            isReadyForBath = true;
        isSoapUsed = false;
        isShowerUsed = false;
        player.transform.position = new Vector3(1.85f, 0.4f, 2f);
        //}
    }
}
