using UnityEngine;
using UnityEngine.UI;

public class CharacterIcon : MonoBehaviour {

    [SerializeField] private Text characterName;
    [SerializeField] private Text characterLevel;
    [SerializeField] private Image characterSprite;
    [SerializeField] private Button characterOptions;

    private Animator animator;

    public Character CurrentCharacter { get { return currentCharacter; } }
    private Character currentCharacter;

    private bool isInParty;

    void Start() {
        animator = GetComponent<Animator>();

        animator.SetBool("isActive", true);
    }

    void OnDestroy() {
        animator.SetBool("isActive", false);    
    }

    public void UpdateCharacterIcon(Character character, bool isInParty) {
        currentCharacter = character;

        characterName.text = character.GetEntityInfo<CharacterInfo>().Name;
        characterLevel.text = $"Lv.{character.level.RoundedCurrentLevel}";

        if (character.GetEntityInfo<CharacterInfo>().DisplaySprite != null) {
            characterSprite.sprite = character.GetEntityInfo<CharacterInfo>().DisplaySprite;
            characterSprite.color = Color.white;
        } else 
            characterSprite.color = Color.black;

        this.isInParty = isInParty; // !!
        characterOptions.GetComponentInChildren<Text>().text = isInParty ? "remove" : "add";
    }

    public void OnToggleOptions() {
        characterOptions.gameObject.SetActive(!characterOptions.gameObject.activeSelf);

        GameMaster.Instance.AudioManager.Play2DSFX(AudioManager.SFXType.Select_Character_Icon);
    }

    public void OnOptionSelected() {
        // if the current character is in the party, kick it
        // if the current character is in the tavern, add to party
        if (isInParty)
            PartyManager.Instance.KickCharacter(currentCharacter);
        else
            PartyManager.Instance.RecruitCharacter(currentCharacter);

        GameMaster.Instance.AudioManager.Play2DSFX(AudioManager.SFXType.Character_Icon_Select_Option);
    }

}
