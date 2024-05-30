using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetCareInputManager : MonoBehaviour
{
    public static PetCareInputManager instance;

    public PetCareStateManager petCareStateManager;
    public ParticleEffectsManager particleEffectsManager;

    [HideInInspector]
    public PetAnimation petAnim;
    
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
                    particleEffectsManager.PlayHappyEffect();
                    petAnim._ChangeAnimationState(_AnimState.Happy);
                    petCareStateManager.ManageHappinessDataFiller(petCareStateManager.petStatData.happinessTickRate);  
                }
                break;
        }
    }
}
