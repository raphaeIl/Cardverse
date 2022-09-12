using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "New ConsumableItem", menuName = "Inventory/Item/Generic ConsumableItem")]
public class ConsumableItem : Item, IConsumable {

	public ConsumableItemType consumableItemType; // only depletable stats can have temp bonus types, while "Permanent" are not actually permanent

    public override ItemBonus[] ItemBonuses { get { return consumedBonuses; } }
	[SerializeField] private ConsumedBonus[] consumedBonuses;

    protected override void OnValidate() {
        base.OnValidate();

		foreach (ConsumedBonus consumedBonus in consumedBonuses)
			consumedBonus._isTemporary = consumedBonus.ConsumedBonusType == ConsumedBonusType.Temporary;
    }

    public virtual bool Consume(Character character) {
		List<ItemBonus.ModifiedStatData> modifiedStats = new List<ItemBonus.ModifiedStatData>();

		foreach (ConsumedBonus consumedBonus in consumedBonuses) {
			ItemBonus.ModifiedStatData modifiedStat = consumedBonus.ApplyBonus(character, this);

			if (modifiedStat != null)
				modifiedStats.Add(modifiedStat);
        }
		
		if (character.OnStatChange != null)
			character.OnStatChange(modifiedStats.ToArray());

		InventoryManager.Instance.RemoveItem(this);
		return true;
	}

}

[System.Serializable]
public class ConsumedBonus : ItemBonus {

	public ConsumedBonusType ConsumedBonusType;

	[ConditionalField("_isTemporary")] // Only applicable for Buffs
	public float Duration;

	[HideInInspector] public bool _isTemporary;

    public override ModifiedStatData ApplyBonus(Character character, Item item) {
		Stat targetStat;
		float previousValue;
		float currentValue;

		targetStat = character.GetStat<Stat>(StatName);

		Debug.Log($"modifying {(targetStat is DepletableStat ? "DepletableStat" : "Stat")} {StatName} with a StatType of {StatModType}");

		if (ConsumedBonusType == ConsumedBonusType.Instant) {
			if (!(targetStat is DepletableStat))
				throw new System.InvalidOperationException($"Can not use a Restoration Bonus on an non-depletable stat, please change the stat type of {ItemUtils.StatDisplayNames[(int)StatName]} for {item.Name}");

			((DepletableStat)targetStat).AddValue(StatModType, value);

			if (StatName == StatName.HP)
				if (character.OnHealthChange != null)
					character.OnHealthChange();

			return null;
		} 

		// buff		
		previousValue = targetStat.MaxValue;
		targetStat.AddModifier(new StatModifier(value, StatModType, item));
		currentValue = targetStat.MaxValue;

		character.StartCoroutine(RemoveBonus(character, item, Duration)); // could be somewhere else

		return new ModifiedStatData(targetStat, previousValue, currentValue);
    }

    public override ModifiedStatData RemoveBonus(Character character, Item item) { // doesn't seem valid for a consumable item but for those that have a duration, this will be used when the effect wears off
		if (ConsumedBonusType == ConsumedBonusType.Instant) // assuming all buffs are temporary
			return null;
		
		Stat targetStat;
		float previousValue;
		float currentValue;
		
		targetStat = character.GetStat<Stat>(StatName);

		previousValue = targetStat.MaxValue;
		if (targetStat.RemoveModifier(targetStat.StatModifiers.Where(sm => sm.Source == item && sm.Type == StatModType && sm.Value == value).FirstOrDefault())) {
			currentValue = targetStat.MaxValue;

			ModifiedStatData statModifiedInfo = new ModifiedStatData(targetStat, previousValue, currentValue);

			if (character.OnStatChange != null)
				character.OnStatChange(new ModifiedStatData[] { statModifiedInfo }.ToArray());

			return statModifiedInfo;
		}

		return null;
    }

	private IEnumerator RemoveBonus(Character character, Item item, float duration) {
		yield return new WaitForSeconds(duration);
		RemoveBonus(character, item);
    }

}

public enum ConsumableItemType {
	Potion,
	Food,
	Special
}

public enum ConsumedBonusType {
	Instant,
	Temporary,
  //Permanent ?? maybe some special item will give permanent buffs?
}
