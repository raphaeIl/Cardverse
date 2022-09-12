using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Item/Experimental/Equipment")]
public class Equipment : EquippableItem, IUseable {

    public EquipmentSlot equipmentSlot; // use to distinguish which part of the body does this equipment belongs to

    public bool Use() {
        return true;
    }
}

public enum EquipmentSlotTemp { Helmet, Chestplate, Leggings, Boots, Weapon, Shield, Accessory}
