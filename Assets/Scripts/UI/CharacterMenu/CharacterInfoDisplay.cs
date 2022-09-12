using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterInfoDisplay : MonoBehaviour {

    [SerializeField] private Text characterName;
    [SerializeField] private Text characterLevel;
    [SerializeField] private Image progressBar;

    public void UpdateCharacterInfoDisplay(Character character) {
        characterName.text = character.GetEntityInfo<CharacterInfo>().Name;
        characterLevel.text = $"Level {character.level.RoundedCurrentLevel} / {character.level.MaxLevel}";
        progressBar.fillAmount = character.level.CurrentLevel / character.level.MaxLevel;
    }
}
