using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    protected float moveSpeed = 5;

    //protected Vector2 direction;
    protected const string DIRECTION = "Direction";

    protected Vector2 movement;
    protected const string MOVEMENT = "Movement";
    protected const string SPEED = "Speed";

    protected Rigidbody2D rigidBody;
    protected Animator animator;

    protected struct Axis
    {
        public const string HORIZONTAL = "Horizontal";
        public const string VERTICAL = "Vertical";
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
        animator.SetFloat($"{MOVEMENT}{SPEED}", movement.sqrMagnitude);

        if(movement.x == 0 && movement.y == 0)
        {
            return;
        }

        //direction.x = movement.x;
        //direction.y = movement.y;
        animator.SetFloat($"{DIRECTION}{Axis.HORIZONTAL}", movement.x);
        animator.SetFloat($"{DIRECTION}{Axis.VERTICAL}", movement.y);
    }

    void FixedUpdate()
    {
        // Movement
        rigidBody.MovePosition(rigidBody.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}