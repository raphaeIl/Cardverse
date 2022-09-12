using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour {

    public Stat CurrentStat { get; private set; }

    [SerializeField] private Image statIcon; // don't have sprites yet
    [SerializeField] private Text statName;
    [SerializeField] private Text statValue;

    [SerializeField] private TextMeshProUGUI changedValue;

    private Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
    }

    public void UpdateStatDisplay(Stat stat) {
        CurrentStat = stat;

        statName.text = (stat is DepletableStat) ? $"Max " + stat.DisplayName : stat.DisplayName;
        statValue.text = ItemUtils.FormatStat(stat);
    }

    public void ShowChangedValue(StatType statType, float amountChanged) {
        changedValue.text = (amountChanged > 0) ? "+" + ItemUtils.FormatStat(statType, amountChanged) : ItemUtils.FormatStat(statType, amountChanged);
        changedValue.faceColor = (amountChanged > 0) ? Color.green : Color.red;

        animator.SetTrigger("changeValue");
    }
}
