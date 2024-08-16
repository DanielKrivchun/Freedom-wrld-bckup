using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New Item")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public int amount;
    public int value;
    public Sprite icon;
    public List<long> uniqueIds = new List<long>();
    public ItemType itemType;

    public enum ItemType
    {
        Consumable,
        Interactable,
        Usable
    }
}