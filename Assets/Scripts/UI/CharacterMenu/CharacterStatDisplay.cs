using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterStatDisplay : MonoBehaviour {

    [SerializeField] private GameObject statDisplayPrefab;

    public List<StatDisplay> CurrentStatDisplays { get { return currentStatDisplays; } }
    private List<StatDisplay> currentStatDisplays;

    void Awake() {
        currentStatDisplays = new List<StatDisplay>();
    }

    public void UpdateStats(Character character, ItemBonus.ModifiedStatData[] modifiedStats) {
        for (int i = 0; i < modifiedStats.Length; i++)
            currentStatDisplays.Find(statDisplay => statDisplay.CurrentStat.StatName == modifiedStats[i].statModified.StatName).ShowChangedValue(modifiedStats[i].statModified.StatType, modifiedStats[i].currentValue - modifiedStats[i].previousValue);

        UpdateStats(character);
    }

    public void UpdateStats(Character character) {
        Stat[] statsToDisplay = character.GetStats(); // not displaying passive elemental stats

        for (int i = 0; i < statsToDisplay.Length; i++) { //TODO: check needed if there is more stat displays than the current character's stats
            currentStatDisplays[i].UpdateStatDisplay(statsToDisplay[i]);
        }
    }

    public void InitStats(Character character) {
        Clear();

        Stat[] statsToDisplay = character.GetStats();
        foreach (Stat stat in statsToDisplay) {
            StatDisplay statDisplay = Instantiate(statDisplayPrefab, transform).GetComponent<StatDisplay>();
            statDisplay.UpdateStatDisplay(stat);

            currentStatDisplays.Add(statDisplay);
        }
    }

    public void Clear() {
        foreach (Transform transform in transform) {
            currentStatDisplays.Clear();
            Destroy(transform.gameObject);
        }
    }
}
