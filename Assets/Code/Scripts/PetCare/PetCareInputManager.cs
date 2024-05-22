using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetCareInputManager : MonoBehaviour
{
    public static PetCareInputManager instance;

    public PetAnimation petAnim;
    public PetCareStateManager petCareStateManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnMouseDown()
    {
        switch (petCareStateManager.selectedPetCareState)
        {
            case PetCareState.Happy:

                if(petCareStateManager.petDataRef.petData.happiness < 100)
                {
                    //Play Happy Animation
                    petAnim._ChangeAnimationState(_AnimState.Happy);
                    petCareStateManager.ManageHappinessDataFiller(petCareStateManager.happinessTickRate);  
                }
                break;

                /*case PetCareState.Clean:
                    //Play Clean Animation
                    if (petCareStateManager.isReadyForToilet)
                    {
                        petAnim._ChangeAnimationState(_AnimState.Toilet);
                        petCareStateManager.SetSelectedStateFillerImage(0.1f);
                    }
                    break;*/
        }
    }
}
