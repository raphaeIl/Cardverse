using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryManager : MonoBehaviour {

    #region Singleton

    public static InventoryManager Instance;

    void Awake() {

        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        
    }

    #endregion

    public Inventory Inventory { get { return inventory; } }
    public EquipmentPanel EquipmentPanel { get { return equipmentPanel;  } }

    [SerializeField] private Inventory inventory;
    [SerializeField] private EquipmentPanel equipmentPanel;

    public void AddItem(Item item, int amount = 1) { // add item to inventory
        if (inventory.AddItem(item, amount))
            print($"Succesfully added {item.Name} to your inventory!");
        else
            throw new InvalidOperationException($"Unable to add {item.Name} to the inventory");
    }

    public void RemoveItem(Item item, int amount = 1) { // remove item from inventory
        if (inventory.RemoveItem(item, amount))
            print($"Succesfully removed {item.Name} from your inventory!");
        else
            throw new InvalidOperationException($"Unable to remove {item.Name} from your inventory");
    }

    public void EquipItem(EquippableItem equippableItem) { // equip item to the currently selected player's EquipmentInventory

        if (inventory.RemoveItem(equippableItem)) {
            EquippableItem previousItem;
            if (equipmentPanel.AddEquipment(equippableItem, out previousItem)) {
                if (previousItem != null)
                    inventory.AddItem(previousItem); // inventory check ??
            } else {
                inventory.AddItem(equippableItem);
                throw new InvalidOperationException($"Unable to equip {equippableItem.Name} to the target player");
            }
        } else 
            throw new InvalidOperationException($"Unable to remove {equippableItem.Name} from your inventory");
    }

    public void UnequipItem(EquippableItem equippableItem) {// unequip item from the currently selected player's EquipmentInventory

        if (!inventory.IsFull)
            if (equipmentPanel.RemoveEquipment(equippableItem)) {
                inventory.AddItem(equippableItem);
            } else
                throw new InvalidOperationException($"Unable to remove {equippableItem.Name} from your inventory");
        else
            throw new InvalidOperationException($"Can not Unequip {equippableItem.Name}, your inventory is full");
    }
}