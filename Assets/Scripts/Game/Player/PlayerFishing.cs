using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerFishing : MonoBehaviour
{
    [SerializeField]
    protected float threshold = 0.05f;

    [ReadOnlyInInspector]
    public bool isFishing = false;

    [SerializeField]
    protected Rigidbody2D rodTip;
    [SerializeField]
    protected float rodTipSpeed = 5;
    [SerializeField]
    protected Rigidbody2D lure;
    protected LureMovement lureMovement;
    [SerializeField]
    protected FishingLine fishingLine;
    public SlackTension slackTension;

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
    [SerializeField]
    protected Vector2 castingRightLureOffset = new Vector2(0.4f, -0.1f);

    [ReadOnlyInInspector]
    public Vector2 currentRodTipPosition;
    [ReadOnlyInInspector]
    public Vector2 previousRodTipPosition;

    [ReadOnlyInInspector]
    public bool isAngling = false;
    [ReadOnlyInInspector]
    public float currentReelRotationVelocity = 0;  // + out, - in
    [ReadOnlyInInspector]
    public float previousReelRotationSpeed = 0;
    [ReadOnlyInInspector]
    public float reelIn;
    [ReadOnlyInInspector]
    public float reelOut;
    [ReadOnlyInInspector]
    public float lineOut;
    [SerializeField]
    protected float lineLength = 25;
    [SerializeField]
    protected float reelingSpeed = 1;
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
        this.animator = GetComponent<Animator>();

        this.controls = new PlayerControls();
        this.controls.Gameplay.ToggleItem.performed += ctx => ToggleFishing();

        this.playerMovement = GetComponent<PlayerMovement>();

        this.lureMovement = lure.gameObject.GetComponent<LureMovement>();
    }

    void OnEnable()
    {
        this.controls.Gameplay.Enable();

        if (slackTension == null)
        {
            return;
        }

        // Subscribe to tension
        this.slackTension.TensionChanged += MoveLureTowardPlayer;
    }

    void OnDisable()
    {
        this.controls.Gameplay.Disable();

        // Unsubscribe from tension
        this.slackTension.TensionChanged -= MoveLureTowardPlayer;
    }

    public void MoveLureTowardPlayer(object source, EventArgs args)
    {
        Vector2 direction = transform.position - lure.transform.position;   // destination - origin
        direction = direction.normalized;

        float tension = slackTension.Tension;
        Vector2 force = direction * tension;

        lure.AddForce(force);
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
        }
        else
        {
            float lineTension = 0;

            float lineSlack = slackTension.Slack;
            if (lineSlack < 0)
            {
                lineTension = -lineSlack;
            }

            HandleReelingIn(lineTension);
            HandleReelingOut(lineTension);

            float deltaReel = reelIn + reelOut;

            // Stop reel velocity
            if (2 - deltaReel < threshold)
            {
                this.currentReelRotationVelocity = 0;
            }

            // Slow reel velocity
            if (deltaReel < threshold)
            {
                float deltaReelVelocity = Mathf.Clamp(reelDrag, reelDrag, -currentReelRotationVelocity);
                if (currentReelRotationVelocity > 0)
                {
                    deltaReelVelocity = Mathf.Clamp(-reelDrag, -currentReelRotationVelocity, -reelDrag);
                }

                this.currentReelRotationVelocity += deltaReelVelocity;
            }

            float delta = Mathf.Clamp(currentReelRotationVelocity * Time.fixedDeltaTime, -lineOut, lineLength - lineOut);
            this.lineOut += delta;

            this.slackTension.ChangeSlackTension(delta);

            //MoveRod();

            //float lineChange = currentReelRotationSpeed * Time.fixedDeltaTime;
            //lineOut = Mathf.Clamp(lineOut + lineChange, 0, lineLength);
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
        if (hasReleasedCast == true)
        {
            return;
        }

        this.hasReleasedCast = true;
        animator.SetBool(Trigger.HAS_RELEASED_CAST, hasReleasedCast);

        // Set lure starting position
        lure.transform.position = rodTip.position + castingRightLureOffset;

        lure.gameObject.SetActive(true);

        // Subscribe to lure falling
        lureMovement.LureStoppedFalling += Angling;

        // Accelerate lure
        Vector2 castDirection = new Vector2(rodMovement.x * castingSpeed, rodMovement.y + 0.25f * castingSpeed);
        Vector2 lureVelocity = castDirection;
        lure.velocity = lureVelocity;

        // Set fishing line slack
        this.slackTension.SetSlackTension(0);

        fishingLine.gameObject.SetActive(true);

        // Subscribe to lure movement
        lureMovement.LureMoved += fishingLine.OnLureMoved;
    }

    protected void SpoolOutLine()
    {
        if (hasReleasedCast != true)
        {
            return;
        }

        float distance = Vector2.Distance(rodTip.position, lure.position);
        this.lineOut = distance;
    }

    protected void SampleCast(Vector2 casting)
    {
        // TODO
    }

    public void Angling(object source, EventArgs args)
    {
        if (isAngling == true)
        {
            return;
        }

        this.isAngling = true;
        animator.SetBool(Trigger.IS_ANGLING, isAngling);

        // Unsubscribe from lure falling
        this.lureMovement.LureStoppedFalling -= Angling;
    }

    protected void HandleReelingIn(float lineTension)
    {
        if (reelIn < reelOut)
        {
            return;
        }

        //float reelRotationSpeed = currentReelRotationSpeed - previousReelRotationSpeed;

        float reelInSpeed = (-reelIn + reelOut) * reelingSpeed - Mathf.Clamp(lineTension - reelingStrength, 0, lineTension);
        currentReelRotationVelocity += reelInSpeed;

        //previousReelRotationSpeed = currentReelRotationSpeed;
        //currentReelRotationSpeed = reelRotationSpeed - reelInSpeed;
    }

    protected void HandleReelingOut(float lineTension)
    {
        if (reelOut < reelIn)
        {
            return;
        }

        //float reelRotationSpeed = currentReelRotationSpeed - previousReelRotationSpeed;

        float reelOutSpeed = (reelOut - reelIn) * reelingSpeed + Mathf.Clamp(lineTension, 0, lineTension);
        currentReelRotationVelocity += reelOutSpeed;

        //previousReelRotationSpeed = currentReelRotationSpeed;
        //currentReelRotationSpeed = reelRotationSpeed + reelOutSpeed;
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