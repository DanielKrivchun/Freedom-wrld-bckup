using Beamable;
using Beamable.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItems : MonoBehaviour
{

    public Item Item;
    public InventoryManager Instance;
    // Start is called before the first frame update
    void Start()
    {
        //GetInventory();
    }

    public async void GetInventory()
    {

        var ctx = await BeamContext.Default.Instance;

        var playerItems = ctx.Inventory.GetItems();

        await playerItems.Refresh();
        Debug.Log($"Player items:{playerItems.Count}");

        // Clear the inventory before fetching new items so we dont duplicate them
        InventoryManager.Instance.Clear();

        foreach (var playerItem in playerItems)
        {
            // Convert PlayerItem to Item before adding
            Item item = ConvertToItem(playerItem);
            if (item != null)
            {
                item.uniqueId = (int)playerItem.ItemId;
                InventoryManager.Instance.Add(item);
                //Debug.Log($"item id=[{playerItem.ItemId}] type=[{playerItem.ContentId}]");
            }
        }

        InventoryManager.Instance.ListItems();
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
