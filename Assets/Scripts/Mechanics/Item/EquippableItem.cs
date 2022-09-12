using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New EquippableItem", menuName = "Inventory/Item/Generic EquippableItem")]
public class EquippableItem : Item, IEquippable {

	public EquipmentType equipmentType;

	public override ItemBonus[] ItemBonuses { get { return equippedBonuses; } }
	[SerializeField] private EquippedBonus[] equippedBonuses;

    public virtual bool EquipTo(Character character) {
		List<ItemBonus.ModifiedStatData> modifiedStats = new List<ItemBonus.ModifiedStatData>();

		foreach (EquippedBonus equippedBonus in equippedBonuses)
			modifiedStats.Add(equippedBonus.ApplyBonus(character, this));

		if (character.OnStatChange != null)
			character.OnStatChange(modifiedStats.ToArray());

        return true;
	}
	public virtual bool UnequipFrom(Character character) {
		List<ItemBonus.ModifiedStatData> modifiedStats = new List<ItemBonus.ModifiedStatData>();

		foreach (EquippedBonus equippedBonus in equippedBonuses)
			modifiedStats.Add(equippedBonus.RemoveBonus(character, this));

		if (character.OnStatChange != null)
			character.OnStatChange(modifiedStats.ToArray());

		return true;
    }

    public override Item Clone() {
        return base.Clone();
    }
}

[System.Serializable]
public class EquippedBonus : ItemBonus {

    public override ModifiedStatData ApplyBonus(Character character, Item item) {
		Stat targetStat;
		float previousValue;
		float currentValue;
		
		targetStat = character.GetStat<Stat>(StatName);

		Debug.Log($"modifying {(targetStat is DepletableStat ? "DepletableStat" : "Stat")} {StatName} with a StatType of {StatModType}");

		previousValue = targetStat.MaxValue;
		targetStat.AddModifier(new StatModifier(value, StatModType, item));
		currentValue = targetStat.MaxValue;

		return new ModifiedStatData(targetStat, previousValue, currentValue);
    }

    public override ModifiedStatData RemoveBonus(Character character, Item item) {
		Stat targetStat;
		float previousValue;
		float currentValue;
		
		targetStat = character.GetStat<Stat>(StatName);

		previousValue = targetStat.MaxValue;
		if (targetStat.RemoveModifier(targetStat.StatModifiers.Where(sm => sm.Source == item && sm.Type == StatModType && sm.Value == value).FirstOrDefault())) {
			currentValue = targetStat.MaxValue;

			return new ModifiedStatData(targetStat, previousValue, currentValue);
		}


		return null;
    }
}

public enum EquipmentType { Helmet, Chestplate, Leggings, Boots, Weapon, Shield, Accessory}
