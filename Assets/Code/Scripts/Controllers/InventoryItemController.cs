using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    public Item item;

    public Button RemoveButton;

    public void RemoveItem()
    {
        InventoryManager.Instance.Remove(item);
        Debug.Log("Item Removed");
        Destroy(gameObject);
    }

    public void AddItem(Item newItem)
    {
        Debug.Log("Item is added here");
        item = newItem;
    }

    public void UseItem()
    {
        if (item == null)
        {
            Debug.Log("Item is null");
        }

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
