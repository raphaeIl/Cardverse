using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class EquipmentPanel : MonoBehaviour {

    [SerializeField] private Sprite[] equipmentSlotIcons;
    [SerializeField] private Transform equipmentPanelParent;

    public EquipmentSlot[] EquipmentSlots { get { return equipmentSlots; } }
    [SerializeField] private EquipmentSlot[] equipmentSlots;

    [SerializeField] private Text panelTitle;

    public event Action OnEquipmentPanelChange;

    private Character currentCharacter;

    void Start() {
        OnEquipmentPanelChange += UpdateEquipmentDisplay;
    }

    void OnValidate() {
        equipmentSlots = equipmentPanelParent.GetComponentsInChildren<EquipmentSlot>();

        if (equipmentSlots.Length != EquipmentInventory.MAX_EQUIPMENT_INVENTORY_SIZE)
            Debug.LogError("There is more Equipment Slots available than the amount of types of equipments");
    }

    void Update() {

    }

    public void SetCurrentCharacter(Character character) {
        currentCharacter = character;

        UpdateEquipmentDisplay();
    }

    public bool AddEquipment(EquippableItem equippableItem, out EquippableItem previousItem) {
        if (currentCharacter == null) {
            previousItem = null;
            Debug.LogError("Current character not set");
            return false;
        }

        if (currentCharacter.EquipmentInventory.AddEquipment(equippableItem, out previousItem)) {
            if (equippableItem.EquipTo(currentCharacter)) {

                if (OnEquipmentPanelChange != null)
                    OnEquipmentPanelChange();

                return true;
            }
        }
        return false;
    }

    public bool RemoveEquipment(EquippableItem equippableItem) {
        if (currentCharacter == null) {
            Debug.LogError("Current character not set");
            return false;
        }

        if (currentCharacter.EquipmentInventory.RemoveEquipment(equippableItem)) {
            if (equippableItem.UnequipFrom(currentCharacter)) {

                if (OnEquipmentPanelChange != null)
                    OnEquipmentPanelChange();

                return true;
            }
        }
        return false;
    }

    public void UpdateEquipmentDisplay() {
        if (currentCharacter == null)
            throw new InvalidOperationException("Current character not set");

        panelTitle.text = $"{currentCharacter.GetEntityInfo<CharacterInfo>().Name}'s Equipment";

        foreach (EquipmentSlot equipmentSlot in equipmentSlots)
            equipmentSlot.InitBackgroundIcon(equipmentSlotIcons[(int)equipmentSlot.equipmentType]);

        for (int i = 0; i < EquipmentInventory.MAX_EQUIPMENT_INVENTORY_SIZE; i++) {
            if (i > equipmentSlots.Length - 1)
                throw new InvalidOperationException("equipment count does not match the total amount of slots that are available");
            equipmentSlots[i].SetItem(currentCharacter.EquipmentInventory.CurrentEquipment[i]);
        }
    }
}
