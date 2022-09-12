using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryToolTipDisplayer : MonoBehaviour {

    #region Singleton

    public static InventoryToolTipDisplayer Instance;

    void Awake() {

        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

    }

    #endregion

    public const float AUTO_FADEIN_THRESHOLD = 0.2f;
    public const float AUTO_FADEOUT_THRESHOLD = 0.2f;

    [SerializeField] private TextMeshProUGUI itemNameDisplay;
    [SerializeField] private TextMeshProUGUI itemLevelDisplay;
    [SerializeField] private TextMeshProUGUI itemTypeDisplay;

    [SerializeField] private ItemListDisplay itemListDisplayParent;

    [SerializeField] private TextMeshProUGUI itemRarityTypeDisplay;
    [SerializeField] private TextMeshProUGUI itemDescriptionDisplay;

    [SerializeField] private StarsDisplay starsDisplay;

    public Item CurrentItem { get { return currentItem; } }
    private Item currentItem;
    
    public Stat CurrentStat { get { return currentStat; } }
    private Stat currentStat;

    private bool isVisible;
    private bool isShowing;

    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();    
    }

    public void DisplayItem(Item item) {
        currentItem = item;

        if (CurrentItem == null)
            return;

        itemNameDisplay.text = CurrentItem.Name;

        itemLevelDisplay.text = (item is EquippableItem) ? "Lvl. " + CurrentItem.Level: ""; // for now only equippableItems will have levels

        itemNameDisplay.color = ItemUtils.RarityDisplayColors[(int)CurrentItem.Rarity];

        itemTypeDisplay.text = ItemUtils.ItemTypeDisplayNames[(int)CurrentItem.ItemType];

        itemListDisplayParent.SetItemBonusDisplay(CurrentItem);

        itemRarityTypeDisplay.text = ItemUtils.FormatRarityLore(CurrentItem);
        itemRarityTypeDisplay.color = ItemUtils.RarityDisplayColors[(int)CurrentItem.Rarity];

        itemDescriptionDisplay.text = CurrentItem.Description;

        starsDisplay.UpdateStarsDisplay((int)CurrentItem.Rarity + 1);

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        Show();
    }
    // beware: the readability of my string concatinations are insanely scuffed, stop now before u get a stroke from reading this mess
    public void DisplayStat(Stat stat) { // too lazy to make another class dedicated for the StatDisplayer, so some UIs might look weird
        currentStat = stat;

        if (CurrentStat == null)
            return;

        itemNameDisplay.text = $"{CurrentStat.DisplayName} [{ItemUtils.FormatStat(CurrentStat)}] " + ((stat.MaxValue == stat.BaseValue) ? "" :
            $"({ItemUtils.FormatStat(CurrentStat.StatType, CurrentStat.BaseValue)}" +
            $"{ ((stat.MaxValue - stat.BaseValue == 0) ? "" : ((CurrentStat.MaxValue - CurrentStat.BaseValue) > 0 ? " + " : " - ") + ItemUtils.FormatStat(CurrentStat.StatType, Mathf.Abs(CurrentStat.MaxValue - CurrentStat.BaseValue)))})");

        itemLevelDisplay.text = "Lvl. " + CurrentStat.level;
        itemNameDisplay.color = Color.white;

        itemTypeDisplay.text = (stat is DepletableStat) ? "DepletableStat" : "Stat";

        itemListDisplayParent.SetStatBonusDisplay(stat);

        itemRarityTypeDisplay.text = "";
        itemDescriptionDisplay.text = $"Base Value (Lvl.{CurrentStat.level}): {ItemUtils.FormatStat(CurrentStat.StatType, CurrentStat.BaseValue)} " +
                                      $"{((CurrentStat is DepletableStat depletableStat) ? $"\nCurrent Value: {ItemUtils.FormatStat(depletableStat.StatType, depletableStat.CurrentValue)}" : "")}" +
                                      $"\n{((CurrentStat is DepletableStat) ? "Max" : "Current")} Value: {ItemUtils.FormatStat(CurrentStat.StatType, CurrentStat.MaxValue)}";
        starsDisplay.UpdateStarsDisplay(0);
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        Show();
    }

    void Update() {
        if (isVisible)
            OnHover();
    }

    public void OnHover() {
         if (isVisible)                                                     // offset
            GetComponent<RectTransform>().position = Input.mousePosition + Vector3.right * 20f;
    }

    public void Show() {
        isShowing = true;

        StartCoroutine(FadeIn());
    }

    public void Hide() {
        isShowing = false;

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn() {
        float fadeTimer = 0f;

        while (fadeTimer < AUTO_FADEIN_THRESHOLD && isShowing) {
            fadeTimer += Time.deltaTime;
            yield return null;
        }


        if (fadeTimer >= AUTO_FADEIN_THRESHOLD) {
            animator.SetBool("isVisible", true);
            isVisible = true;
        }
    }

    private IEnumerator FadeOut() {
        float fadeTimer = 0f;

        while (fadeTimer < AUTO_FADEOUT_THRESHOLD && !isShowing) {
            fadeTimer += Time.deltaTime;
            yield return null;
        }


        if (fadeTimer >= AUTO_FADEOUT_THRESHOLD) {
            isVisible = false;
            animator.SetBool("isVisible", false);
        }
    }
}
