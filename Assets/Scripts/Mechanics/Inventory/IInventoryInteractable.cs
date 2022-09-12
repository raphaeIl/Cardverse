using UnityEngine;
using UnityEngine.EventSystems;

public interface IInventoryInteractable : IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
    
    public GameObject AttachedGameObject { get; }
    public bool IsActive { get; set; }

    public void OnHover(PointerEventData eventData); // custom 
}
