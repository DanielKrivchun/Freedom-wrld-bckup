using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathObject : MonoBehaviour
{
    public GameObject foamBubblesHolder;

    [Space]
    public PetCareUIManager petCareUIManager;

    [Space]
    public int cleanlinessMultiplier;

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
        if (foamBubblesHolder != null)
        {
            foamBubblesHolder.SetActive(false);
        }
    }

    public void MakePetReadyForBath()
    {
        if (PetCareStateManager.instance.petDataRef.petData.cleanliness < 100)
        {
            isReadyForBath = true;
            isSoapUsed = false;
            isShowerUsed = false;
        }
    }

    public IEnumerator ResetPlayerProperties()
    {
        yield return new WaitForSeconds(1f);
        petCareUIManager.ManagePetCareBtns(false);
        PetCareStateManager.instance.ResetPetCareTakingState();
    }
}
