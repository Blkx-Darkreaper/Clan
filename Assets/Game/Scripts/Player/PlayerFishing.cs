using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerFishing : MonoBehaviour
{
    public static PlayerFishing singleton = null;

    [SerializeField]
    protected float threshold = 0.05f;

    [ReadOnlyInInspector]
    public bool isFishing = false;

    [SerializeField]
    protected Rigidbody2D rodTip;
    [SerializeField]
    protected float rodTipSpeed = 5;
    [SerializeField]
    protected float rodTipHeight;
    [SerializeField]
    protected Rigidbody2D lure;
    [SerializeField]
    protected Transform lureKnot;
    protected LureMovement lureMovement;
    [SerializeField]
    protected FishingLine fishingLine;

    [ReadOnlyInInspector]
    public bool isReadyToCast = false;

    [SerializeField]
    protected float rodSpeed = 0.05f;
    [ReadOnlyInInspector]
    public Vector2 rodMovement;
    [ReadOnlyInInspector]
    public float rodVelocity;
    [ReadOnlyInInspector]
    public Vector2 rodPosition;
    [ReadOnlyInInspector]
    public bool isHoldingLine = false;
    [ReadOnlyInInspector]
    public bool hasReleasedCast = false;
    [ReadOnlyInInspector]
    public LinkedList<Vector2> allCastingSamples;
    [SerializeField]
    protected float castingSpeed = 500f;

    [ReadOnlyInInspector]
    public Vector2 currentRodTipPosition;
    [ReadOnlyInInspector]
    public Vector2 previousRodTipPosition;

    [ReadOnlyInInspector]
    public bool isAngling = false;
    [ReadOnlyInInspector]
    public float reelIn;
    [ReadOnlyInInspector]
    public float reelOut;
    [ReadOnlyInInspector]
    public float currentReelRotationVelocity = 0;  // + out, - in
    [ReadOnlyInInspector]
    public float slack;
    public delegate void SlackUpdatedEventHandler(object source, EventArgs args);
    public event SlackUpdatedEventHandler SlackUpdated;
    [ReadOnlyInInspector]
    public float lineOut;
    [SerializeField]
    protected float lineLength = 25;
    [SerializeField]
    protected float lineOutResetThreshold = 0.45f;
    [SerializeField]
    protected float reelingSpeed = 1;
    [SerializeField]
    protected float maxReelRotationSpeed = 5;
    [SerializeField]
    protected float reelDrag = 0.1f;
    [SerializeField]
    protected float reelingStrength = 1;

    [ReadOnlyInInspector]
    public bool hasBite = false;
    [ReadOnlyInInspector]
    public float hookBite;
    [ReadOnlyInInspector]
    public FishMovement fish;

    protected const string ROD = "Rod";
    protected const string MOVEMENT = "Movement";
    protected const string SPEED = "Speed";
    protected const string POSITION = "Position";

    protected Animator animator;
    protected PlayerControls controls;
    protected PlayerMovement playerMovement;

    protected struct Axis
    {
        public const string HORIZONTAL = "Horizontal";
        public const string VERTICAL = "Vertical";
        public const string LEFT_STICK_X = "Horizontal";
        public const string LEFT_STICK_Y = "Vertical";
        //public const string RIGHT_STICK_X = "joystickbutton4";
        //public const string RIGHT_STICK_Y = "joystickbutton5";
        public const string RIGHT_STICK_X = "Horizontal";
        public const string RIGHT_STICK_Y = "Vertical";
        public const string DPAD_X = "joystickbutton6";
        public const string DPAD_Y = "joystickbutton7";
        public const string LEFT_TRIGGER = "joystickbutton9";
        public const string RIGHT_TRIGGER = "joystickbutton10";
    }
    protected struct Direction
    {
        public const string X = "X";
        public const string Y = "Y";
    }
    protected struct Button
    {
        public const KeyCode A = KeyCode.Joystick1Button0;
        public const KeyCode B = KeyCode.Joystick1Button1;
        public const KeyCode X = KeyCode.Joystick1Button2;
        public const KeyCode Y = KeyCode.Joystick1Button3;
        public const KeyCode LEFT_BUMPER = KeyCode.Joystick1Button4;
        public const KeyCode RIGHT_BUMPBER = KeyCode.Joystick1Button5;
        public const KeyCode BACK = KeyCode.Joystick1Button6;
        public const KeyCode START = KeyCode.Joystick1Button7;
        public const KeyCode LEFT_STICK_CLICK = KeyCode.Joystick1Button8;
        public const KeyCode RIGHT_STICK_CLICK = KeyCode.Joystick1Button9;
    }

    protected struct Verb
    {
        public static string action = "Fire1";
        public static string cast = "Fire2";
        public static string reelIn = "LeftTrigger";
        public static string reelOut = "RightTrigger";
    }

    protected struct Trigger
    {
        public const string IS_FISHING = "IsFishing";
        public const string IS_HOLDING = "IsHolding";
        public const string HAS_RELEASED_CAST = "HasReleasedCast";
        public const string IS_ANGLING = "IsAngling";
        public const string ALL_REELED_IN = "AllReeledIn";
    }

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }

        if (this != singleton)
        {
            Destroy(this);
            return;
        }

        this.animator = GetComponent<Animator>();

        this.controls = new PlayerControls();
        this.controls.Gameplay.ToggleItem.performed += ctx => ToggleFishing();

        this.playerMovement = GetComponent<PlayerMovement>();

        this.lureMovement = lure.gameObject.GetComponent<LureMovement>();
    }

    void OnEnable()
    {
        this.controls.Gameplay.Enable();

        // Subscribe to Lure movement
        this.lureMovement.LureMoved += OnLineOutUpdated;
    }

    void OnDisable()
    {
        this.controls.Gameplay.Disable();

        // Unsubscribe to Lure movement
        this.lureMovement.LureMoved -= OnLineOutUpdated;
    }

    public void OnLineOutUpdated(object source, EventArgs arg)
    {
        //if(slack < threshold)
        //{
        //    return;
        //}

        float distance = Vector2.Distance(rodTip.position, lureKnot.position);
        float updatedLineOut = distance + slack;

        float diff = Mathf.Abs(updatedLineOut - lineOut);
        if (diff < threshold)
        {
            return;
        }

        this.lineOut = updatedLineOut;
    }

    public void OnLureMoved(object source, EventArgs args)
    {
        UpdateSlack(0);
    }

    public void OnAngling(object source, EventArgs args)
    {
        if (isAngling == true)
        {
            return;
        }

        this.isAngling = true;
        animator.SetBool(Trigger.IS_ANGLING, isAngling);

        // Unsubscribe from lure falling
        this.lureMovement.LureStoppedFalling -= OnAngling;
    }

    protected virtual void UpdateSlack(float delta)
    {
        float updatedSlack = slack + delta;

        float distance = Vector2.Distance(rodTip.position, lureKnot.position);
        float maxSlack = lineLength - distance;

        updatedSlack = Mathf.Clamp(updatedSlack, 0, maxSlack);

        SetSlack(updatedSlack);
    }

    protected virtual void SetSlack(float updatedSlack)
    {
        this.slack = updatedSlack;

        if (SlackUpdated == null)
        {
            return;
        }

        SlackUpdated(this, EventArgs.Empty);
    }

    void Update()
    {
        // Input
        if (isFishing != true)
        {
            return;
        }

        GetRodMovement();

        this.controls.Gameplay.ItemPrimary.performed += ctx => reelIn = ctx.ReadValue<float>();
        this.controls.Gameplay.ItemPrimary.canceled += ctx => reelIn = 0f;

        this.controls.Gameplay.ItemSecondary.performed += ctx => reelOut = ctx.ReadValue<float>();
        this.controls.Gameplay.ItemSecondary.canceled += ctx => reelOut = 0f;

        //StartCastingWithButton();

        //CastLureWithButton();

        //StartCasingWithStick();

        //CastLureWithStick();

        if (isAngling != true)
        {
            return;
        }

        //reelIn = Input.GetAxisRaw(Verb.reelIn);

        //reelOut = Input.GetAxisRaw(Verb.reelOut);
    }

    #region Update
    protected void GetRodMovement()
    {
        float lineHold = reelIn + reelOut;
        float holdDiff = Mathf.Abs(2f - lineHold);
        this.isHoldingLine = holdDiff <= threshold;

        this.rodMovement.x = Input.GetAxisRaw(Axis.RIGHT_STICK_X);
        this.rodMovement.y = Input.GetAxisRaw(Axis.RIGHT_STICK_Y);

        this.rodVelocity = rodMovement.sqrMagnitude;
        animator.SetFloat($"{MOVEMENT}{SPEED}", rodVelocity);

        if (rodVelocity > 0 && rodMovement.x == 0 && rodMovement.y == 0)
        {
            return;
        }

        animator.SetFloat($"{ROD}{Axis.HORIZONTAL}", rodMovement.x);
        animator.SetFloat($"{ROD}{Axis.VERTICAL}", rodMovement.y);
    }

    protected void ToggleFishing()   // A
    {
        if (hasBite == true)
        {
            return;
        }
        if(isAngling == true)
        {
            return;
        }

        isFishing = !isFishing;
        animator.SetBool(Trigger.IS_FISHING, isFishing);

        playerMovement.enabled = !isFishing;
    }

    protected void StartCastingWithButton()   // B
    {

    }

    protected void CastLureWithButton()   // B
    {

    }
    #endregion Update

    void FixedUpdate()
    {
        // Movement
        if (isFishing != true)
        {
            return;
        }

        float rodPositionX = rodPosition.x;

        if (isAngling != true)
        {
            // Casting
            rodPositionX = WindupRod(rodPositionX);
            rodPosition.x = rodPositionX;

            animator.SetFloat($"{ROD}{POSITION}{Direction.X}", rodPosition.x);

            CheckIsReadyToCast(rodPositionX);

            SampleCast(rodMovement);

            ReleaseCast(rodPositionX);

            SpoolOutLine();
        }
        else
        {
            float lineTension = 0;

            HandleReelingIn(lineTension);
            HandleReelingOut(lineTension);

            this.currentReelRotationVelocity = Mathf.Clamp(currentReelRotationVelocity, -maxReelRotationSpeed, maxReelRotationSpeed);

            float deltaReel = reelIn + reelOut;

            // Stop reel rotation
            if (2 - deltaReel < threshold)
            {
                this.currentReelRotationVelocity = 0;
            }

            // Slow reel rotation
            float currentReelRotationSpeed = Mathf.Abs(currentReelRotationVelocity);
            if (deltaReel < threshold && currentReelRotationSpeed > 0)
            {
                // Reeling in
                float deltaReelVelocity = Mathf.Max(currentReelRotationVelocity, reelDrag);

                // Reeling out
                if (currentReelRotationVelocity > 0)
                {
                    deltaReelVelocity = Mathf.Max(-currentReelRotationVelocity, -reelDrag);
                }

                this.currentReelRotationVelocity += deltaReelVelocity;
            }

            this.currentReelRotationVelocity = Mathf.Clamp(currentReelRotationVelocity, -maxReelRotationSpeed, maxReelRotationSpeed);

            float delta = Mathf.Clamp(currentReelRotationVelocity * Time.fixedDeltaTime, -lineOut, lineLength - lineOut);
            this.lineOut += delta;

            UpdateSlack(delta);

            if (currentReelRotationVelocity < 0)
            {
                lineTension = -currentReelRotationVelocity;
            }
            MoveLureTowardPlayer(lineTension);

            //MoveRod();

            ResetFishing();
        }
    }

    #region FixedUpdate
    protected float WindupRod(float rodPositionX)
    {
        if (hasReleasedCast == true)
        {
            return rodPositionX;
        }
        //if(isHolding == true)
        //{
        //    return rodPositionX;
        //}

        if (rodVelocity > 0)
        {
            rodPositionX += rodMovement.x * rodSpeed;
        }

        // Settle back to idle if no input
        if (rodVelocity < threshold && Mathf.Abs(rodMovement.x) < threshold)
        {
            if (rodPositionX > 0)
            {
                rodPositionX -= rodSpeed;
            }
            else
            {
                rodPositionX += rodSpeed;
            }
        }

        rodPositionX = Mathf.Clamp(rodPositionX, -1, 1);
        return rodPositionX;
    }

    protected void CheckIsReadyToCast(float rodPositionX)
    {
        //if (isReleasingCast == true)
        //{
        //    return;
        //}
        if (isReadyToCast == true)
        {
            return;
        }

        float windupDiff = Mathf.Abs(1f - Mathf.Abs(rodPositionX));
        if (windupDiff > threshold)
        {
            return;
        }

        this.isReadyToCast = true;
    }

    protected void ReleaseCast(float rodPositionX)
    {
        if (hasReleasedCast == true)
        {
            return;
        }
        if (rodVelocity < threshold)
        {
            return;
        }
        if (isHoldingLine == true)
        {
            return;
        }
        if (rodPositionX < threshold)
        {
            return;
        }
        if (rodMovement.x < 0)
        {
            return;
        }

        this.isReadyToCast = false;
        this.hasReleasedCast = true;
        animator.SetBool(Trigger.HAS_RELEASED_CAST, hasReleasedCast);

        // Set lure starting position
        Vector2 lureOffset = lure.position - (Vector2)lureKnot.position;
        lure.transform.position = rodTip.position + lureOffset;

        lure.gameObject.SetActive(true);

        // Subscribe to lure falling
        lureMovement.LureStoppedFalling += OnAngling;

        // Accelerate lure
        Vector2 castDirection = new Vector2(rodMovement.x * castingSpeed, rodMovement.y + 0.25f * castingSpeed);
        Vector2 lureVelocity = castDirection;
        lure.velocity = lureVelocity;

        // Set fishing line slack
        SetSlack(0);

        fishingLine.gameObject.SetActive(true);

        // Subscribe to lure movement
        lureMovement.LureMoved += OnLureMoved;
    }

    protected void SpoolOutLine()
    {
        if (hasReleasedCast != true)
        {
            return;
        }

        this.currentReelRotationVelocity = maxReelRotationSpeed;

        float distance = Vector2.Distance(rodTip.position, lureKnot.position);
        distance = Mathf.Clamp(distance, 0, lineLength);

        this.lineOut = distance;
    }

    protected void SampleCast(Vector2 casting)
    {
        // TODO
    }

    protected void HandleReelingIn(float lineTension)
    {
        if (reelIn <= reelOut)
        {
            return;
        }

        float reelInSpeed = (-reelIn + reelOut) * reelingSpeed;
        //float reelInSpeed = (-reelIn + reelOut) * reelingSpeed - Mathf.Clamp(lineTension - reelingStrength, 0, lineTension);
        currentReelRotationVelocity += reelInSpeed;
    }

    protected void HandleReelingOut(float lineTension)
    {
        if (reelOut <= reelIn)
        {
            return;
        }

        float reelOutSpeed = (reelOut - reelIn) * reelingSpeed;
        //float reelOutSpeed = (reelOut - reelIn) * reelingSpeed + Mathf.Clamp(lineTension, 0, lineTension);
        currentReelRotationVelocity += reelOutSpeed;
    }

    protected virtual void MoveLureTowardPlayer(float lineTension)
    {
        if (lureMovement.isFalling == true)
        {
            return;
        }

        Vector2 force = Vector2.zero;
        if (lineTension < threshold)
        {
            lure.AddForce(force);
            return;
        }

        Vector2 direction = rodTip.position - (Vector2)lureKnot.position;   // destination - origin
        float directionX = direction.x;
        float directionY = direction.y;

        Vector2 normalizedDirection = direction.normalized;

        // Move lure horizontally until it's beneath the rod tip
        float absDirX = Mathf.Abs(directionX);
        //if (absDirX > threshold)
        if (lineOut > rodTipHeight)
        {
            // Normalize x direction
            normalizedDirection.x = directionX / absDirX;

            normalizedDirection.y = 0;
        }

        force = normalizedDirection * lineTension;
        lure.AddForce(force);

        //lure.MovePosition(endPosition);
    }

    protected virtual void ResetFishing()
    {
        if(lineOut > lineOutResetThreshold)
        {
            return;
        }

        this.isAngling = false;
        animator.SetBool(Trigger.IS_ANGLING, isAngling);

        this.hasReleasedCast = false;
        animator.SetBool(Trigger.HAS_RELEASED_CAST, hasReleasedCast);

        animator.SetTrigger(Trigger.ALL_REELED_IN);

        lure.gameObject.SetActive(false);
        fishingLine.gameObject.SetActive(false);
    }

    protected void HandleTension()
    {
        float lineResistance = (reelOut + reelIn) / 2f * reelingStrength;

        // TODO
    }

    protected void MoveRod()
    {
        Vector2 velocity = currentRodTipPosition - previousRodTipPosition;

        previousRodTipPosition = currentRodTipPosition;
        currentRodTipPosition += velocity;

        // TODO: reduce rod tip speed if in lateral direction
        Vector2 acceleration = rodMovement * rodTipSpeed * Time.fixedDeltaTime;
        currentRodTipPosition += acceleration;

        rodTip.MovePosition(currentRodTipPosition);

        // TODO: add constraint of how far rod tip can move away from player
    }
    #endregion FixedUpdate
}