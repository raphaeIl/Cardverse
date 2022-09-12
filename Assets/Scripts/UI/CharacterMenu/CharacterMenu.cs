using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMenu : MonoBehaviour {

    #region Singleton

    public static CharacterMenu Instance;

    void Awake() {

        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

    }

    #endregion

    public List<StatDisplay> CharacterStatDisplays { get { return characterStatDisplay.CurrentStatDisplays; } }

    [SerializeField] private CharacterInfoDisplay characterInfoDisplay;
    [SerializeField] private CharacterStatDisplay characterStatDisplay;

    private Character currentCharacter;

    public void SetCurrentCharacter(Character character) {
        //currentCharacter.OnStatChange -= UpdateCharacterMenu; // unsafe check
        //currentCharacter.level.OnLevelChanged -= (a, b) => UpdateCharacterMenu();

        currentCharacter = character;

        currentCharacter.OnStatChange += UpdateCharacterStats;
        currentCharacter.level.OnLevelChanged += (a, b) => UpdateCharacterMenu();

        characterStatDisplay.InitStats(character);
        UpdateCharacterMenu();
    }

    public void UpdateCharacterStats(ItemBonus.ModifiedStatData[] modifiedStatData) {
        characterStatDisplay.UpdateStats(currentCharacter, modifiedStatData);

        UpdateCharacterMenu();
    }

    public void UpdateCharacterMenu() {
        if (currentCharacter == null) {
            print("Current Character not set");
            return;
        }

        characterInfoDisplay.UpdateCharacterInfoDisplay(currentCharacter);
        characterStatDisplay.UpdateStats(currentCharacter);
    }

    // test code
    public void OnAddExp() {
        currentCharacter.level.AddEXP(100);
    }
}

