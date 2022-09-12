using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tavern : MonoBehaviour {

    /// <summary>
    /// Parameter 1: Character that was removed (can be null)
    /// Parameter 2: Character that was added (can be null)
    /// </summary>
    public event System.Action<Character, Character> OnTavernChange;

    public List<Character> tavern { get { return _tavern; } }
    private List<Character> _tavern;

    [SerializeField] private GameObject characterTemplatePrefab;

    void Awake() {
    }

    public void OpenTavern(CharacterInfo[] charactersToAdd) { // at start of game, every available character will be sent to the tavern with their assigned tavern id (for now)
        _tavern = new List<Character>();

        for (int i = 0; i < charactersToAdd.Length; i++) { 
            Character character = Instantiate(characterTemplatePrefab, transform).GetComponent<Character>();
            CharacterInfo characterInfo = charactersToAdd[i];

            character.gameObject.name = characterInfo.Name;
            characterInfo.TavernID = i;
            character.SetEntityInfo(characterInfo);

            //character.gameObject.SetActive(false);

            tavern.Add(character);
        }
    }

    public bool AddCharacter(Character characterToAdd) {
        if (_tavern.Contains(characterToAdd)) {
            print($"{characterToAdd.GetEntityInfo<CharacterInfo>().Name} is already in the tavern");
            return false;
        }

        _tavern.Add(characterToAdd);

        if (OnTavernChange != null)
            OnTavernChange(null, characterToAdd);

        return true;
    }

    public bool RemoveCharacter(Character characterToRemove) {
        if (!_tavern.Contains(characterToRemove)) {
            print($"{characterToRemove.GetEntityInfo<CharacterInfo>().Name} is not currently in the tavern");
            return false;
        }

        if (_tavern.Remove(characterToRemove)) {
            if (OnTavernChange != null)
                OnTavernChange(characterToRemove, null);

            return true;
        }

        return false;
    }

    public void SortByID() {
        _tavern.Sort((first, second) => first.GetEntityInfo<CharacterInfo>().TavernID - second.GetEntityInfo<CharacterInfo>().TavernID);
    }

    public void SortByName() {
        _tavern.Sort((first, second) => first.GetEntityInfo<CharacterInfo>().Name.CompareTo(second.GetEntityInfo<CharacterInfo>().Name));
    }

    public Character FindCharacterInTavern(int id) {
        return _tavern.Where(character => character.GetEntityInfo<CharacterInfo>().TavernID == id).FirstOrDefault();
    }

    public Character FindCharacterInTavern(string name) {
        return _tavern.Where(character => character.GetEntityInfo<CharacterInfo>().Name.Equals(name)).FirstOrDefault();
    }
}
