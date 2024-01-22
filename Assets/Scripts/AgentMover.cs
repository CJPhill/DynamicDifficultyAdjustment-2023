using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMover : MonoBehaviour
{

    private Rigidbody2D rb2d;

    [SerializeField]
    private float maxSpeed = 2, acceleration = 50, deacceleration = 100;
    [SerializeField]
    private float currentSpeed = 0;
    private Vector2 oldMovementInput;
    public Vector2 MovementInput { get; set; }
    private Animator animator;

    private void Awake() {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate() {
        if (MovementInput.magnitude > 0 && currentSpeed >= 0)
        {
            oldMovementInput = MovementInput;
            currentSpeed += acceleration * maxSpeed * Time.deltaTime;
            acceleration = 50;
        }
        else
        {
            currentSpeed -= deacceleration * maxSpeed * Time.deltaTime;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Slash1")){
            currentSpeed = 0.0f;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Bow")){
            currentSpeed = 0.0f;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Throw")){
            currentSpeed = 0.0f;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Roll")){
            acceleration = 100;
        }
        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        rb2d.velocity = oldMovementInput * currentSpeed;
        //Debug.Log(MovementInput);
    }
}
