using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryInteractable : MonoBehaviour, IInventoryInteractable {

    public virtual GameObject AttachedGameObject { get { return gameObject; } }
    public virtual bool IsActive { get; set; }

    // seems a bit extra but maybe this event system will be useful in the future??
    public InteractableEvent[] InteractableActionEvents;

    protected virtual void Awake() {
        int totalEventsCount = Enum.GetNames(typeof(InteractableEventType)).Length;
        InteractableActionEvents = new InteractableEvent[totalEventsCount];

        for (int i = 0; i < totalEventsCount; i++)
            InteractableActionEvents[i] = new InteractableEvent();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        InteractableActionEvents[(int)InteractableEventType.OnPointerBeginDrag].Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData) {
        InteractableActionEvents[(int)InteractableEventType.OnPointerDrag].Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData) {
        InteractableActionEvents[(int)InteractableEventType.OnPointerEndDrag].Invoke(eventData);
    }

    public void OnHover(PointerEventData eventData) {
        InteractableActionEvents[(int)InteractableEventType.OnPointerHover].Invoke(eventData);
    }

    public void OnPointerClick(PointerEventData eventData) {
        InteractableActionEvents[(int)InteractableEventType.OnPointerClick].Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        InteractableActionEvents[(int)InteractableEventType.OnPointerEnter].Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData) {
        InteractableActionEvents[(int)InteractableEventType.OnPointerExit].Invoke(eventData);
    }

    [Serializable]
    public class InteractableEvent {
        public event Action<PointerEventData> Event;
        public event Action<PointerEventData, IInventoryInteractable> AdvancedEvent; // extra IInventoryInteractable parameter for other non IInteractable classes that subscribe to these events?

        public InteractableEvent() { }

        public InteractableEvent(IInventoryInteractable targetInventoryInteractable) {
            Event += (eventData) => AdvancedEvent(eventData, targetInventoryInteractable);
        }

        public void Invoke(PointerEventData eventData) {
            if (Event != null)
                Event(eventData);
        }
    }

    public enum InteractableEventType {
        OnPointerClick,
        OnPointerEnter,
        OnPointerExit,
        OnPointerBeginDrag,
        OnPointerDrag,
        OnPointerEndDrag,
        OnPointerHover
    }
}
