﻿using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class LureMovement : MonoBehaviour
{
    [ReadOnlyInInspector]
    public float height;
    [ReadOnlyInInspector]
    public float gravityScale = 1f;
    [SerializeField]
    protected float fallDuration = 1.5f;    // s
    [ReadOnlyInInspector]
    public float fallEndTime;    // s
    [ReadOnlyInInspector]
    public bool isFalling = false;
    protected Vector2 previousPosition;
    public Vector2 CurrentPosition { get { return (Vector2)transform.position; } }
    public Vector2 PreviousPosition { get { return previousPosition; } }

    public delegate void LureMovedEventHandler(object source, EventArgs args);
    public event LureMovedEventHandler LureMoved;
    public delegate void LureStoppedFallingEventHandler(object source, EventArgs args);
    public event LureStoppedFallingEventHandler LureStoppedFalling;

    protected Rigidbody2D rigidBody;
    protected Animator animator;

    void Awake()
    {
        this.rigidBody = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        this.gravityScale = rigidBody.gravityScale;
    }

    void OnEnable()
    {
        this.fallEndTime = Time.time + fallDuration;
        this.rigidBody.gravityScale = gravityScale;
        this.isFalling = true;
        this.previousPosition = CurrentPosition;
    }

    void OnDisable()
    {

    }

    void FixedUpdate()
    {
        StopFalling();

        if (transform.hasChanged != true)
        {
            return;
        }

        previousPosition = CurrentPosition;
        MoveLure();
    }

    #region FixedUpdate
    protected void StopFalling()
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

        StopLureFalling();
    }

    protected virtual void MoveLure()
    {
        if (LureMoved == null)
        {
            return;
        }

        LureMoved(this, EventArgs.Empty);
    }

    protected virtual void StopLureFalling()
    {
        if (LureStoppedFalling == null)
        {
            return;
        }

        LureStoppedFalling(this, EventArgs.Empty);
    }
    #endregion FixedUpdate
}