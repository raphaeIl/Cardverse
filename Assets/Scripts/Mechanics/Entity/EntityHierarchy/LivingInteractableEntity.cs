using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConversationHandler))]
public class LivingInteractableEntity : LivingEntity, IInteractable {
    
    private InteractableEntityInfo interactableEntityInfo;

    private ConversationHandler conversation;

    protected override void Start() {
        base.Start();

        conversation = GetComponent<ConversationHandler>();

        if (conversation != null)
            conversation.SetConversation(GetEntityInfo<InteractableEntityInfo>().conversation);

        if (this is Character)
            if (GetEntityInfo<CharacterInfo>().Name.Equals("Sebastian"))
                StartConversation();

    }

    protected override void Update() {
        base.Update();
        if (Input.GetKeyDown(GameMaster.Instance.GameSettings.GetControlKey(ControlType.Interact)))
            this.InteractWith(interactableCollided);
    }

    public override void SetEntityInfo(LivingEntityInfo livingEntityInfo) {
        interactableEntityInfo = (InteractableEntityInfo)livingEntityInfo;
    }

    public override T GetEntityInfo<T>() {
        return (T)(LivingEntityInfo)interactableEntityInfo;
    }


    private LivingInteractableEntity interactableCollided;
    private bool canInteract;

    public void InteractWith(LivingInteractableEntity interactWith) {
        interactWith.StartConversation();
    }

    public void StartConversation() {
        Vector2 dialogueLocation = (Vector2)WorldInfo.transform.position + Vector2.up * 1f;

        DialogueUI dialogueUI = Instantiate(GameMaster.Instance.GameAssets.pfDialogue, dialogueLocation, Quaternion.identity).GetComponent<DialogueUI>();

        dialogueUI.UpdateDialogueUI(this, conversation);
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        interactableCollided = collision.gameObject.GetComponent<LivingInteractableEntity>();

        if (interactableCollided != null)
            canInteract = true;
    }

}
