using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    public Item[] items;
    private Dictionary<string, Item> itemDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeItemDictionary();
    }

    private void InitializeItemDictionary()
    {
        itemDictionary = new Dictionary<string, Item>();

        foreach (var item in items)
        {
            if (!itemDictionary.ContainsKey(item.itemName))
            {
                itemDictionary.Add(item.itemName, item);
            }
        }
    }

    public Item GetItemByName(string itemName)
    {
        // Remove the prefix "items." from itemName if it exists
        string actualItemName = itemName.Contains(".") ? itemName.Substring(itemName.LastIndexOf('.') + 1) : itemName;

        if (itemDictionary.TryGetValue(actualItemName, out Item item))
        {
            return item;
        }
        return null;
    }
}