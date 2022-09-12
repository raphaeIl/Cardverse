using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using TMPro;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemWorld : MonoBehaviour {

    public Item CurrentItem { get { return currentItem; } }
    [SerializeField] private Item currentItem;

    [SerializeField] private TextMeshPro amountDisplay;
    private SpriteRenderer spriteRenderer;
    private Light2D light;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        light = GetComponentInChildren<Light2D>();   
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        PartyMember characterCollided = collider.GetComponent<PartyMember>();

        if (characterCollided != null && characterCollided.GetComponent<CharacterController2D>() != null) {
            InventoryManager.Instance.AddItem(currentItem, currentItem.Amount);
            DestroySelf();
        }

        //ItemWorld itemWorldCollided = collider.GetComponent<ItemWorld>();
    }

    public void UpdateItemWorld(Item item) {
        this.currentItem = item;
         
        if (CurrentItem == null)
            return;

        spriteRenderer.sprite = item.DisplayIcon;
        light.color = ItemUtils.ItemWorldGlowColor[(int)item.Rarity];

        amountDisplay.enabled = CurrentItem.Amount > 1;
        amountDisplay.text = item.Amount + "";
        
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }
}
