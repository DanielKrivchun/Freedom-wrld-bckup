using UnityEngine;
using TMPro;
using System;
using System.Threading.Tasks;

public class BuyQuantityManager : MonoBehaviour
{
    public TMP_InputField quantityInputField;  // Reference to the InputField for quantity
    public TMP_Text priceText;  // Reference to the price text in PricePanel
    public int maxQuantity = 9;

    private int originalPrice;  // This will store the base price for 1 item

    [Space]
    public GetServerTime getServerTime;

    [SerializeField]
    private ExtraPlayerDataListSO extraPlayerDataListSO;

    [Header("Script Ref")]
    private ExtraPlayerDataManager extraPlayerDataManager;

    // Method to set the base price dynamically from the item clicked in the store
    public void SetBasePrice(int price)
    {
        originalPrice = price;
        UpdatePrice(int.Parse(quantityInputField.text));  // Update price based on current quantity
    }

    // Start is called before the first frame update
    private void Start()
    {
        extraPlayerDataManager = FindObjectOfType<ExtraPlayerDataManager>();

        if (string.IsNullOrEmpty(quantityInputField.text))
        {
            quantityInputField.text = "1";
        }
    }

    // Call this when the + button is pressed
    public void IncreaseQuantity()
    {
        int currentQuantity = int.Parse(quantityInputField.text);
        if (currentQuantity < maxQuantity)
        {
            currentQuantity++;
            quantityInputField.text = currentQuantity.ToString();
            UpdatePrice(currentQuantity);
        }
    }

    // Call this when the - button is pressed
    public void DecreaseQuantity()
    {
        int currentQuantity = int.Parse(quantityInputField.text);
        if (currentQuantity > 1)
        {
            currentQuantity--;
            quantityInputField.text = currentQuantity.ToString();
            UpdatePrice(currentQuantity);
        }
    }

    // Update the price based on the current quantity
    private void UpdatePrice(int quantity)
    {
        // Multiply the stored original price by the current quantity
        int totalPrice = originalPrice * quantity;
        priceText.text = totalPrice.ToString();  // Update the price display
    }

    public async Task<bool> IsItemPurchasable(string itemName, int quantity)
    {
        DateTime currentTime = await getServerTime.GetCurrentTimeTask();

        if (itemName == "CosmicBerryElectrolyteDrink")
        {
            if (DateTime.TryParse(extraPlayerDataListSO.extraPlayerData.cosmicBerryElectrolyteBoughtTime, out DateTime cosmicBerryDrinkTime))
            {
                TimeSpan timeSinceAte = cosmicBerryDrinkTime - currentTime;

                if (timeSinceAte.Days >= 7)
                {
                    extraPlayerDataManager.SetCosmicBerryDrinkBoughtTime();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (extraPlayerDataListSO.extraPlayerData.cosmicBerryElectrolyteBoughtTime == "")
            {
                extraPlayerDataManager.SetCosmicBerryDrinkBoughtTime();
                return true;
            }
        }
        else if (itemName == "MiracleCognitiveSupplements")
        {
            if (DateTime.TryParse(extraPlayerDataListSO.extraPlayerData.miracleCognitiveBoughtTime, out DateTime miracleCognitiveBoughtTime))
            {
                TimeSpan timeSinceAte = miracleCognitiveBoughtTime - currentTime;

                if (timeSinceAte.Days >= 1)
                {
                    extraPlayerDataManager.SetMiracleCognitiveBoughtTime();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (extraPlayerDataListSO.extraPlayerData.miracleCognitiveBoughtTime == "")
            {
                extraPlayerDataManager.SetMiracleCognitiveBoughtTime();
                return true;
            }
        }
        else if (itemName == "ProteinShake")
        {
            if (DateTime.TryParse(extraPlayerDataListSO.extraPlayerData.proteinShakeBoughtTime, out DateTime proteinShakeBoughtTime))
            {
                TimeSpan timeSinceAte = proteinShakeBoughtTime - currentTime;

                if (timeSinceAte.Days >= 1)
                {
                    extraPlayerDataManager.SetProteinShakeBoughtTime();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (extraPlayerDataListSO.extraPlayerData.proteinShakeBoughtTime == "")
            {
                extraPlayerDataManager.SetProteinShakeBoughtTime();
                return true;
            }
        }

        return true;
    }
}