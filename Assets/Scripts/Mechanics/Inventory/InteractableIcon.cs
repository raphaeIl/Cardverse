using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemSlot))]
public class InteractableIcon : InventoryInteractable {

    private ItemSlot currentSlot;
    private RectTransform currentlyDraggingObject;

    protected override void Awake() {
        base.Awake();

        currentSlot = GetComponent<ItemSlot>();

        IsActive = false;
        // event setup:
        InteractableActionEvents[(int)InteractableEventType.OnPointerClick].Event     += Click;
        InteractableActionEvents[(int)InteractableEventType.OnPointerEnter].Event     += PointerEnter;
        InteractableActionEvents[(int)InteractableEventType.OnPointerExit].Event      += PointerExit;
        InteractableActionEvents[(int)InteractableEventType.OnPointerHover].Event     += Hover;
        InteractableActionEvents[(int)InteractableEventType.OnPointerBeginDrag].Event += BeginDrag;
        InteractableActionEvents[(int)InteractableEventType.OnPointerDrag].Event      += Drag;
        InteractableActionEvents[(int)InteractableEventType.OnPointerEndDrag].Event   += EndDrag;

    }

    private void Click(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right) {
            InventoryToolTipDisplayer.Instance.Hide();
            InventoryActionPopUp.Instance.ShowInventoryActionPopUp(currentSlot);
        }
    }

    private void PointerEnter(PointerEventData eventData) {
        IsActive = true;

        InventoryActionPopUp.Instance.Hide();

        if (currentSlot.CurrentItem == null)
            InventoryToolTipDisplayer.Instance.Hide();

        InventoryToolTipDisplayer.Instance.DisplayItem(currentSlot.CurrentItem);
    }

    private void PointerExit(PointerEventData eventData) {
        IsActive = false;

        if (InventoryToolTipDisplayer.Instance.CurrentItem == currentSlot.CurrentItem) {
            InventoryToolTipDisplayer.Instance.DisplayItem(null);
            InventoryToolTipDisplayer.Instance.Hide();
        }
    }

    private void Hover(PointerEventData eventData) {

    }

    private void BeginDrag(PointerEventData eventData) {
        if (currentSlot == null || currentSlot.CurrentItem == null)
            return;

        InventoryToolTipDisplayer.Instance.Hide();

        GameObject clone = Instantiate(gameObject, InventoryManager.Instance.Inventory.transform); // because of the way the RectMask2D works, a clone has to be made for the player to drag it instead of the actual slot, (because the actual slot's parent has a rectmask which will make it disappear anywhere outside the mask's bounds)
        currentlyDraggingObject = clone.GetComponent<RectTransform>();
        Destroy(currentlyDraggingObject.GetComponent<InteractableIcon>());
        currentlyDraggingObject.sizeDelta = new Vector2(50, 50);

        gameObject.GetComponent<CanvasGroup>().alpha = 0.5f;
    }

    private void Drag(PointerEventData eventData) {
        if (currentlyDraggingObject == null)
            return;

        currentlyDraggingObject.position = eventData.position;
    }

    private void EndDrag(PointerEventData eventData) {
        print(gameObject.name);
        if (currentlyDraggingObject == null)
            return;
        
        InteractableIcon slotDropped = InventoryInteractableHandler.Instance.MouseOverInteractableIcon<InteractableIcon>();

        if (currentSlot is EquipmentSlot) { // dragging from EquipmentSlot => Anywhere else (player unequipping item)
            if (slotDropped == null || !(slotDropped.currentSlot is EquipmentSlot)) { // if the player drags an already equipped item to anywhere except the current slot location
                InventoryManager.Instance.UnequipItem((EquippableItem)currentSlot.CurrentItem); // unequip item from EquipmentPanel, add it back to the inventory
                currentSlot.SetItem(null);
            }
        } else { // dragging from Inventory => Equipment Slot (player equipping item)
            if (slotDropped != null && // is there an actual avaible slot under wherever the player dropped the item
                currentSlot.CurrentItem.ItemType == ItemType.Equippable && // is the dragged item an equippableitem
                (slotDropped.currentSlot is EquipmentSlot) && // is the slot dropped onto an equipment slot
                ((EquipmentSlot)slotDropped.currentSlot).equipmentType == ((EquippableItem)currentSlot.CurrentItem).equipmentType) { // does the slot dropped onto's type match the type of the item being dragged

                if (((EquipmentSlot)slotDropped.currentSlot).CurrentItem != null)
                    InventoryManager.Instance.UnequipItem((EquippableItem)((EquipmentSlot)slotDropped.currentSlot).CurrentItem);

                InventoryManager.Instance.EquipItem((EquippableItem)currentSlot.CurrentItem);
                currentSlot.SetItem(null);
            }

        } // another option might be swapping slots that already exist in the inventory, since the current inventory is "infinite" unlike a limited one (like minecraft's 45 slot inventory) ill take care of swapping slots later

        Destroy(currentlyDraggingObject.gameObject);
        gameObject.GetComponent<CanvasGroup>().alpha = 1f;
    }
}
