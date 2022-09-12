using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryActionPopUp : MonoBehaviour {

    #region Singleton

    public static InventoryActionPopUp Instance;

    void Awake() {

        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

    }

    #endregion

    private Animator animator;
    private Button[] actionButtons;

    private ItemSlot currentlyDisplayingSlot;

    void OnValidate() {
        actionButtons = GetComponentsInChildren<Button>();    
    }

    void Start() {
        animator = GetComponent<Animator>();
    }

    public void ShowInventoryActionPopUp(ItemSlot slotSelected) {
        currentlyDisplayingSlot = slotSelected;

        animator.SetBool("isVisible", true);
        GetComponent<RectTransform>().position = slotSelected.transform.position;

        // dsiable on use button is not consumeablitem
        if (currentlyDisplayingSlot.CurrentItem.ItemType == ItemType.Consumable) {
            actionButtons[2].interactable = true;
            actionButtons[2].GetComponentInChildren<Text>().color = new Color(1, 1, 1, 1f);
        } else {
            actionButtons[2].interactable = false;
            actionButtons[2].GetComponentInChildren<Text>().color = new Color(1, 1, 1, 0.3f);
        }
    }
    
    public void Hide() {
        animator.SetBool("isVisible", false);
    }

    public void OnDelete() {
        InventoryActionDialogue.Instance.ShowInventoryActionDialogue(currentlyDisplayingSlot, InventoryActionDialogue.ActionDialogueType.Item_Deletion);

        Hide();
    }

    public void OnDrop() { // defaultly for now drop amount is 1
        int dropAmount = 1;

        Item itemToDrop = currentlyDisplayingSlot.CurrentItem;

        if (currentlyDisplayingSlot.CurrentItem.Amount < dropAmount)
            dropAmount = currentlyDisplayingSlot.CurrentItem.Amount;

        Item clone = itemToDrop.Clone();
        clone.Amount = dropAmount;

        ItemWorldSpawner.Instance.SpawnItemWorld(clone, PartyManager.Instance.PartyController.WorldPosition);
        InventoryManager.Instance.RemoveItem(currentlyDisplayingSlot.CurrentItem, dropAmount);

        Hide();
    }

    public void OnUse() {
        InventoryActionDialogue.Instance.ShowInventoryActionDialogue(currentlyDisplayingSlot, InventoryActionDialogue.ActionDialogueType.Item_Use);
        
        Hide();
    }

}
