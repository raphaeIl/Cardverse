using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour {

    public Character CurrentCharacter { get; private set; }

    [SerializeField] private Text memberName;
    [SerializeField] private Text memberLevel;
    [SerializeField] private Image memberIcon;
    [SerializeField] private Image memberHealthProgressbar;
    [SerializeField] private Image cooldownProgressbar;
    [SerializeField] private Text cooldownText;

    private Animator animator;
    private bool isSelected;

    void Awake() {
        animator = GetComponent<Animator>();
    }

    void Update() {
        animator.SetBool("isSelected", isSelected);
    }

    public void UpdateCharacterInfoDisplay(Character character) {
        CurrentCharacter = character;

        memberName.text = character.GetEntityInfo<CharacterInfo>().Name;
        memberLevel.text = $"Lv. {character.level.RoundedCurrentLevel}";
        memberHealthProgressbar.fillAmount = character.GetStat<DepletableStat>(StatName.HP).CurrentValue / character.GetStat<DepletableStat>(StatName.HP).MaxValue;
        // no all sprites yet
        if (character.GetEntityInfo<CharacterInfo>().DisplaySprite != null) {
            memberIcon.sprite = character.GetEntityInfo<CharacterInfo>().DisplaySprite;
            memberIcon.color = Color.white;
        } else
            memberIcon.color = Color.black;
    }

    public void UpdateCooldownProgressbar(float progress, float total) {
        cooldownProgressbar.fillAmount = progress / total;

        cooldownText.text = progress > 0 ? progress.ToString("0.0") : "";
    }

    public void SetSelected(bool selected) {
        isSelected = selected;
    }

    public void Select() {
        isSelected = true;
    }

    public void Deselect() {
        isSelected = false;
    }

}
