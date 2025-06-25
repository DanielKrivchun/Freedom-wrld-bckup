using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathObject : MonoBehaviour
{
    public GameObject foamBubblesHolder;

    [Space]
    public PetCareStateManager petStateManager;
    public PetCareUIManager petCareUIManager;

    [Space]
    public int cleanlinessMultiplier;

    [HideInInspector]
    public bool isReadyForBath, isSoapUsed, isShowerUsed;

    #region MANAGE FOAM BUBBLES
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
    #endregion

    #region BATH PREPARATION AND RESET
    //Make player ready for bath
    public void MakePetReadyForBath()
    {
        isReadyForBath = true;
        isSoapUsed = false;
        isShowerUsed = false;
    }

    //Reset pet caretaking state
    public IEnumerator ResetPlayerProperties()
    {
        yield return new WaitForSeconds(1f);
        petCareUIManager.ManagePetCareBtns(false);
        petStateManager.ResetPetCareTakingState();
    }
    #endregion
}
