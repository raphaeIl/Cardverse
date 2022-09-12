using MyBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[Serializable] // inspired by https://assetstore.unity.com/packages/tools/integration/character-stats-106351#content (modified)
public class Stat {

	public StatName StatName;
	public StatType StatType;

	public int level;

	public BaseValueScalingType BaseValueScalingType;

	public readonly float defaultBaseValue;
	public float BaseValue { get { return BaseValueScalingType == BaseValueScalingType.None ? defaultBaseValue : 
															StatType == StatType.Multiplicative ? defaultBaseValue + baseValueScaling[(int)BaseValueScalingType].Evaluate(level) / 100.0f :
																							  defaultBaseValue + baseValueScaling[(int)BaseValueScalingType].Evaluate(level); } }

	// calculating the base value for a certain level (simple mutiplier * linear function, pokemon uses a similar one) // http://howtomakeanrpg.com/a/how-to-make-an-rpg-levels.html
	private readonly FunctionEvaluator[] baseValueScaling = { null, new FunctionEvaluator((x) => (x - 1)), new FunctionEvaluator((x) => 20 * (x - 1)), new FunctionEvaluator((x) => 50 * (x - 1)) };
	
	public string DisplayName { get { return ItemUtils.StatDisplayNames[(int)StatName]; } }

	protected bool isDirty = true;
	protected float lastBaseValue;

	protected float _maxValue;
	public virtual float MaxValue { // max value of this stat (not that it matters since a normal stat can not be changed)
		get {
			UpdateStatValue();
			return _maxValue;
		}
	}
	
	protected readonly List<StatModifier> statModifiers;
	public readonly ReadOnlyCollection<StatModifier> StatModifiers; // used to expose every current stat modifier either for the user to see or for displaying (unmodifiable, this is a reference to the original private list)

	public Stat(float baseValue, StatName statName, StatType statType, BaseValueScalingType baseValueScalingType) {
		statModifiers = new List<StatModifier>();
		StatModifiers = statModifiers.AsReadOnly();
		
		defaultBaseValue = baseValue;
		StatName = statName;
		StatType = statType;
		BaseValueScalingType = baseValueScalingType;

		level = 1;
		_maxValue = BaseValue;
    }

	public Stat(float baseValue, StatName statName, StatType statType) : this(baseValue, statName, statType, BaseValueScalingType.None) { }

	public void SetBaseLevel(int previous, int Currentlevel) {
		level = Currentlevel;
    }

	public virtual void AddModifier(StatModifier mod) {
		isDirty = true;
		statModifiers.Add(mod);

		UpdateStatValue();
	}

	public virtual bool RemoveModifier(StatModifier mod) {
		if (statModifiers.Remove(mod)) {
			isDirty = true;
			UpdateStatValue();
			return true;
		}
		return false;
	}

	public virtual bool RemoveAllModifiersFromSource(object source) {
		int numRemovals = statModifiers.RemoveAll(mod => mod.Source == source);

		if (numRemovals > 0) {
			isDirty = true;
			UpdateStatValue();
			return true;
		}
		return false;
	}

	public void UpdateStatValue() {
		if (isDirty || lastBaseValue != BaseValue) {
				lastBaseValue = BaseValue;
				_maxValue = CalculateFinalValue();
				isDirty = false;
		}
    }

	protected virtual int CompareModifierOrder(StatModifier a, StatModifier b) {
		if (a.Order < b.Order)
			return -1;
		else if (a.Order > b.Order)
			return 1;
		return 0; //if (a.Order == b.Order)
	}
		
	protected virtual float CalculateFinalValue() {
		float finalValue = BaseValue;
		float sumPercentAdd = 0;

		statModifiers.Sort(CompareModifierOrder);

		for (int i = 0; i < statModifiers.Count; i++) {
			StatModifier mod = statModifiers[i];

			if (mod.Type == StatModType.Flat) {
				finalValue += mod.Value;
			}
			else if (mod.Type == StatModType.PercentAdd) {
				sumPercentAdd += mod.Value;

				if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.PercentAdd) {
					finalValue *= 1 + sumPercentAdd;
					sumPercentAdd = 0;
				}
			}
			else if (mod.Type == StatModType.PercentMult) {
				finalValue *= 1 + mod.Value;
			}
		}

		// Workaround for float calculation errors, like displaying 12.00001 instead of 12
		return (float)Math.Round(finalValue, 4);
	}

    public override string ToString() {
		return $"Stat[StatName={StatName}, StatType={StatType}, BaseValue={BaseValue}]";
    }

}

public enum StatName {
	HP,
	Attack,
	Magic_Attack, 
	Defense,
	Magic_Defense,
	Mana,
	Stamina,
	Critical_Damage,
	Critical_Rate,
	Resistance,
	Speed,
	Luck,
	Elemental
}

public enum StatType {
	Additive, // "Flat" buffs
	Multiplicative // Percentage buffs
}

public enum BaseValueScalingType {
	None,
	Slow,
	Normal,
	Fast
}

[Serializable]
public struct PossibleStatType {

	public float BaseValue;
	public StatName StatName;
	public StatType StatType;
	public BaseValueScalingType BaseValueScalingType;

	[Tooltip("Can this Stat change? Ex: Health can drop from a max value to 0 while Attack has to be fixed at a value")]
	public bool IsDepletable;

	[ConditionalField("IsDepletable")]
	[Range(0, 1)] public float ReplenishRate;

	[Tooltip("Is this stat an elemental one? Ex: Fire Damage Bonus / Water Res Bonus")]
	public bool IsElemental;

	[ConditionalField("IsElemental")] public ElementType ElementType;
	[ConditionalField("IsElemental")] public ElementalStatType ElementalStatType;

    public PossibleStatType(float baseValue, StatName statName, StatType statType, BaseValueScalingType baseValueScalingType, bool isDepletable, float replenishRate, bool isElemental, ElementType elementType, ElementalStatType elementalStatType) {
		BaseValue = baseValue;
        StatName = statName;
        StatType = statType;
		BaseValueScalingType = baseValueScalingType;
		IsDepletable = isDepletable;
		ReplenishRate = replenishRate;
		IsElemental = isElemental;
		ElementType = elementType;
		ElementalStatType = elementalStatType;
	}

	public PossibleStatType(float baseValue, StatName statName, StatType statType, BaseValueScalingType baseValueScalingType, bool isDepletable, float replenishRate = 0) : this(baseValue, statName, statType, baseValueScalingType, isDepletable, replenishRate, false, ElementType.None, ElementalStatType.Unknown) { }

    public PossibleStatType(float baseValue, StatName statName, StatType statType, BaseValueScalingType baseValueScalingType, bool isElemental, ElementType elementType = ElementType.None, ElementalStatType elementalStatType = ElementalStatType.Unknown) : this(baseValue, statName, statType, baseValueScalingType, false, 0, isElemental, elementType, elementalStatType) { }
    
}
