using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    protected float moveSpeed = 5;

    protected const string DIRECTION = "Direction";

    public Vector2 FacingDirection { get { return facingDirection; } }
    protected Vector2 facingDirection;
    protected Vector2 movement;
    protected const string MOVEMENT = "Movement";
    protected const string SPEED = "Speed";

    protected Rigidbody2D rigidBody;
    protected Animator animator;
    protected PlayerControls controls;

    protected struct Axis
    {
        public const string HORIZONTAL = "Horizontal";
        public const string VERTICAL = "Vertical";
    }

    void Awake()
    {
        this.rigidBody = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        this.controls = new PlayerControls();

        //this.controls.Gameplay.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        //this.controls.Gameplay.Move.canceled += ctx => movement = Vector2.zero;
    }

    void Update()
    {
        this.movement.x = Input.GetAxisRaw(Axis.HORIZONTAL);
        this.movement.y = Input.GetAxisRaw(Axis.VERTICAL);

        animator.SetFloat($"{MOVEMENT}{SPEED}", movement.sqrMagnitude);

        if (movement.x == 0 && movement.y == 0)
        {
            return;
        }

        float facingX = Mathf.Ceil(movement.x);
        float facingY = Mathf.Ceil(movement.y);
        this.facingDirection = new Vector2(facingX, facingY);

        animator.SetFloat($"{DIRECTION}{Axis.HORIZONTAL}", movement.x);
        animator.SetFloat($"{DIRECTION}{Axis.VERTICAL}", movement.y);
    }

    void FixedUpdate()
    {
        // Movement
        rigidBody.MovePosition(rigidBody.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}