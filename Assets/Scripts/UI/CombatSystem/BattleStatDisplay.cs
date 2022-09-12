using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStatDisplay : MonoBehaviour {

    public Text statName;
    public Text currentValue;
    public Text maxValue;
    public Image progressBar;

    public void UpdateStatDisplay(DepletableStat stat) {
        UpdateStatDisplay(stat.DisplayName, stat.CurrentValue, stat.MaxValue);
    }

    private void UpdateStatDisplay(string statName, float currentValue, float maxValue) { 
        this.statName.text = statName;
        this.currentValue.text = Mathf.RoundToInt(currentValue) + "";
        this.maxValue.text = " / " + Mathf.RoundToInt(maxValue);
        progressBar.fillAmount = (float)currentValue / maxValue;
    }
}
