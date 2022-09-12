using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryActionDialogue : MonoBehaviour {

    #region Singleton

    public static InventoryActionDialogue Instance;

    void Awake() {

        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

    }

    #endregion

    [SerializeField] private Text titleDisplay;
    [SerializeField] private Text actionDescriptionDisplay;
    [SerializeField] private Image itemImage;
    [SerializeField] private Text itemAmountDisplay;
    [SerializeField] private Text itemNameDisplay;
    [SerializeField] private Text itemDescriptionDisplay;

    private ItemSlot currentlyDisplayingSlot;
    private ActionDialogueType currentDialogueType;

    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();

        Hide();
        Disable();
    }

    public void ShowInventoryActionDialogue(ItemSlot slotSelected, ActionDialogueType actionDialogueType) {
        this.currentlyDisplayingSlot = slotSelected;
        this.currentDialogueType = actionDialogueType;

        titleDisplay.text = actionDialogueType == ActionDialogueType.Item_Deletion ? "Confirm delete item" : "Use item";
        actionDescriptionDisplay.text = actionDialogueType == ActionDialogueType.Item_Deletion ? "Are you sure you want to delete this item?" : "Use this item?"; //  "Select amount to use"
        itemImage.sprite = slotSelected.CurrentItem.DisplayIcon;
        itemAmountDisplay.text = slotSelected.CurrentItem.Amount + "";
        itemNameDisplay.text = slotSelected.CurrentItem.Name;
        itemDescriptionDisplay.text = slotSelected.CurrentItem.Description;

        Enable();
        Show();
    }

    public void Show() {
        animator.SetBool("isVisible", true);
    }

    public void Hide() {
        animator.SetBool("isVisible", false);
    }

    public void Enable() {
        this.gameObject.SetActive(true);
    }

    public void Disable() {
        this.gameObject.SetActive(false);
    }

    public void OnCancel() {
        Hide();
    }

    public void OnConfirm() {
        if (currentDialogueType == ActionDialogueType.Item_Deletion)
            InventoryManager.Instance.RemoveItem(currentlyDisplayingSlot.CurrentItem);
        else {
            if (currentlyDisplayingSlot.CurrentItem is ConsumableItem consumableItem)
                consumableItem.Consume(PartyManager.Instance.Party.ActiveCharacter);
        }

        Hide();
    }

    public enum ActionDialogueType {
        Item_Deletion,
        Item_Use
    }

}
