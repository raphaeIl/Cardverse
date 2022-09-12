using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class PartyManager : MonoBehaviour {

    #region Singleton

    public static PartyManager Instance;

    void Awake() {

        if (Instance == null)
            Instance = this;
        else {
            Destroy(this);
            return;
        }

        tavern.OpenTavern(UNIQUE_CHARACTERS.ToArray());
    }

    #endregion

    public ReadOnlyCollection<CharacterInfo> UNIQUE_CHARACTERS { get { return new HashSet<CharacterInfo>(uniqueCharacters).ToList().AsReadOnly(); } }

    [Tooltip("All unique characters the player owns, duplicates will be removed")]
    [SerializeField] private CharacterInfo[] uniqueCharacters;

    public bool IsTavernVisible { get { return tavernUI.IsVisible; } }

    public Party Party { get { return party; } }
    public Tavern Tavern { get { return tavern; } }

    public PartyController PartyController { get { return partyController; } } // i really didn't want to expose these fields but i had no choice lol, I'll find another solution later trust

    [SerializeField] private Party party;
    [SerializeField] private Tavern tavern;

    [SerializeField] private PartyController partyController;

    [SerializeField] private PartyUI partyUI;
    [SerializeField] private TavernUI tavernUI;

    // Start is called before the first frame update
    void Start() {
        //InventoryManager.Instance.owner = tavern.FindCharacterInTavern("Sebastian");

        UpdatePartyDisplay();

        party.OnPartyChange += (removedCharacter, addedCharacter) => UpdatePartyDisplay();
        party.OnPartyChange += partyController.UpdatePartyMembers;

        partyUI.OnSwapActiveCharacter += UpdateSelectedCharacter;

        party.OnPartyChange += tavernUI.RefreshPartyDisplay;
        tavern.OnTavernChange += tavernUI.RefreshTavernDisplay;

        // test code
        partyUI.OnSwapActiveCharacter += UpdateStatDisplay;
        partyUI.OnSwapActiveCharacter += UpdateEquipmentPanelDisplay;

        foreach (Character character in tavern.tavern) character.OnHealthChange += UpdatePartyDisplay;

        if (party.ActivePartyMemberCount == 0)
            RecruitCharacter(tavern.FindCharacterInTavern("Sebastian"));

        partyController.InitParty(party);
        
        Invoke("InitStatDisplay", 0.1f);
        Invoke("InitEquipmentPanelDisplay", 0.1f);
    }

    public void RecruitCharacter(Character character) {
        if (party.AddCharacter(character))
            if (tavern.RemoveCharacter(character))
                Toast.DisplayMessage($"Succesfully added {character.GetEntityInfo<CharacterInfo>().Name} to the party!");
            else
                Toast.DisplayError($"Unable to remove {character.GetEntityInfo<CharacterInfo>().Name} from the tavern", exception: new InvalidOperationException());
        else Toast.DisplayError($"Unable to add {character.GetEntityInfo<CharacterInfo>().Name} to the party", exception: new InvalidOperationException());
    }

    public void KickCharacter(Character character) {
         if (party.RemoveCharacter(character))
            if (tavern.AddCharacter(character))
                Toast.DisplayMessage($"Succesfully added {character.GetEntityInfo<CharacterInfo>().Name} to the tavern!");
            else
                Toast.DisplayError($"Unable to add {character.GetEntityInfo<CharacterInfo>().Name} to the tavern", exception: new InvalidOperationException());
        else Toast.DisplayError($"Unable to remove {character.GetEntityInfo<CharacterInfo>().Name} from the party", exception: new InvalidOperationException());
    }

    public void UpdateSelectedCharacter(int previousSelected, int nowSelected) {
        party.SelectCharacter(nowSelected);
        partyController.SelectCharacter(previousSelected, nowSelected);
    }

    public void UpdateStatDisplay(int previousSelected, int nowSelected) {
        CharacterMenu.Instance.SetCurrentCharacter(party.ActiveParty[nowSelected]);
    }

    public void UpdateEquipmentPanelDisplay(int previousSelected, int nowSelected) {
        InventoryManager.Instance.EquipmentPanel.SetCurrentCharacter(party.ActiveParty[nowSelected]);
    }

    public void UpdatePartyDisplay() {
        partyUI.UpdatePartyDisplay(party.ActiveParty);
    }

    public void OnModifyParty() {
        if (!tavernUI.IsInitialized)
            tavernUI.InitTavernUI(tavern, party);

        partyUI.Hide();
        InventoryInteractableHandler.Instance.HideAllVisiblePanels();

        tavernUI.Show();

        GameMaster.Instance.AudioManager.Play2DSFX(AudioManager.SFXType.Modify_Party_Button);
    }

    public void OnCloseTavern() {
        tavernUI.Hide();

        partyUI.Show();
        InventoryInteractableHandler.Instance.ShowAllVisiblePanels();

        GameMaster.Instance.AudioManager.Play2DSFX(AudioManager.SFXType.Close_Button);
    }

    private void InitTavern() {
        tavern.OpenTavern(UNIQUE_CHARACTERS.ToArray());
    }

    private void InitStatDisplay() {
        UpdateStatDisplay(-1, 0);
    }

    private void InitEquipmentPanelDisplay() {
        UpdateEquipmentPanelDisplay(-1, 0);
    }
}