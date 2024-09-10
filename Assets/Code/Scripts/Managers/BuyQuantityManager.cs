using UnityEngine;
using TMPro;

public class BuyQuantityManager : MonoBehaviour
{
    public TMP_InputField quantityInputField;  // Reference to the InputField for quantity
    public TMP_Text priceText;  // Reference to the price text in PricePanel
    public int maxQuantity = 9;

    private int originalPrice;  // This will store the base price for 1 item

    // Method to set the base price dynamically from the item clicked in the store
    public void SetBasePrice(int price)
    {
        originalPrice = price;
        UpdatePrice(int.Parse(quantityInputField.text));  // Update price based on current quantity
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize quantity if empty
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
}