using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


public class CharacterController2D : MonoBehaviour 
    {
    public float speed = 1;
    public float acceleration = 2;
    public Vector3 nextMoveCommand;
    public Animator animator;
    public bool flipX = false;

    public float stepSize = 0.1f;

    new Rigidbody2D rigidbody2D;
    public SpriteRenderer spriteRenderer;
    PixelPerfectCamera pixelPerfectCamera;

    private bool controlsEnabled = true;

    enum State {
        Idle, Moving
    }

    State state = State.Idle;
    Vector3 start, end;
    Vector3 currentVelocity;
    float startTime;
    float distance;

    float velocity;

    void IdleState() {
        if (nextMoveCommand != Vector3.zero) {
            start = transform.position;
            end = start + nextMoveCommand;
            distance = (end - start).magnitude;
            velocity = 0;
            UpdateAnimator(nextMoveCommand);
            nextMoveCommand = Vector3.zero;
            state = State.Moving;
        }
    }

    void MoveState() {

        velocity = Mathf.Clamp01(velocity + Time.deltaTime * acceleration);
        UpdateAnimator(nextMoveCommand);
        rigidbody2D.velocity = Vector3.SmoothDamp(rigidbody2D.velocity, nextMoveCommand * speed, ref currentVelocity, acceleration, speed);
        spriteRenderer.flipX = rigidbody2D.velocity.x >= 0 ? true : false;


    }

    void UpdateAnimator(Vector3 direction) {
        if (animator) {
            animator.SetInteger("WalkX", direction.x < 0 ? -1 : direction.x > 0 ? 1 : 0);
            animator.SetInteger("WalkY", direction.y < 0 && direction.x == 0 ? 1 : direction.y > 0 && direction.x == 0 ? -1 : 0);
        }
    }

    void Update() {
        switch (state) {
            case State.Idle:
                IdleState();
                break;
            case State.Moving:
                MoveState();
                break;
        }

        if (controlsEnabled)
            CharacterControl();

    }

    void LateUpdate() {
        if (pixelPerfectCamera != null) {
            transform.position = pixelPerfectCamera.RoundToPixel(transform.position);
        }
    }

    void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();
        pixelPerfectCamera = GameObject.FindObjectOfType<PixelPerfectCamera>();
    }

    void CharacterControl() {
        Vector3 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        nextMoveCommand = input.normalized * stepSize;

    }

    public void DisableAllControls() {
        controlsEnabled = false;
        nextMoveCommand = Vector3.zero;
    }

    public void EnableAllControls() {
        controlsEnabled = true;
    }

}

