using System;
using UnityEngine;
using StarterAssets;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif
public class ThirdPersonController : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;
    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;
    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;


    [Space(10)]
    [Header("Jump & Grappling")]
    [Tooltip("The height the player can jump")]
    [SerializeField] private float jumpHeight = 1.2f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    [SerializeField] private float gravity = -15.0f;

    [Tooltip("The move speed for grappling")]
    [SerializeField] private float grappleSpeed = 20f;
    [Tooltip("The acceleration to grapple speed")]
    [SerializeField] private float grappleSpeedChangeRate = 5f;
    [Tooltip("Time to rotate towards grapple point")]
    [SerializeField] private float grappleRotationSmoothTime = 0.09f;
    [Tooltip("Extra momentum after letting go of grapple")]
    [SerializeField] private float momentumExtraSpeed = 9f;
    [Tooltip("Extra upwards momentum after letting go of grapple")]
    [SerializeField] private float momentumExtraPushup = 2f;
    [Tooltip("The falloff speed after grappling")]
    [SerializeField] private float momentumDrag = 0.01f;
    [Tooltip("Maximum magnitude of acummulated momentum")]
    [SerializeField] private float maxVelocityMomentum = 1f;

    public bool IsGrappling { get; set; } = false;
    public Vector3 GrapplePoint { get; set; } = Vector3.zero;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;
    [SerializeField] private float terminalSlidingVelocity = 20.0f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;
    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;


    // cinemachine
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;

    // player
    private float speed;
    private float animationBlend;
    private float targetRotation = 0.0f;
    private float rotationVelocity;
    private float verticalVelocity;
    private Vector3 velocityMomentum;
    private float terminalVelocity = 53.0f;
    private bool rotateOnMove = true;
    private Vector3 grappleDir;
    private bool isSliding = false;
    private float slidingVelocity;

    // timeout deltatime
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;

    // animation IDs
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDFreeFall;
    private int animIDMotionSpeed;

    private PlayerInput playerInput;
    private Animator animator;
    private CharacterController controller;
    private StarterAssetsInputs input;
    private GameObject mainCamera;

    private const float threshold = 0.01f;

    private bool hasAnimator;

    // platforms
    private GameObject currentPlatform = null;
    private Vector3 prevPlatformPos = Vector3.zero;
    private Vector3 platformVelocity = Vector3.zero;

    private readonly RaycastHit[] groundHits = new RaycastHit[12];

    private bool IsCurrentDeviceMouse => playerInput.currentControlScheme == "KeyboardMouse";

    private void Awake()
    {
        // get a reference to our main camera
        if (mainCamera == null)
            mainCamera = this.transform.parent.GetComponentInChildren<Camera>().gameObject;
    }

    private void Start()
    {
        hasAnimator = TryGetComponent(out animator);
        controller = GetComponent<CharacterController>();
        input = GetComponent<StarterAssetsInputs>();


        playerInput = GetComponent<PlayerInput>();
        AssignAnimationIDs();

        // reset our timeouts on start
        jumpTimeoutDelta = JumpTimeout;
        fallTimeoutDelta = FallTimeout;

        if (this.gameObject.CompareTag("Player1"))
        {
            GameManager.Instance.SetPlayer1TPC(this);
        }
        else if (this.gameObject.CompareTag("Player2"))
        {
            GameManager.Instance.SetPlayer2TPC(this);
        }
    }

    private void Update()
    {
        Jump();
        Gravity();
        DampenVelocityMomentum();
        GroundedCheck();
        PlatformCheck();
        SlopeCheck();

        if (IsGrappling)
            GrapplingMove();
        else
            Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        // update animator if using character
        if (hasAnimator)
        {
            animator.SetBool(animIDGrounded, Grounded);
        }
    }

    /// <summary>
    /// check if ground is platform, add platform velocity to movement if so
    /// </summary>
    private void PlatformCheck()
    {
        if (Grounded)
        {
            CheckGround();

            for (int i = 0; i < groundHits.Length; i++)
            {
                if (groundHits[i].collider == null || !groundHits[i].collider.gameObject.CompareTag("MagicMoveable"))
                    continue; // next

                if (currentPlatform == null) // We have just stepped on to the platform
                {
                    currentPlatform = groundHits[i].collider.gameObject;
                    prevPlatformPos = currentPlatform.transform.position;
                }
                else // earliest second frame on platform
                {
                    platformVelocity = currentPlatform.transform.position - prevPlatformPos;
                    this.transform.position += platformVelocity;
                    Physics.SyncTransforms();

                    prevPlatformPos = currentPlatform.transform.position;
                }
            }
        }
        else
        {
            if (currentPlatform != null)
            {
                currentPlatform = null;
                prevPlatformPos = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// check if ground is a slope, add sliding to movement if so
    /// </summary>
    private void SlopeCheck()
    {
        if (!Grounded)
            return;

        isSliding = false; // reset
        CheckGround();
        for (int i = 0; i < groundHits.Length; i++)
        {
            if (groundHits[i].collider == null)
                continue;

            float slopeAngle = Mathf.Round(Mathf.Acos(Vector3.Dot(groundHits[i].normal, Vector3.up)) * Mathf.Rad2Deg);
            bool onSlope = slopeAngle >= controller.slopeLimit;

            if (!onSlope)
                continue;
            else
            {
                isSliding = true;

                //calculate a vector that runs across the slope
                Vector3 tangent = Vector3.Cross(groundHits[i].normal, Vector3.up);
                //from that, calculate the direction of steepest descent
                Vector3 slideDir = Vector3.Cross(groundHits[i].normal, tangent);

                this.transform.position += Time.deltaTime * -slidingVelocity * slideDir;
                Physics.SyncTransforms();
            }
        }

    }

    /// <summary>
    /// Does a sphere cast on the ground, saving results in groundHits
    /// </summary>
    private void CheckGround()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z);
        Physics.SphereCastNonAlloc(spherePosition, GroundedRadius, Vector3.down, groundHits, 0.5f, GroundLayers, QueryTriggerInteraction.Ignore);
    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (input.Look.sqrMagnitude >= threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            cinemachineTargetYaw += input.Look.x * deltaTimeMultiplier;
            cinemachineTargetPitch += input.Look.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride, cinemachineTargetYaw, 0.0f);
    }

    private void DampenVelocityMomentum()
    {
        if (velocityMomentum.magnitude > 0.01f)
            velocityMomentum -= velocityMomentum * momentumDrag * Time.deltaTime;
        else
            velocityMomentum = Vector3.zero;
    }

    void GrapplingMove()
    {
        // stop gravity
        verticalVelocity = 0f;

        grappleDir = (GrapplePoint - this.transform.position).normalized;

        // Rotate to face grappleDir
        targetRotation = Quaternion.LookRotation(grappleDir).eulerAngles.y;
        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, grappleRotationSmoothTime);
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

        // Smoothing acceleration
        float currentSpeed = controller.velocity.magnitude;
        float speed = Mathf.Lerp(currentSpeed, grappleSpeed, grappleSpeedChangeRate * Time.deltaTime);
        // round speed to 3 decimal places
        speed = Mathf.Round(speed * 1000f) / 1000f;

        // Move
        controller.Move(grappleDir * (speed * Time.deltaTime));

        // Save momentum
        velocityMomentum = grappleDir * (speed * momentumExtraSpeed * Time.deltaTime) + Vector3.up * momentumExtraPushup;
        Vector3.ClampMagnitude(velocityMomentum, maxVelocityMomentum);
    }

    private void Move()
    {
        float targetSpeed = input.Sprint ? SprintSpeed : MoveSpeed;

        // when falling after grappling
        if (velocityMomentum.sqrMagnitude > 1f)
            targetSpeed = 1f;

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (input.Move == Vector2.zero)
            targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = input.AnalogMovement ? input.Move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, SpeedChangeRate * Time.deltaTime);

            // round speed to 3 decimal places
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }
        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, SpeedChangeRate * Time.deltaTime);

        Vector3 inputDirection = new Vector3(input.Move.x, 0.0f, input.Move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (input.Move != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);

            // rotate to face input direction relative to camera position
            if (rotateOnMove)
            {
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }

        // Move
        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
        controller.Move(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime + velocityMomentum * Time.deltaTime);

        // update animator if using character
        if (hasAnimator)
        {
            animator.SetFloat(animIDSpeed, animationBlend);
            animator.SetFloat(animIDMotionSpeed, inputMagnitude);
        }
    }

    private void Jump()
    {
        //TODO: Make jump bigger based on the time the button is pressed
        if (Grounded)
        {
            velocityMomentum = Vector3.zero;

            // reset the fall timeout timer
            fallTimeoutDelta = FallTimeout;

            // update animator if using character
            if (hasAnimator)
            {
                animator.SetBool(animIDJump, false);
                animator.SetBool(animIDFreeFall, false);
            }

            // Jump, only on frame triggered
            if (input.Jump && !input.HoldingJump && !isSliding && jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                // update animator if using character
                if (hasAnimator)
                {
                    animator.SetBool(animIDJump, true);
                }
            }

            // jump timeout
            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                if (hasAnimator)
                {
                    animator.SetBool(animIDFreeFall, true);
                }
            }
        }

    }

    private void Gravity()
    {
        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // stop our velocity dropping infinitely when grounded
        if (Grounded && verticalVelocity < 0.0f)
        {
            verticalVelocity = -2f;
        }

        if (isSliding && slidingVelocity < terminalSlidingVelocity)
        {
            slidingVelocity += gravity * Time.deltaTime;
        }
        else if(slidingVelocity != -2.0f)
            slidingVelocity = -2.0f;
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }

    public void SetRotateOnMove(bool newRotateOnMove)
    {
        rotateOnMove = newRotateOnMove;
    }
}
