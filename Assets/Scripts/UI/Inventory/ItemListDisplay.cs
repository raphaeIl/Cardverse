using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemListDisplay : MonoBehaviour {

    [SerializeField] private GameObject listElementPrefab;

    public void SetItemBonusDisplay(Item item) {
        if (item.ItemType == ItemType.Equippable)
            CreateItemBonusList(((EquippableItem)item).ItemBonuses);
        else if (item.ItemType == ItemType.Consumable)
            CreateItemBonusList(((ConsumableItem)item).ItemBonuses);
     // else item is generic, no bonuses
    }

    public void SetStatBonusDisplay(Stat stat) {
        CreateStatBonusList(stat.StatModifiers.ToArray());
    }


    private void CreateItemBonusList(ItemBonus[] itemBonuses) {
        Clear();

        if (itemBonuses == null)
            return;

        foreach (ItemBonus itemBonus in itemBonuses) {
            ItemBonusUI itemBonusUI = Instantiate(listElementPrefab, transform).GetComponent<ItemBonusUI>();
            itemBonusUI.UpdateItemBonus(itemBonus);
        }
    }

    private void CreateStatBonusList(StatModifier[] statModifiers) {
        Clear();

        if (statModifiers == null)
            return;

        foreach (StatModifier statModifier in statModifiers) {
            ItemBonusUI statBonusUI = Instantiate(listElementPrefab, transform).GetComponent<ItemBonusUI>();
            statBonusUI.UpdateStatBonus(statModifier);
        }
    }

    private void Clear() {
        foreach (Transform transform in transform) {
            Destroy(transform.gameObject);
        }
    }
}
