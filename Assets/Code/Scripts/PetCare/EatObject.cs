using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatObject : MonoBehaviour
{
    public FoodItems foodName;
    public int hungerValue;

    [HideInInspector]
    public int foodSpawnIndex;

    private void OnMouseDown()
    {
        if (PetCareStateManager.instance.petDataRef.petData.hunger < 100)
        {
            PetCareInputManager.instance.petAnim._ChangeAnimationState(_AnimState.Eating);
            PetCareStateManager.instance.ManageHungerDataFiller(hungerValue);

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
