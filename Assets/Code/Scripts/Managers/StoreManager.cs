using UnityEngine;
using TMPro;  // For TextMeshPro
using UnityEngine.UI;
using Beamable;
using System.Text.RegularExpressions;  // For images

public class StoreItemButton : MonoBehaviour //This function is attached to the store buttons to add info to the confirm purchase panel
{
    public Item storeItem;

    public int itemPrice;

    // These should be references to the UI elements in the ConfirmPurchasePopup
    public GameObject confirmPurchasePopup;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;
    public Image itemImage;
    public TMP_Text itemPriceText;

    public BuyQuantityManager buyQuantityManager;

    // This method will be called when the item in the store is clicked
    public void OnItemClick()
    {
        string formattedItemName = Regex.Replace(storeItem.itemName, "(?<!^)([A-Z])", " $1");

        // Set the ConfirmPurchasePopup UI fields with the store item data
        itemNameText.text = formattedItemName;
        itemDescriptionText.text = storeItem.description;
        itemPriceText.text = itemPrice.ToString();
        itemImage.sprite = storeItem.icon;

        // Set the base price in the quantity manager
        buyQuantityManager.SetBasePrice(itemPrice);

        // Activate the ConfirmPurchasePopup
        confirmPurchasePopup.SetActive(true);
    }
}