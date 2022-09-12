using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PartyMember : MonoBehaviour {

    public SpriteRenderer selectionCircle;

    public Character CurrentCharacter { get { return currentCharacter; } }
    private Character currentCharacter;

    private float moveSpeed;
    private float memberSpacing;

    private Animator animator;

    private PartyMember followingMember;

    private Vector3 lastPosition;

    void Start() {
        animator = GetComponent<Animator>();
    }

    void Update() {
        if (currentCharacter != null)
            if (currentCharacter.IsMouseOver && Input.GetMouseButtonDown(0))
                currentCharacter.OnMouseClick();
            

        if (followingMember == null) {
            if (GetComponent<CharacterController2D>() == null)
                UpdateAnimator(Vector3.zero);

            return;
        }
        transform.position = Vector2.Lerp(transform.position, followingMember.transform.position - (followingMember.transform.position - transform.position).normalized * memberSpacing, Time.deltaTime * moveSpeed);

        UpdateAnimator((transform.position - lastPosition).normalized);

        lastPosition = transform.position;
    }

    void OnMouseEnter() {
        if (currentCharacter != null)
            currentCharacter.IsMouseOver = true;
    }

    void OnMouseExit() {
        if (currentCharacter != null)
            currentCharacter.IsMouseOver = false;
    }

    public void SetMember(Character character) {
        currentCharacter = character;

        UpdateWorldPlayer();
        GetComponentInChildren<ParticleSystem>().Play();
    }

    public void Follow(PartyMember partyMember, float moveSpeed, float memberSpacing) {
        followingMember = partyMember;
        this.memberSpacing = memberSpacing;
        this.moveSpeed = moveSpeed;
    }

    public void UpdateWorldPlayer() {
        SpriteRenderer characterGfx = transform.GetChild(0).GetComponent<SpriteRenderer>();

        for (int i = 0; i < 3; i++) // disabling the graphics, shadow and light of the partyMember if the current character is null
            transform.GetChild(i).gameObject.SetActive(currentCharacter != null);

        if (currentCharacter == null)
            return;

        if (currentCharacter.GetEntityInfo<CharacterInfo>().DisplaySprite != null) {
            characterGfx.sprite = currentCharacter.GetEntityInfo<CharacterInfo>().DisplaySprite;
            characterGfx.color = Color.white;
        } else characterGfx.color = Color.black;

        if (GetComponent<CharacterController2D>() != null)
            GetComponent<CharacterController2D>().spriteRenderer = characterGfx;
    }

    public void UpdateAnimator(Vector3 direction) {
        animator.SetInteger("WalkX", direction.x < 0f && Mathf.Abs(direction.y) < 0.4f ? -1 : direction.x > 0f && Mathf.Abs(direction.y) < 0.4f ? 1 : 0);
        animator.SetInteger("WalkY", direction.y < 0 && Mathf.Abs(direction.x) < 0.4f ? 1 : direction.y > 0 && Mathf.Abs(direction.x) < 0.4f ? -1 : 0);

        if (Mathf.Abs(direction.y) <= 0.5f) {
            transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = direction.x > 0;
        } 
    }

    public void ConstrainAllMovement() {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void UnConstrainAllMovement() {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void ShowSelectionCircle() {
        selectionCircle.gameObject.SetActive(true);
    }

    public void HideSelectionCircle() {
        selectionCircle.gameObject.SetActive(false);
    }
}
