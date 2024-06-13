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

    //Give me requested food item GameObject
    public GameObject GetMyFood(FoodItems foodItems)
    {
        tempFoodHolder = InstantiateFoodObject(foodItems);
        generatedFoodObjects.Add(tempFoodHolder);
        return tempFoodHolder.foodObject;
    }

    //Instantiates food item GameObject
    private FoodHolder InstantiateFoodObject(FoodItems foodItems)
    {
        tempObject = Instantiate(PrefabHolderList.Find(asd => asd.foodType == foodItems).foodObject);

        FoodHolder foodHolder = new FoodHolder();
        foodHolder.foodObject = tempObject;
        foodHolder.foodType = foodItems;
        return foodHolder;
    }

    //Reset
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
