using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(StatDisplay))]
public class InteractableListElement : InventoryInteractable {

    private StatDisplay currentElement;

    protected override void Awake() {
        base.Awake();

        IsActive = false;

        InteractableActionEvents[(int)InteractableEventType.OnPointerEnter].Event     += PointerEnter;
        InteractableActionEvents[(int)InteractableEventType.OnPointerExit].Event      += PointerExit;
    }

    void Start() {
        currentElement = GetComponent<StatDisplay>();
    }

    public void PointerEnter(PointerEventData eventData) {
        IsActive = true;

        InventoryToolTipDisplayer.Instance.DisplayStat(currentElement.CurrentStat);
    }

    public void PointerExit(PointerEventData eventData) {
        IsActive = false;

        if (InventoryToolTipDisplayer.Instance.CurrentStat == currentElement.CurrentStat) {
            InventoryToolTipDisplayer.Instance.DisplayStat(null);
            InventoryToolTipDisplayer.Instance.Hide();
        }
    }

}
