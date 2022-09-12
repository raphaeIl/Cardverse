using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyUI : MonoBehaviour {

    public const float switchCooldown = 1f;
    public event System.Action<int, int> OnSwapActiveCharacter;

    public int SelectedSlot { get; private set; }

    private PartyMemberUI[] partyMemberUIs;

    private float switchTimer;

    private Animator animator;

    void Awake() {
        partyMemberUIs = GetComponentsInChildren<PartyMemberUI>();
        animator = GetComponentInParent<Animator>();
    }

    void Start() {
        OnSwapActiveCharacter += UpdateSelectedCharacter;    
        
         if (OnSwapActiveCharacter != null) // defaultly selecting first character in party
             OnSwapActiveCharacter(SelectedSlot, 0);

        Show();
    }

    void Update() {
        if (switchTimer > 0) {
            switchTimer -= Time.deltaTime;
            UpdateCooldownTimerDisplay();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && SelectedSlot != 0 && partyMemberUIs[0].CurrentCharacter != null) {
            if (OnSwapActiveCharacter != null)
                OnSwapActiveCharacter(SelectedSlot, 0);
            SelectedSlot = 0;
            
            GameMaster.Instance.AudioManager.Play2DSFX(AudioManager.SFXType.Swap_Character);
        } else if (Input.GetKeyDown(KeyCode.Alpha2) && SelectedSlot != 1 && partyMemberUIs[1].CurrentCharacter != null) {
            if (OnSwapActiveCharacter != null)
                OnSwapActiveCharacter(SelectedSlot, 1);
            SelectedSlot = 1;

            GameMaster.Instance.AudioManager.Play2DSFX(AudioManager.SFXType.Swap_Character);
        } else if (Input.GetKeyDown(KeyCode.Alpha3) && SelectedSlot != 2 && partyMemberUIs[2].CurrentCharacter != null) {
            if (OnSwapActiveCharacter != null)
                OnSwapActiveCharacter(SelectedSlot, 2);
            SelectedSlot = 2;

            GameMaster.Instance.AudioManager.Play2DSFX(AudioManager.SFXType.Swap_Character);
        } else if (Input.GetKeyDown(KeyCode.Alpha4) && SelectedSlot != 3 && partyMemberUIs[3].CurrentCharacter != null) {
            if (OnSwapActiveCharacter != null)
                OnSwapActiveCharacter(SelectedSlot, 3);
            SelectedSlot = 3;

            GameMaster.Instance.AudioManager.Play2DSFX(AudioManager.SFXType.Swap_Character);
        }
    }

    public void UpdateSelectedCharacter(int previousSelected, int nowSelected) {
        DeselectedAll();
        partyMemberUIs[nowSelected].Select();
        
        switchTimer = switchCooldown;
    }

    public void UpdatePartyDisplay(Character[] characters) {
        for (int i = 0; i < Party.PARTY_LIMIT; i++) {
            Character currentCharacter = characters[i];

            partyMemberUIs[i].gameObject.SetActive(currentCharacter != null);

            if (currentCharacter != null) {
                partyMemberUIs[i].UpdateCharacterInfoDisplay(characters[i]);
            }
        }
    }

    public void Show() {
        animator.SetBool("isVisible", true);
    }

    public void Hide() {
        animator.SetBool("isVisible", false);
    }

    public void DeselectedAll() {
        foreach (PartyMemberUI partyMember in partyMemberUIs)
            partyMember.Deselect();
    }

    private void UpdateCooldownTimerDisplay() {
        foreach (PartyMemberUI partyMember in partyMemberUIs)
            if (partyMember != partyMemberUIs[SelectedSlot])
                partyMember.UpdateCooldownProgressbar(switchTimer, switchCooldown);
    }
}
