using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    public Item item;

    public Button RemoveButton;

    public GameObject itemInfoPanelPrefab;
    private GameObject itemInfoPanelInstance;
    private Transform parentTransform;

    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemAmountText;
    public Button useItemButton;

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

    public void OnItemSelected()
    {
        // Find the parent transform in the scene
        parentTransform = GameObject.Find("CanvasMain/UI overlay/Inventory/Inventory Panel/ExtraItemInfoTransform").transform;

        // Traverse all children of the parentTransform to find inactive GameObjects with the desired tag
        foreach (Transform child in parentTransform)
        {
            if (child.CompareTag("ExtraItemInfoPanel"))
            {
                Destroy(child.gameObject);
            }
        }

        // Instantiate a new panel instance
        itemInfoPanelInstance = Instantiate(itemInfoPanelPrefab, parentTransform);
        itemInfoPanelInstance.SetActive(true);

        // Retrieve and set up references to UI elements
        itemIcon = itemInfoPanelInstance.transform.Find("ItemInfoIcon").GetComponent<Image>();
        itemAmountText = itemInfoPanelInstance.transform.Find("ItemInfoAmt").GetComponent<TextMeshProUGUI>();
        itemNameText = itemInfoPanelInstance.transform.Find("ItemInfoName").GetComponent<TextMeshProUGUI>();
        itemDescriptionText = itemInfoPanelInstance.transform.Find("ItemInfoDescription").GetComponent<TextMeshProUGUI>();
        useItemButton = itemInfoPanelInstance.transform.Find("UseItemButton").GetComponent<Button>();

        // Update the UI elements with item data
        itemIcon.sprite = item.icon;
        itemAmountText.text = "Qty: " + item.amount;
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;

        Debug.Log("Opened Item Info Panel");

        // Set up the button to use the item
        useItemButton.onClick.RemoveAllListeners();
        useItemButton.onClick.AddListener(UseItem);
    }

    public void UseItem()
    {
        Destroy(itemInfoPanelInstance);
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
