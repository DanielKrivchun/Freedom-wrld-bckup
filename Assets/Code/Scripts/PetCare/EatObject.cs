using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatObject : MonoBehaviour
{
    public FoodItems foodName;
    public int foodSpawnIndex;
    public int decreaseHungerValue;

    private void OnMouseDown()
    {
        /*if (PetCareStateManager.instance.petDataRef.petData.hunger < 100)
        {*/
            PetCareInputManager.instance.petAnim._ChangeAnimationState(_AnimState.Eating);
            PetCareStateManager.instance.ManageHungerDataFiller(decreaseHungerValue);

            PetCareStateManager.instance.RemoveFoodFromTable(foodSpawnIndex);
            gameObject.SetActive(false);
        //}
    }
}
