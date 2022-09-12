using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour {

    [SerializeField] protected Image itemIcon;
    [SerializeField] private Text amountDisplay;

    [HideInInspector] public RectTransform rectTransform;

    protected Item currentItem;
    public virtual Item CurrentItem { 
        get { return currentItem; }
        set {
            currentItem = value;

            if (currentItem != null) {
                itemIcon.enabled = true;
                itemIcon.sprite = currentItem.DisplayIcon;
            } else {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }
        }
    }

    protected virtual void OnValidate() {
        if (itemIcon == null)
            itemIcon = GetComponent<Image>();
    }

    protected virtual void Start() {
        rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void Update() {

    }

    public virtual void SetItem(Item item) {
        CurrentItem = item;

        if (CurrentItem == null)
            return;

        amountDisplay.gameObject.SetActive(item.isStackable && item.Amount > 1);
        amountDisplay.text = item.Amount + "";
    }

}
