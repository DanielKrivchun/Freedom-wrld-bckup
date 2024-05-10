using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetCareInputManager : MonoBehaviour
{
    public PetAnimation petAnim;
    public PetCareStateManager petCareStateManager;

    private void OnMouseDown()
    {
        switch (petCareStateManager.selectedPetCareState)
        {
            case PetCareState.Happy:

                if(petCareStateManager.happiness < 100)
                {
                    //Play Happy Animation
                    petAnim._ChangeAnimationState(_AnimState.Happy);
                    petCareStateManager.SetSelectedStateDataFiller(2);  
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

    public void ShowPetEatingObject()
    {
            petAnim._ChangeAnimationState(_AnimState.Eating);
            petCareStateManager.SetSelectedStateDataFiller(15); 
    }
}
