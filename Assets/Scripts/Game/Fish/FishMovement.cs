using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class FishMovement : MonoBehaviour
{
    [SerializeField]
    protected float moveSpeed = 5;
    [SerializeField]
    protected float maxStamina = 100;
    [SerializeField]
    protected float maxStrength = 10;

    protected Vector2 movement;
    protected float currentStamina;

    protected Rigidbody2D rigidBody;
    protected Animator animator;

    protected struct Axis
    {
        public const string HORIZONTAL = "Horizontal";
        public const string VERTICAL = "Vertical";
    }

    protected const string MOVEMENT = "Movement";
    protected struct Movement
    {
        public const string SPEED = "Speed";
    }

    void Awake()
    {
        this.rigidBody = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        this.currentStamina = maxStamina;
    }

    void Update()
    {
        animator.SetFloat($"{MOVEMENT}{Axis.HORIZONTAL}", movement.x);
        animator.SetFloat($"{MOVEMENT}{Axis.VERTICAL}", movement.y);
        animator.SetFloat($"{MOVEMENT}{Movement.SPEED}", movement.sqrMagnitude);
    }

    void FixedUpdate()
    {
        // Movement
        rigidBody.MovePosition(rigidBody.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}