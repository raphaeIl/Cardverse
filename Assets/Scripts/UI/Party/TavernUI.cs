using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TavernUI : MonoBehaviour {

    private Animator animator;

    public bool IsVisible { get { return animator.GetBool("isVisible"); } }
    public bool IsInitialized { get { return currentParty != null && currentTavern != null; } }
    private Party currentParty;
    private Tavern currentTavern;

    [SerializeField] private GameObject currentPartyParent;
    [SerializeField] private GameObject currentTavernParent;
    [SerializeField] private GameObject characterIconPrefab;

    private List<CharacterIcon> tempTavernDisplay;
    private List<CharacterIcon> tempPartyDisplay;

    void Awake() {
        animator = GetComponent<Animator>();

        tempTavernDisplay = new List<CharacterIcon>();
        tempPartyDisplay = new List<CharacterIcon>();
    }

    void Start() {
        //Hide();
    }

    public void InitTavernUI(Tavern tavern, Party party) {
        currentTavern = tavern;
        currentParty = party;

        foreach (Character character in currentTavern.tavern) {
            CharacterIcon characterIcon = Instantiate(characterIconPrefab, currentTavernParent.transform).GetComponent<CharacterIcon>();
            characterIcon.UpdateCharacterIcon(character, false);

            tempTavernDisplay.Add(characterIcon);
        }

        for (int i = 0; i < currentParty.ActivePartyMemberCount; i++) {
            CharacterIcon characterIcon = Instantiate(characterIconPrefab, currentPartyParent.transform).GetComponent<CharacterIcon>();
            characterIcon.UpdateCharacterIcon(currentParty.ActiveParty[i], true);

            tempPartyDisplay.Add(characterIcon);
        } 
    }

    public void Show() {
        animator.SetBool("isVisible", true);
    }

    public void Hide() {
        animator.SetBool("isVisible", false);
    }

    public void RefreshTavernDisplay(Character removedCharacter, Character addedCharacter) {

        if (!IsInitialized) return;

        if (removedCharacter != null) {
            CharacterIcon removedCharacterIcon = FindCharacterIconInTavernDisplay(removedCharacter);

            Destroy(removedCharacterIcon.gameObject);
            tempTavernDisplay.Remove(removedCharacterIcon);
        }

        if (addedCharacter != null) {
            CharacterIcon addedCharacterIcon = Instantiate(characterIconPrefab, currentTavernParent.transform).GetComponent<CharacterIcon>();

            addedCharacterIcon.UpdateCharacterIcon(addedCharacter, false);

            tempTavernDisplay.Add(addedCharacterIcon);
        }
    }

    public void RefreshPartyDisplay(Character removedCharacter, Character addedCharacter) {

        if (!IsInitialized) return;

        if (removedCharacter != null) {
            CharacterIcon removedCharacterIcon = FindCharacterIconInPartyDisplay(removedCharacter);

            Destroy(removedCharacterIcon.gameObject);
            tempPartyDisplay.Remove(removedCharacterIcon);
        }

        if (addedCharacter != null) {
            CharacterIcon addedCharacterIcon = Instantiate(characterIconPrefab, currentPartyParent.transform).GetComponent<CharacterIcon>();

            addedCharacterIcon.UpdateCharacterIcon(addedCharacter, true);

            tempPartyDisplay.Add(addedCharacterIcon);
        }
    }

    private CharacterIcon FindCharacterIconInTavernDisplay(Character character) {
        return tempTavernDisplay.Where(icon => icon.CurrentCharacter == character).FirstOrDefault();
    }

    private CharacterIcon FindCharacterIconInPartyDisplay(Character character) {
        return tempPartyDisplay.Where(icon => icon.CurrentCharacter == character).FirstOrDefault();
    }
}
