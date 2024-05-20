using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    Item item;

    public Button RemoveButton;

    public void RemoveItem()
    {
        InventoryManager.Instance.Remove(item);

        Destroy(gameObject);
    }

    public void AddItem(Item newItem)
    {
        item = newItem;
    }

    public void UseItem() 
    {
        switch (item.itemType)
        {
            case Item.ItemType.Consumable:
                ExamplePlayerController.Instance.UseFood(item);
                break;
            case Item.ItemType.Interactable:
                ExamplePlayerController.Instance.UseInteractable(item);
                break;
        }

        RemoveItem();
    }
}
