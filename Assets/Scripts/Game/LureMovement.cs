using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(Animator))]
public class LureMovement : MonoBehaviour
{
    public SlackTension slackTension;

    [ReadOnlyInInspector]
    public float gravityScale = 1f;
    [SerializeField]
    protected float fallDuration = 1.5f;    // s
    [ReadOnlyInInspector]
    public float fallEndTime;    // s
    [ReadOnlyInInspector]
    public bool isFalling = false;

    public delegate void LureMovedEventHandler(object source, EventArgs args);
    public event LureMovedEventHandler LureMoved;
    public delegate void LureStoppedFallingEventHandler(object source, EventArgs args);
    public event LureStoppedFallingEventHandler LureStoppedFalling;

    protected Rigidbody2D rigidBody;

    void Awake()
    {
        this.rigidBody = GetComponent<Rigidbody2D>();
        //this.animator = GetComponent<Animator>();
        this.gravityScale = rigidBody.gravityScale;
    }

    void OnEnable()
    {
        this.fallEndTime = Time.time + fallDuration;
        this.rigidBody.gravityScale = gravityScale;
        this.isFalling = true;

        // Subscribe to tension
        this.slackTension.TensionChanged += Move;
    }

    void OnDisable()
    {
        // Unsubscribe from tension
        this.slackTension.TensionChanged -= Move;
    }

    void FixedUpdate()
    {
        Fall();

        if(transform.hasChanged != true)
        {
            return;
        }

        OnLureMoved();
    }

    #region FixedUpdate
    protected void Fall()
    {
        if (isFalling != true)
        {
            return;
        }
        if (Time.time < fallEndTime)
        {
            return;
        }

        // Finished falling, stop all movement
        this.rigidBody.velocity = Vector2.zero;
        this.rigidBody.gravityScale = 0f;
        this.isFalling = false;

        OnLureStoppedFalling();
    }

    protected virtual void OnLureMoved()
    {
        if (LureMoved == null)
        {
            return;
        }

        LureMoved(this, EventArgs.Empty);
    }

    protected virtual void OnLureStoppedFalling()
    {
        if(LureStoppedFalling == null)
        {
            return;
        }

        LureStoppedFalling(this, EventArgs.Empty);
    }

    protected void Move(object source, EventArgs args)
    {

    }
    #endregion FixedUpdate
}