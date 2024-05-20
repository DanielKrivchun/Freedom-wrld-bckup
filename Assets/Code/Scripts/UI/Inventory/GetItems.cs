using Beamable;
using Beamable.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItems : MonoBehaviour
{

    public Item Item;
    public InventoryManager inventoryManager;
    // Start is called before the first frame update
    void Start()
    {
        GetInventory();
    }

    public async void GetInventory()
    {

        // acquire a context
        var ctx = await BeamContext.Default.Instance;

        // GetItems() allows a ItemRef to specify which type of items to get
        var playerItems = ctx.Inventory.GetItems();

        // wait for the items to be updated
        await playerItems.Refresh();

        // Clear the inventory before fetching new items so we dont duplicate them
        //InventoryManager.Instance.Clear();

        foreach (var playerItem in playerItems)
        {
            // Convert PlayerItem to Item before adding
            Item item = ConvertToItem(playerItem);
            //Debug.Log($"Adding item: {playerItem.ContentId}");
            if (item != null)
            {
                item.uniqueId = (int)playerItem.ItemId;
                InventoryManager.Instance.Add(item);
                //Debug.Log($"item id=[{playerItem.ItemId}] type=[{playerItem.ContentId}]");
                //Debug.Log($"Item: {playerItem}");
            }
        }
    }

    private Item ConvertToItem(PlayerItem playerItem)
    {
        Item newItem = ItemDatabase.Instance.GetItemByName(playerItem.ContentId);
        if (newItem != null)
        {
            // Create a new instance of the Item object to ensure uniqueness
            newItem = Instantiate(newItem);

            // Set additional properties if needed, but they should already be set in the ScriptableObject
            return newItem;
        }

        return null;
    }
}
