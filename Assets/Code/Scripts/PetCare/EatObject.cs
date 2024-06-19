using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatObject : MonoBehaviour
{
    public FoodItems foodName;
    public int hungerValue;
    public int happinessValue;
    public int energyValue;
    public int cleanlinessValue;
    public int runningValue;
    public int climbingValue;
    public int flyingValue;
    public int swimmingValue;
    public int IntelligenceValue;
    public int luckValue;

    [Header("Script Ref")]
    private ParticleEffectsManager particleEffectsManager;

    [Header("Pet Data Reference")]
    public PetDataRef petDataRef;

    [HideInInspector]
    public int foodSpawnIndex;

    private void Start()
    {
        particleEffectsManager = FindObjectOfType<ParticleEffectsManager>();
    }

    private void OnMouseDown()
    {
        if (PetCareStateManager.instance.petDataRef.petData.hunger < 100)
        {
            if (foodName == FoodItems.Fairy || foodName == FoodItems.GoldenFairy) {
                //Do particle effect
                particleEffectsManager.PlayFairyEffect();
            }
            PetCareInputManager.instance.petAnim._ChangeAnimationState(_AnimState.Eating);
            PetCareStateManager.instance.ManageHungerDataFiller(hungerValue);
            PetCareStateManager.instance.ManageHappinessDataFiller(happinessValue);
            PetCareStateManager.instance.ManageEnergyDataFiller(energyValue);
            PetCareStateManager.instance.ManageCleanlinessDataFiller(cleanlinessValue);

            petDataRef.petData.running += runningValue;
            petDataRef.petData.climbing += climbingValue;
            petDataRef.petData.flying += flyingValue;
            petDataRef.petData.swimming += swimmingValue;
            petDataRef.petData.intelligence += IntelligenceValue;
            petDataRef.petData.luck += luckValue;

            PetCareStateManager.instance.RemoveFoodFromTable(foodSpawnIndex);
            gameObject.SetActive(false);
            gameObject.transform.SetParent(null);
        }
        else
        {
            PetCareUIManager.instance.ShowNotificationUI("Hunger is full!");
        }
    }
}
