using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New InteractableEntity Info", menuName = "Entity/Experimental/Generic InteractableEntity")]
public class InteractableEntityInfo : LivingEntityInfo {

    public List<ConversationHandler.DialogueData> conversation; 
}
