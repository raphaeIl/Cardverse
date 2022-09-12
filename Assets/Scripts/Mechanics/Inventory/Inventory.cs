using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public class Inventory : MonoBehaviour { // further features includes serialization, too lazy to implement that now => https://www.youtube.com/watch?v=19ZtPMW-e-I&list=PLm7W8dbdflojT-OqfBJvqK6L9LRwKmymz&index=29&ab_channel=Kryzarel

    public const int MAX_INVENTORY_SIZE = 1000;

    [SerializeField] private Item[] AllUniqueItems;

    public List<Item> Items { get { return items; } }
    public List<ItemSlot> ItemSlots { get { return itemSlots; } }

    [SerializeField] private List<Item> items;
    [SerializeField] private List<ItemSlot> itemSlots;

    [SerializeField] private Transform itemsParent;
    [SerializeField] private GameObject itemSlotPrefab;

    public event Action OnInventoryChange;

    public int ItemsCount { get { return items.Sum(item => item.Amount); } } // includes stacked items
    public bool IsFull { get { return ItemsCount >= MAX_INVENTORY_SIZE; } }

    private InventorySortOrder inventorySortOrder;

    void Awake() {
        OnInventoryChange += UpdateInventoryDisplay;

        if (items == null)
            items = new List<Item>();

        itemSlots = GetComponentsInChildren<ItemSlot>().ToList();

        for (int i = 0; i < items.Count; i++)
            CreateItemSlot(items[i]);

    }

    void OnValidate() {
        UpdateInventoryDisplay();
    }

    void Update() {

        if (Input.GetKey(KeyCode.E)) // test code 
            ItemWorldSpawner.Instance.SpawnItemWorld(AllUniqueItems[UnityEngine.Random.Range(0, AllUniqueItems.Length)].Clone(), PartyManager.Instance.PartyController.WorldPosition);

    }

    public bool AddItem(Item item, int amount = 1) {
        if (IsFull)
            return false;

        Item itemMatch = null;
        if (item.isStackable && this.ContainsItem(item, out itemMatch)) { // if the item is stackable, instead of adding it to the inventory again, adds the amount to the already existing one (if it exists)
            itemMatch.Amount += amount;
        } else {
            CreateItemSlot(item);
            items.Add(item);
        }

        if (OnInventoryChange != null)
            OnInventoryChange();

        return true;
    }

    public bool RemoveItem(Item item, int amount = 1) {
        
        Item itemMatch = null;
        if (item.isStackable && this.ContainsItem(item, out itemMatch) && itemMatch.Amount - amount > 1) { // remove works similar to add
            itemMatch.Amount -= amount; // should - item.Amount
        } else {
            if (items.Remove(item)) {
                DestroyItemSlot(item);
            }
        }

        if (OnInventoryChange != null)
            OnInventoryChange();

        return true;
    }

    public bool RemoveItem(long id, int amount = 1) { // remove by id, exception thrown if dupe found
        return RemoveItem(items.Where(item => item.ID == id).Single(), amount);
    }

    public void Sort(InventorySortType sortType, InventorySortOrder sortOrder = InventorySortOrder.Ascending) { // all ordered corresponding to the enum order
        Comparison<Item> comparison1 = (item1, item2) => (sortOrder == InventorySortOrder.Ascending ? item1 : item2).Name.CompareTo((sortOrder == InventorySortOrder.Ascending ? item2 : item1).Name); // primary comparison (order by name)
        Comparison<Item> comparison2 = (item1, item2) => ((int)item1.Rarity - (int)item2.Rarity) * (sortOrder == InventorySortOrder.Ascending ? 1 : -1);                                               // secondary comparison (order by rarity)
        Comparison<Item> comparison3 = (item1, item2) => (item1.Level - item2.Level) * (sortOrder == InventorySortOrder.Ascending ? 1 : -1);                                                           // tertiary comparison (order by level)
        Comparison<Item> comparison4 = (item1, item2) => ((int)item1.ItemType - (int)item2.ItemType) * (sortOrder == InventorySortOrder.Ascending ? 1 : -1);                                           // forth comparison (order by itemType)

        switch (sortType) {
            case InventorySortType.Name:
                items.Sort(comparison1);
                break;

            case InventorySortType.Rarity: // secondary comparison needed since different items can have the same rarity
                items = items.OrderBy(item => item, Comparer<Item>.Create(comparison2)).ThenBy(item => item, Comparer<Item>.Create(comparison1)).ToList();
                break;

            case InventorySortType.Level:
                items = items.OrderBy(item => item, Comparer<Item>.Create(comparison3)).ThenBy(item => item, Comparer<Item>.Create(comparison1)).ToList();
                break;

            case InventorySortType.ItemType:
                items = items.OrderBy(item => item, Comparer<Item>.Create(comparison4)).ThenBy(item => item, Comparer<Item>.Create(comparison1)).ToList();
                break;

            default:
                break;
        }

        UpdateInventoryDisplay();
    }

    public bool ContainsItem(Item item, out Item itemMatch) {
        foreach (Item inventoryItem in items)
            if (inventoryItem.Equals(item)) {
                itemMatch = inventoryItem;
                return true;
            }

        itemMatch = null;
        return false;
    }

    public void UpdateInventoryDisplay() {
        for (int i = 0; i < itemSlots.Count; i++) {
            if (i < items.Count) {
                itemSlots[i].SetItem(items[i]);
            } else itemSlots[i].SetItem(null);
        }
    }

    private void CreateItemSlot(Item item) {
        ItemSlot newItemSlot = Instantiate(itemSlotPrefab, itemsParent).GetComponent<ItemSlot>();
        newItemSlot.SetItem(item);

        itemSlots.Add(newItemSlot);
    }

    private void DestroyItemSlot(Item item) {
        ItemSlot itemSlotToDestroy = itemSlots.Where(slot => slot.CurrentItem == item).First();
        itemSlots.Remove(itemSlotToDestroy);

        Destroy(itemSlotToDestroy.gameObject);
    }

    public void OnSortOption(Dropdown optionSelected) {
        Sort((InventorySortType)Enum.ToObject(typeof(InventorySortType) , optionSelected.value), inventorySortOrder);
    }

    public void OnSortType(Button toggleType) {
        inventorySortOrder = (inventorySortOrder == InventorySortOrder.Ascending) ? InventorySortOrder.Descending : InventorySortOrder.Ascending;
        toggleType.transform.GetComponent<RectTransform>().eulerAngles = new Vector3((inventorySortOrder == InventorySortOrder.Ascending ? 180 : 0) , 0, 0);

        OnSortOption(toggleType.transform.parent.GetComponentInChildren<Dropdown>());
    }
}

public enum InventorySortType {
    None,
    Name,
    Rarity,
    Level,
    ItemType
}

public enum InventorySortOrder {
    Ascending,
    Descending
}
