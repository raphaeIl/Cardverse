using System;

public class EquipmentInventory { // unlike a normal Inventory, every character has a EquipmentInventory for their equipments unlike just a single Inventory for all

    public static int MAX_EQUIPMENT_INVENTORY_SIZE { get { return Enum.GetNames(typeof(EquipmentType)).Length; } }

    public event Action OnEquipmentChange;

    public EquippableItem[] CurrentEquipment { get { return currentEquipment; } }
    private EquippableItem[] currentEquipment;

    public EquipmentInventory() {
        currentEquipment = new EquippableItem[MAX_EQUIPMENT_INVENTORY_SIZE];
    }

    public bool AddEquipment(EquippableItem equippableItem, out EquippableItem previousItem) {
        previousItem = currentEquipment[(int)equippableItem.equipmentType];
        currentEquipment[(int)equippableItem.equipmentType] = equippableItem;

        if (OnEquipmentChange != null)
            OnEquipmentChange();

        return true;
    }

    public bool RemoveEquipment(EquippableItem equippableItem) {

        if (currentEquipment[(int)equippableItem.equipmentType] == equippableItem) {
            currentEquipment[(int)equippableItem.equipmentType] = null;

            if (OnEquipmentChange != null)
                OnEquipmentChange();

            return true;
        }
        return false;
    }
    
}
