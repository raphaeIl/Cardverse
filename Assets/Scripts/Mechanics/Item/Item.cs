using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item/Generic Item")]
public class Item : ScriptableObject {

	[field: SerializeField] public long ID { get; private set; }

	public string Name;
	public Sprite DisplayIcon;
	public string Description;

	public ItemType ItemType;
	public Rarity Rarity;

	public int Level;

	public bool isStackable;
	public int Amount;

    public virtual ItemBonus[] ItemBonuses { get; private set; }
	   
	protected virtual void OnEnable() {
		UpdateItemInfo();
    }

    protected virtual void OnValidate() {
		UpdateItemInfo();
    }

	public void UpdateItemInfo() {
		ID = GetInstanceID();

		Amount = isStackable ? Amount : 1;

		if (this is EquippableItem)
			ItemType = ItemType.Equippable;
		else if (this is ConsumableItem)
			ItemType = ItemType.Consumable;
		else if (this is UseableItem)
			ItemType = ItemType.Useable;
		// todo: material check
		else
			ItemType = ItemType.Miscellaneous;
    }

	public virtual Item Clone() { // since the uniqueItems array in Inventory.cs stores references to the original scriptable object, a clone is needed to get an completely new item
		Item clone = Instantiate(this);
		clone.UpdateItemInfo();

		return clone;
    }

    public override bool Equals(object obj) { // check if all item charateristics match, "same" type of item
        return obj is Item item &&
               Name == item.Name &&
               Description == item.Description &&
               ItemType == item.ItemType &&
               Rarity == item.Rarity &&
               Level == item.Level &&
               isStackable == item.isStackable;
    }

	public bool IsDuplicate(object obj) { // checks if the item is a complete duplicate, can be useful for checking for duplication bugs and such
		return obj is Item item && ID == item.ID;
    }
}

[System.Serializable]
public abstract class ItemBonus {

	public StatName StatName;
	public StatModType StatModType;

	[Header("Hover below for more info")]
	[Tooltip("because my code is so trash, please convert any percentage values to their decimal forms. Ex. If u want a bonus for +50% defense, type 0.5 instead of 50 (this only applies for \"Percent Add\" or \"Percent Muti\" bonuses, NOT \"Flat\" ones) (might be reworked later)")]
	public float value;

	public abstract ModifiedStatData ApplyBonus(Character character, Item item);
	public abstract ModifiedStatData RemoveBonus(Character character, Item item);

		public class ModifiedStatData {

		public Stat statModified;
		public float previousValue;
		public float currentValue;

        public ModifiedStatData(Stat statModified, float previousValue, float currentValue) {
            this.statModified = statModified;
            this.previousValue = previousValue;
            this.currentValue = currentValue;
        }
    }
}

public enum ItemType {
	Material,
	Equippable,
	Consumable,
	Useable,
    Miscellaneous
}

public enum Rarity {
	Common,
	Uncommon,
	Rare,
	Epic,
	Legendary,
	Mythic
}

