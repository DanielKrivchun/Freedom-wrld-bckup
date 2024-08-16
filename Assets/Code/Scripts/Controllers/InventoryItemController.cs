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
        if (item.amount == 0 )
        {
            Destroy(gameObject);
        }
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
                if (PetCareStateManager.instance.petDataRef.petData.foodData.Count < 3)
                {
                    ExamplePlayerController.Instance.UseItem(item);
                    break;
                }
                else
                {
                    PetCareUIManager.instance.ShowNotificationUI("Can't use food item as there are already 3 food items on table!");
                    return;
                }

            case Item.ItemType.Interactable:
                ExamplePlayerController.Instance.UseInteractable(item);
                break;

            case Item.ItemType.Usable:
                if (PetCareStateManager.instance.petDataRef.petData.isSick)
                {
                    ExamplePlayerController.Instance.UseMedicine(item);
                    break;
                }
                else
                {
                    PetCareUIManager.instance.ShowNotificationUI("Can't use medicine as Pet isn't Sick!");
                    return;
                }
                
        }

        RemoveItem();
    }
}
