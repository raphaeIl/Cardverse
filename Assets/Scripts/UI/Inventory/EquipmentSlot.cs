using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : ItemSlot {

    [SerializeField] private Image backgroundIcon;

    public EquipmentType equipmentType;

    public override Item CurrentItem { 
        get { return currentItem; }
        set {
            currentItem = value;

            if (currentItem != null) {
                itemIcon.enabled = true;
                itemIcon.sprite = currentItem.DisplayIcon;
                backgroundIcon.gameObject.SetActive(false);
            } else {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
                backgroundIcon.gameObject.SetActive(true);
            }
        }
    }

    protected override void OnValidate() {
        base.OnValidate();
        gameObject.name = equipmentType.ToString() + " Slot";
    }

    public void InitBackgroundIcon(Sprite sprite) {
        backgroundIcon.sprite = sprite;
    }

    public override void SetItem(Item item) {
        CurrentItem = item;

        if (CurrentItem == null)
            return;
    }

}
