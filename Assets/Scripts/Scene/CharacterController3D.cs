using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


public class CharacterController3D : MonoBehaviour 
    {
    public float speed = 1;
    public float acceleration = 2;
    public Vector3 nextMoveCommand;
    public Animator animator;
    public bool flipX = false;

    public float stepSize = 0.1f;

    public SpriteRenderer spriteRenderer;
    PixelPerfectCamera pixelPerfectCamera;

    private new Rigidbody rigidbody;

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
        rigidbody.velocity = Vector3.SmoothDamp(rigidbody.velocity, nextMoveCommand * speed, ref currentVelocity, acceleration, speed);
        spriteRenderer.flipX = rigidbody.velocity.x >= 0 ? true : false;

    }

    void UpdateAnimator(Vector3 direction) {
        if (animator) {
            animator.SetInteger("WalkX", direction.x < 0 ? -1 : direction.x > 0 ? 1 : 0);
            animator.SetInteger("WalkY", direction.z < 0 ? 1 : direction.z > 0 ? -1 : 0);
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
        rigidbody = GetComponent<Rigidbody>();
        pixelPerfectCamera = GameObject.FindObjectOfType<PixelPerfectCamera>();
    }

    void CharacterControl() {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
    
        nextMoveCommand =   input.normalized * stepSize;

        if (Input.GetKeyDown(KeyCode.Space))
            animator.SetTrigger("Jump");
            
    }

    public void DisableAllControls() {
        controlsEnabled = false;
        nextMoveCommand = Vector3.zero;
    }

    public void EnableAllControls() {
        controlsEnabled = true;
    }

}

