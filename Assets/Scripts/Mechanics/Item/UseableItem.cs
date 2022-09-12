using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New UseableItem", menuName = "Inventory/Item/Generic UseableItem")]
public class UseableItem : Item, IUseable {

	public UseableItemType useableItemType;

	//public override ItemBonus[] ItemBonuses { get { return usedBonuses; } }
	[SerializeField] private UsedBonus[] usedBonuses;

	public bool Use() { // this will most likely be custom for each useable item unlike ConsumableItems or EquippbleItems where all items have similar if not the same type of effects
		Debug.Log("use");

		return true;
    }

	[System.Serializable]
	public class UsedBonus { // doesn't nessarily have to extend from ItemBonus since this is kinda special
	
	}
}

public enum UseableItemType {
	Gadget,
	Story,
	Event,
	Other
}
