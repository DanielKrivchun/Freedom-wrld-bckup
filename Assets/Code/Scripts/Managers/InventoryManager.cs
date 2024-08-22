using Beamable;
using Beamable.Player;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Beamable.Common.Docs;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> Items = new List<Item>();

    public Transform ItemContent;
    public InventoryItemController InventoryItem;

    public Toggle EnableRemove;

    public List<InventoryItemController> InventoryItems;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Clear()
    {
        Items.Clear();
    }

    public void Add(Item item)
    {
        // Check if an item with the same name already exists in the list
        Item existingItem = Items.Find(i => i.itemName == item.itemName);

        if (existingItem != null)
        {
            // Increase the amount of the existing item
            existingItem.amount++;
            existingItem.uniqueIds.Add(item.uniqueIds[0]);
        }
        else
        {
            // If the item doesn't exist, add it to the list
            item.amount++;
            Items.Add(item);
        }
    }

    public void Remove(Item item) //Fix this
    {
        item.amount--;
        DeleteOneItem("items." + item.itemName, item.uniqueIds[0]);
        item.uniqueIds.Remove(item.uniqueIds[0]); //CHECK THIS THING

        if (item.amount == 0) 
        {
            Items.Remove(item);
        }

        ListItems();
    }

    public async void DeleteOneItem(string ContentId, long ItemId)
    {
        var ctx = await BeamContext.Default.Instance;
        await ctx.Inventory.Update(builder => builder.DeleteItem(ContentId, ItemId));
        Debug.Log($"Removed: {ContentId}");
    }

    public void ListItems()
    {
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }

        InventoryItems.Clear();

        foreach (var item in Items)
        {
            InventoryItemController obj = Instantiate(InventoryItem, ItemContent);
            //var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();
            var amountText = obj.transform.Find("AmtPanel/ItemAmount").GetComponent<TextMeshProUGUI>();

            //itemName.text = item.itemName;
            itemIcon.sprite = item.icon;
            amountText.text = "x" + item.amount;

            if (EnableRemove.isOn)
            {
                removeButton.gameObject.SetActive(true);
            }

            InventoryItems.Add(obj);

        }

        SetInventoryItems();
    }

    public void EnableItemsRemove()
    {
        if (EnableRemove.isOn)
        {
            foreach (Transform item in ItemContent)
            {
                item.Find("RemoveButton").gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Transform item in ItemContent)
            {
                item.Find("RemoveButton").gameObject.SetActive(false);
            }
        }
    }

    public void SetInventoryItems()
    {
        //InventoryItems = ItemContent.GetComponentsInChildren<InventoryItemController>();

        for (int i = 0; i < InventoryItems.Count; i++)
        {
            InventoryItems[i].AddItem(Items[i]);
        }
    }

}
