using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    protected float moveSpeed = 5;

    protected Vector2 movement;

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

    void Update()
    {
        // Input
        movement.x = Input.GetAxisRaw(Axis.HORIZONTAL);
        movement.y = Input.GetAxisRaw(Axis.VERTICAL);

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