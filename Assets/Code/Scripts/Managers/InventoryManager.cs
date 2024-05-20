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
    public GameObject InventoryItem;

    public Toggle EnableRemove;

    public InventoryItemController[] InventoryItems;

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
        Items.Add(item);
    }

    public void Remove(Item item)
    {
        DeleteOneItem("items."+item.itemName, item.uniqueId);
        Items.Remove(item);
    }

    public async void DeleteOneItem(string ContentId,long ItemId)
    {
        var ctx = await BeamContext.Default.Instance;
        await ctx.Inventory.Update(builder => builder.DeleteItem(ContentId, ItemId));
        Debug.Log($"Removed: {ContentId}");
    }

    public void ListItems() 
    {   foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in Items)
        {
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();

            itemName.text = item.itemName;
            itemIcon.sprite = item.icon;

            if (EnableRemove.isOn)
            {
                removeButton.gameObject.SetActive(true);
            }
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
        InventoryItems = ItemContent.GetComponentsInChildren<InventoryItemController>();

        for (int i = 0; i < Items.Count; i++)
        {
            InventoryItems[i].AddItem(Items[i]);
        }
    }

}
