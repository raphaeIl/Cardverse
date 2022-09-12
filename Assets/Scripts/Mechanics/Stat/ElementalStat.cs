using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalStat : Stat {

    /// <summary>
    /// Key: Strength, Value: Weakness // (strength)element is strong against (weakness)element
    /// </summary>                                                                                                                        [key] is strong against [value]
    public static readonly Dictionary<ElementType, ElementType> ELEMENT_RELATION_CHART = new Dictionary<ElementType, ElementType>() { { ElementType.Water, ElementType.Fire },
                                                                                                                                      { ElementType.Fire, ElementType.Ice },
                                                                                                                                      { ElementType.Ice, ElementType.Earth },
                                                                                                                                      { ElementType.Earth, ElementType.Lightning },
                                                                                                                                      { ElementType.Lightning, ElementType.Water }, };

    public static readonly ElementalStat DEFAULT = new ElementalStat(0, StatName.Elemental, StatType.Multiplicative, BaseValueScalingType.None, ElementType.None, ElementalStatType.Unknown);
    public const float DEFAULT_ELEMENT_BONUS = 0.1f; // 10%

    public ElementType ElementType;
    public ElementalStatType ElementalStatType;

    public ElementalStat(float baseValue, StatName statName, StatType statType, BaseValueScalingType baseValueScalingType, ElementType elementType, ElementalStatType elementalStatType) : base(baseValue, statName, statType, baseValueScalingType) {
        ElementType = elementType;
        ElementalStatType = elementalStatType;
    }

    public override string ToString() {
        return $"ElementalStat[StatName={StatName}, StatType={StatType}, BaseValue={BaseValue}, ElementType={ElementType}, ElementalStatType={ElementalStatType}]";
    }

}

public enum ElementalStatType {
    Unknown, // entities that don't have an element type shouldn't have this attribute
    Damage, // boosts damage towards a specific element
    Resistance // boosts resistance towards a specific element
}
