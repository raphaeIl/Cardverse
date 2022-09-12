using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ItemBonusUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private TextMeshProUGUI valueDisplay;

    void OnValidate() {
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void UpdateItemBonus(ItemBonus itemBonus) {
        nameDisplay.text = ItemUtils.StatDisplayNames[(int)itemBonus.StatName] + ":";

        valueDisplay.color = (itemBonus is EquippedBonus) ? Color.red : (itemBonus.value > 0) ? Color.green : Color.cyan;
        valueDisplay.text = ((itemBonus.value > 0) ? "+" : "") + ItemUtils.FormatBonusStat(itemBonus);

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void UpdateStatBonus(StatModifier statModifier) {

        Item item = statModifier.Source as Item;

        if (item == null) {
            Debug.LogError($"{statModifier.Source} is not an Item, currently ItemBonusUIs only supports Items :(");
            return;
        }

        nameDisplay.text = item.Name;
        nameDisplay.color = (item is EquippableItem) ? Color.magenta : (item is ConsumableItem) ? Utils.ConvertColor(80, 200, 120) : Color.gray;

        ItemBonus currentItemBonus = item.ItemBonuses.Where(bonus => bonus.value == statModifier.Value && bonus.StatModType == statModifier.Type).FirstOrDefault();
        valueDisplay.text = $"{((statModifier.Value > 0) ? "+" : "")}{ItemUtils.FormatBonusStat(currentItemBonus.StatName, statModifier.Type, statModifier.Value)}";
        valueDisplay.color = (statModifier.Value > 0) ? Color.green : Color.red;

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }


}
