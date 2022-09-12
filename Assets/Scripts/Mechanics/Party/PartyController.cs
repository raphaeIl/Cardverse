using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PartyController : MonoBehaviour {

    public Vector2 WorldPosition { get { return partyMembers[0].transform.position; } }
    public float memberSpacing = 0.7f;
	
    public PartyMember PartyLeader { get { return partyMembers[0]; } }
    public PartyMember[] PartyMembers { get { return partyMembers; } }

    private Party currentParty;
    private PartyMember[] partyMembers;

    void OnValidate() {
        partyMembers = transform.GetComponentsInChildren<PartyMember>();
    }

    // Start is called before the first frame update
    void Awake() {
        partyMembers = transform.GetComponentsInChildren<PartyMember>();
    }

    public void InitParty(Party party) {
        this.currentParty = party;

        UpdatePartyMembers(null, null);
    }

    public void UpdatePartyMembers(Character removedCharacter, Character addedCharacter) {
        if (currentParty == null)
            return;

        for (int i = 0; i < Party.PARTY_LIMIT; i++) {
            partyMembers[i].SetMember(currentParty.ActiveParty[i]);
        }

        AutoFollow();
    }

    public void AutoFollow(bool follow = true) {
        for (int i = Party.PARTY_LIMIT - 1; i > 0; i--) {
            if (follow)
                partyMembers[i].Follow(partyMembers[i - 1], partyMembers[0].GetComponent<CharacterController2D>().speed, memberSpacing); // speed is the leading member's speed which is the one controlled by the player, every other member will have the same speed
            else
                partyMembers[i].Follow(null, partyMembers[0].GetComponent<CharacterController2D>().speed, memberSpacing);
        }
    }

    public void SelectCharacter(int previousSelected, int nowSelected) {
        SwapPosition(GetCharacterSlot(currentParty.ActiveParty[nowSelected]));
    }

    public PartyMember GetCorrespondingPartyMember(Character character) {
        foreach (PartyMember partyMember in partyMembers)
            if (partyMember.CurrentCharacter == character)
                return partyMember;
            
        throw new InvalidOperationException("That character is not currently in the party!");
    }

    private void SwapPosition(int slot) { // swaps the first (previously selected) to the one selected now (slot)
        Character previouslySelected = partyMembers[0].CurrentCharacter;

        partyMembers[0].SetMember(partyMembers[slot].CurrentCharacter);
        partyMembers[slot].SetMember(previouslySelected);
    }

    private int GetCharacterSlot(Character character) {
        for (int i = 0; i < partyMembers.Length; i++)
            if (partyMembers[i].CurrentCharacter == character)
                return i;

        return -1;
    }
}
