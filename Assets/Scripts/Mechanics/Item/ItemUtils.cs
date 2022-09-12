using System;
using System.Linq;
using UnityEngine;

public static class ItemUtils {

    public static string[] ItemTypeDisplayNames { get { return ConvertTypes(Enum.GetNames(typeof(ItemType))); } }
    public static string[] EquipmentTypeDisplayNames { get { return ConvertTypes(Enum.GetNames(typeof(EquipmentType))); } }
    public static string[] ConsumeableTypeDisplayNames { get { return ConvertTypes(Enum.GetNames(typeof(ConsumableItemType))); } }

    public static string[] RarityDisplayNames { get { return ConvertTypes(Enum.GetNames(typeof(Rarity))); } }
    public static Color[] RarityDisplayColors { get { return new Color[] { Color.white, Utils.ConvertColor(85, 255, 85), Utils.ConvertColor(85, 85, 255), Utils.ConvertColor(170, 0, 170), Utils.ConvertColor(255, 170, 0), Utils.ConvertColor(255, 85, 255) }; } }

    public static Color[] ItemWorldGlowColor { get { return RarityDisplayColors; } }

    public static string[] StatDisplayNames { get { return ConvertTypes(Enum.GetNames(typeof(StatName))); } }

    public static string FormatStat(Stat stat) { // if the stat is a flat stat, round it to a whole number, if it's a percentage stat, round it to 1 decimal place and add a % symbol after
        return FormatStat(stat.StatType, stat.MaxValue);
    }

    public static string FormatStat(StatType statType, float value) {
        return (statType == StatType.Additive ? value.ToString("0.0") + "" : (value * 100f).ToString("0.0") + "%");
    }

    public static string FormatBonusStat(ItemBonus equippedBonus) {
        return FormatBonusStat(equippedBonus.StatName, equippedBonus.StatModType, equippedBonus.value);
    }

    public static string FormatBonusStat(StatName statName, StatModType statModType, float value) {
        // i really don't like the way im doing this rn
        Stat originalStat = PartyManager.Instance.Party.ActiveCharacter.GetStat<Stat>(statName) ?? PartyManager.Instance.Party.ActiveCharacter.GetStat<DepletableStat>(statName);

        return (originalStat.StatType == StatType.Additive) ?
               (statModType == StatModType.Flat) ? value + "" : (value * 100f).ToString("0.0") + "%" :
               (value * 100f).ToString("0.0") + "%";
    }

    public static string FormatRarityLore(Item item) {
        if (item is EquippableItem equippableItem)
            return RarityDisplayNames[(int)item.Rarity] + " " + EquipmentTypeDisplayNames[(int)equippableItem.equipmentType];
        else if (item is ConsumableItem consumableItem)
            return RarityDisplayNames[(int)item.Rarity] + " " + ((consumableItem.consumableItemType == ConsumableItemType.Special) ? "Consumable" : ConsumeableTypeDisplayNames[(int)consumableItem.consumableItemType]);
        else
            return RarityDisplayNames[(int)item.Rarity] + " " + ((item.ItemType == ItemType.Miscellaneous) ? "Item" : ItemTypeDisplayNames[(int)item.ItemType]);

    }

    private static string[] ConvertTypes(string[] originalEnum) { // enums uses _ to concat words, where in display we want a space instead of _
        return originalEnum.Select(a => a.Replace("_", " ")).ToArray();
    } 

}
