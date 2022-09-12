using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Entity/Character")]
public class CharacterInfo : InteractableEntityInfo {

    public CharacterArchetype CharacterArchetype;

    public List<Skill> Skills;
    
    public int TavernID { get; set; } // keeping track of the characters using their tavern position (only will be set at character creation + adding to tavern, order same as creation)

    public override bool Equals(object obj) {
        if (!(obj is CharacterInfo))
            return false;
        return TavernID == ((CharacterInfo)obj).TavernID;
    }
}
