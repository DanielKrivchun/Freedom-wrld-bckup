using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamplePlayerController : MonoBehaviour
{

    public static ExamplePlayerController Instance;

    private void Awake()
    {
        Instance = this;
    }
    public void UseFood(Item item)
    {
        Debug.Log("Eating food");
        Debug.Log($"Name: {item.itemName}, UniqueId: {item.uniqueId}");

        switch(item.itemName) 
        {
            case "Apple":
                PetCareStateManager.instance.GenerateFoodItemOnTable("Apple");
                break;

            case "Mushroom":
                PetCareStateManager.instance.GenerateFoodItemOnTable("Mushroom");
                break;
        }
    }

    public void UseInteractable(Item item)
    {
        Debug.Log("Using Item");
        Debug.Log($"Name: {item.itemName}, UniqueId: {item.uniqueId}");
    }
}
