using UnityEngine;
using System.Linq;
using System;

public class Party : MonoBehaviour {

    public const int PARTY_LIMIT = 4;

    /// <summary>
    /// Parameter 1: Character that was removed (can be null)
    /// Parameter 2: Character that was added (can be null)
    /// </summary>
    public event System.Action<Character, Character> OnPartyChange; 

    public Character[] ActiveParty { get { return activeParty; } }
    private Character[] activeParty;
    
    public Character ActiveCharacter { get { return activeCharacter; } }
    private Character activeCharacter;

    void Awake() {
        activeParty = new Character[PARTY_LIMIT];    
    }

    public int ActivePartyMemberCount { get {
            for (int i = 0; i < ActiveParty.Length; i++)
                if (ActiveParty[i] == null)
                    return i;
            return 4;
        } }
    public bool IsPartyFull { get { if (ActivePartyMemberCount > PARTY_LIMIT) throw new System.InvalidOperationException("Something went wrong, total party members exceeds limit"); return ActivePartyMemberCount == PARTY_LIMIT; } }
    public bool IsPartyEmpty { get { return ActivePartyMemberCount == 0; } }

    public void SelectCharacter(int slot) {
        activeCharacter = activeParty[slot];
    }

    public bool AddCharacter(Character characterToAdd) {
        if (IsPartyFull) {
            print("Party is already full!");
            return false;
        }

        if (activeParty.Contains(characterToAdd)) {
            print($"{characterToAdd.GetEntityInfo<CharacterInfo>().Name} is already in the party");
            return false;
        }

        for (int i = 0; i < PARTY_LIMIT; i++) {
            if (activeParty[i] == null) {
                activeParty[i] = characterToAdd;

                if (OnPartyChange != null)
                    OnPartyChange(null, characterToAdd);

                return true;
            }
        }

        throw new System.InvalidOperationException("An unknown error has occured, unable to add character to the party");
    }

    public bool RemoveCharacter(Character characterToRemove) {
        if (IsPartyEmpty) {
            print("Party is empty, no characters to remove");
            return false;
        }

        if (ActivePartyMemberCount <= 1) {
            print("Party must have as least 1 member");
            return false;
        }

        if (!activeParty.Contains(characterToRemove)) {
            print($"{characterToRemove.GetEntityInfo<CharacterInfo>().Name} is not currently in the party");
            return false;
        }

        bool removed = false;

        for (int i = 0; i < PARTY_LIMIT; i++) {
            if (activeParty[i] == characterToRemove) {
                activeParty[i] = null;
                removed = true;
            }
        }

        for (int i = 0; i < PARTY_LIMIT; i++) { // since the character removed can be in the middle, shifting the entire array to the left after removal is necessary
            if (activeParty[i] == null && i < PARTY_LIMIT - 1) {
                activeParty[i] = activeParty[i + 1];
                activeParty[i + 1] = null;
            }
        }

        if (removed) {
            if (OnPartyChange != null)
                OnPartyChange(characterToRemove, null);

            return true;
        } else
            throw new System.InvalidOperationException("An unknown error has occured, unable to remove character from the party");
    }

    public bool RemoveCharacter(int tavernID) {
        Character characterToRemove = FindCharacterInParty(tavernID);

        return RemoveCharacter(characterToRemove);
    }

    public Character FindCharacterInParty(int id) {
        return activeParty.Where(character => character.GetEntityInfo<CharacterInfo>().TavernID == id).FirstOrDefault();
    }

    public Character FindCharacterInParty(string name) {
        return ActiveParty.Where(character => character.name.Equals(name)).FirstOrDefault();
    }

    public int GetCharacterIndexInParty(Character character) {
        if (!activeParty.Contains(character))
            throw new InvalidOperationException($"{character.GetEntityInfo<CharacterInfo>().Name} is not currently in the party");

        return Array.IndexOf(activeParty, character);
    }
}
