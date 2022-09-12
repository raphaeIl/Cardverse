using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New LivingEntity Info", menuName = "Entity/Experimental/Generic LivingEntity")] // using a scriptable object to store every livingentity's info, since it's info it's not the actual gameObject that will be created, all the info are passed into the real one
public class LivingEntityInfo : ScriptableObject {

    public const int Physical_Attack_Stamina_Cost = 10;
    public const int Magic_Attack_Mana_Cost = 2;

    public string Name;
    public Sprite DisplaySprite;
    public ElementType ElementType;
    public List<PossibleStatType> PossibleStats;
    public List<PossibleStatType> PassiveStats;

    protected virtual void OnEnable() {
        OnValidate();
    }

    protected virtual void OnValidate() {
        if (PossibleStats == null)
            PossibleStats = new List<PossibleStatType>();

        if (!PossibleStats.Any(possibleStat => possibleStat.StatName == StatName.HP)) { // every livingentity should have a health stat, this automatically adds it so we don't have to manually add it for every single entity
            PossibleStats.Add(new PossibleStatType(1, StatName.HP, StatType.Additive, BaseValueScalingType.Fast, isDepletable: true, replenishRate: 0));
        }

        if (PassiveStats == null)
            PassiveStats = new List<PossibleStatType>();

        foreach (ElementType elementType in Enum.GetValues(typeof(ElementType))) {
            if (elementType == ElementType.None)
                continue;

            if (!PassiveStats.Any(possibleStat => possibleStat.IsElemental && possibleStat.ElementType == elementType && possibleStat.ElementalStatType == ElementalStatType.Damage))
                PassiveStats.Add(new PossibleStatType(0, StatName.Elemental, StatType.Multiplicative, BaseValueScalingType.None, true, elementType, ElementalStatType.Damage));

            if (!PassiveStats.Any(possibleStat => possibleStat.IsElemental && possibleStat.ElementType == elementType && possibleStat.ElementalStatType == ElementalStatType.Resistance))
                PassiveStats.Add(new PossibleStatType(0, StatName.Elemental, StatType.Multiplicative, BaseValueScalingType.None, true, elementType, ElementalStatType.Resistance));
        }

        if (ElementType != ElementType.None) {

            for (int i = 0; i < PassiveStats.Count; i++) {
                PossibleStatType reset = PassiveStats[i];
                if (reset.StatName == StatName.Elemental) {
                    reset.BaseValue = 0;

                    PassiveStats[i] = reset;
                }
            }

            int element_strength_damage_bonus = PassiveStats.FindIndex(possibleStat => possibleStat.IsElemental && possibleStat.ElementType == ElementalStat.ELEMENT_RELATION_CHART[ElementType] && possibleStat.ElementalStatType == ElementalStatType.Damage);
            int element_strength_resistance_bonus = PassiveStats.FindIndex(possibleStat => possibleStat.IsElemental && possibleStat.ElementType == ElementalStat.ELEMENT_RELATION_CHART[ElementType] && possibleStat.ElementalStatType == ElementalStatType.Resistance);
            
            PossibleStatType matching_element_damage_stat = PassiveStats[element_strength_damage_bonus];
            PossibleStatType matching_element_resistance_stat = PassiveStats[element_strength_resistance_bonus];

            matching_element_damage_stat.BaseValue = ElementalStat.DEFAULT_ELEMENT_BONUS;
            matching_element_resistance_stat.BaseValue = ElementalStat.DEFAULT_ELEMENT_BONUS;

            PassiveStats[element_strength_damage_bonus] = matching_element_damage_stat;
            PassiveStats[element_strength_resistance_bonus] = matching_element_resistance_stat;
        }
    }

}
