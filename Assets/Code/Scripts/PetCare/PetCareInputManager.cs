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
                //Play Happy Animation
                petAnim._ChangeAnimationState(_AnimState.Happy);
                petCareStateManager.SetSelectedStateFillerImage(0.05f);
                break;

            case PetCareState.Clean:
                //Play Clean Animation
                if (petCareStateManager.isReadyForToilet)
                {
                    petAnim._ChangeAnimationState(_AnimState.Toilet);
                    petCareStateManager.SetSelectedStateFillerImage(0.1f);
                }
                break;
        }
    }

    public void ShowPetEatingObject()
    {
        petAnim._ChangeAnimationState(_AnimState.Eating);
        petCareStateManager.SetSelectedStateFillerImage(0.2f);
    }
}
