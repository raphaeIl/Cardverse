using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DepletableStat : Stat {
    
	protected float _currentValue;
	public virtual float CurrentValue { get { return _currentValue; } }

	public float replenishRate; // measured in % per second

	public DepletableStat(float baseValue, StatName statName, StatType statType, BaseValueScalingType baseValueScalingType, float replenishRate) : base(baseValue, statName, statType, baseValueScalingType) {
		_currentValue = BaseValue;

		this.replenishRate = replenishRate;
	}
	
	public DepletableStat(float baseValue, StatName statName, StatType statType, float replenishRate) : this(baseValue, statName, statType, BaseValueScalingType.None, replenishRate) { }

	public void Replenish() {
		AddValue(StatModType.Flat, MaxValue * replenishRate * Time.deltaTime);
    }

	public virtual void AddValue(StatModType statModifier, float value) {
		if (statModifier == StatModType.Flat)
			_currentValue += value;
		else
			_currentValue += (MaxValue * value);

		_currentValue = Mathf.Clamp(_currentValue, 0, MaxValue);
    }

	public virtual void RemoveValue(StatModType statModifier, float value) {
		if (statModifier == StatModType.Flat)
			_currentValue -= value;
		else
			_currentValue -= (MaxValue * value);

		_currentValue = Mathf.Clamp(_currentValue, 0, MaxValue);
    }

	public override string ToString() {
		return $"DepletableStat[StatName={StatName}, StatType={StatType}, BaseValue={BaseValue}, CurrentValue={CurrentValue}]";
    }

}
