using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodObjectHolder", menuName = "ScriptableObject/FoodObjectHolder", order = 100)]
public class FoodObjectHolder : ScriptableObject    
{
    public List<FoodHolder> PrefabHolderList;

    [Space]
    public List<FoodHolder> generatedFoodObjects;

    private FoodHolder tempFoodHolder;
    private GameObject tempObject;

    public GameObject GetMyFood(FoodItems foodItems)
    {
        /*foreach (FoodHolder foodHolder in generatedFoodObjects)
        {
            if (!foodHolder.foodObject.activeSelf)
            {
                return foodHolder.foodObject;
            }
        }*/

        tempFoodHolder = InstantiateFoodObject(foodItems);
        generatedFoodObjects.Add(tempFoodHolder);
        return tempFoodHolder.foodObject;
    }

    private FoodHolder InstantiateFoodObject(FoodItems foodItems)
    {
        tempObject = Instantiate(PrefabHolderList.Find(asd => asd.foodType == foodItems).foodObject);

        FoodHolder foodHolder = new FoodHolder();
        foodHolder.foodObject = tempObject;
        foodHolder.foodType = foodItems;
        return foodHolder;
    }

    public void ResetFoodObjects()
    {
        generatedFoodObjects.Clear();
    }
}

[System.Serializable]
public class FoodHolder
{
    public GameObject foodObject;
    public FoodItems foodType;
}

public enum FoodItems
{
    None,
    Apple,
    Mushroom
}
