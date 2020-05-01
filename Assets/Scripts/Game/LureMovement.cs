using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(Animator))]
public class LureMovement : MonoBehaviour
{
    [ReadOnlyInInspector]
    public float gravityScale = 0.1f;
    [SerializeField]
    protected float fallSpeed = 1;
    [SerializeField]
    protected float fallDuration = 1.5f;    // s
    [ReadOnlyInInspector]
    public float fallEndTime;    // s
    [ReadOnlyInInspector]
    public bool isFalling = false;

    public delegate void LureStoppedEventHandler(object source, EventArgs args);
    public event LureStoppedEventHandler LureStopped;

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
    }

    void FixedUpdate()
    {
        if(isFalling != true)
        {
            return;
        }
        if(Time.time < fallEndTime)
        {
            return;
        }

        // Finished falling, stop all movement
        this.rigidBody.velocity = Vector2.zero;
        this.rigidBody.gravityScale = 0f;
        this.isFalling = false;

        OnLureStopped();
    }

    protected virtual void OnLureStopped()
    {
        if(LureStopped == null)
        {
            return;
        }

        LureStopped(this, EventArgs.Empty);
    }
}