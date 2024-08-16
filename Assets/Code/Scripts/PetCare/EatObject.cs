using System;
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
    public int intelligenceValue;
    public int luckValue;
    private DateTime serverTimeNow;

    [Header("Script Ref")]
    private ParticleEffectsManager particleEffectsManager;
    private ExtraPlayerDataManager extraPlayerDataManager;

    [Header("Pet Data Reference")]
    public PetDataRef petDataRef;

    [Header("Pet Care Stat Data Reference")]
    public PetCareStatData petCareStatDatRef;

    [HideInInspector]
    public int foodSpawnIndex;

    private void Start()
    {
        particleEffectsManager = FindObjectOfType<ParticleEffectsManager>();
        extraPlayerDataManager = FindObjectOfType<ExtraPlayerDataManager>();
    }

    private void OnMouseDown()
    {
        //AntiBiotics
        if (foodName == FoodItems.AntiBiotics) //We should not let the user use the item if:petDataRef.petData.isSick != true. But currently the item will just be stuck on the table so no point
        {
            PetCareStateManager.instance.UpdatePetSickToHealthy(); //Make sure that this works
            PetCareInputManager.instance.petAnim._ChangeAnimationState(_AnimState.Eating);

            EatItem();

            return;
        }
        //Vitamins
        else if (foodName == FoodItems.Vitamins)
        {
            extraPlayerDataManager.SetVitaminsAteTime();
            petCareStatDatRef.fluChance = 1;
            PetCareUIManager.instance.ShowNotificationUI($"Vitamins Eaten. Sickness chance reduced for 3 days.");

            EatItem();

            return;
        }
        //Energy Drink
        else if (foodName == FoodItems.EnergyDrink) //Should still be able to drink this if hunger is full. 
        {
            EatItem();

            return;
        }
        //Fairies
        else if (foodName == FoodItems.Fairy || foodName == FoodItems.GoldenFairy)
        { // Fairy effects
            particleEffectsManager.PlayFairyEffect();
            EatItem();

            return;
        }
        else if (foodName == FoodItems.CosmicBerryElectrolyteDrink || foodName == FoodItems.ProteinShake || foodName == FoodItems.MiracleCognitiveSupplements)
        {
            EatItem();

            return;
        }
        //All actual food items
        if (PetCareStateManager.instance.petDataRef.petData.hunger < 100) //If we get down here we are eating an item that affects hunger.
        {
            EatItem();
        }
        else
        {
            PetCareUIManager.instance.ShowNotificationUI("Hunger is full!");
        }
    }


    private void EatItem()
    {
        //Debug.Log($"Used eatItem function: {foodName}");

        if (foodName != FoodItems.Fairy || foodName != FoodItems.GoldenFairy) //Doesnt seem to work, Doesnt recognize fairies
        {
            PetCareInputManager.instance.petAnim._ChangeAnimationState(_AnimState.Eating);
        }

        PetCareStateManager.instance.ManageHungerDataFiller(hungerValue);
        PetCareStateManager.instance.ManageHappinessDataFiller(happinessValue);
        PetCareStateManager.instance.ManageEnergyDataFiller(energyValue);
        PetCareStateManager.instance.ManageCleanlinessDataFiller(cleanlinessValue);

        petDataRef.petData.running += runningValue;
        petDataRef.petData.climbing += climbingValue;
        petDataRef.petData.flying += flyingValue;
        petDataRef.petData.swimming += swimmingValue;
        petDataRef.petData.intelligence += intelligenceValue;
        petDataRef.petData.luck += luckValue;

        PetCareStateManager.instance.RemoveFoodFromTable(foodSpawnIndex);
        gameObject.SetActive(false);
        gameObject.transform.SetParent(null);
    }
}
