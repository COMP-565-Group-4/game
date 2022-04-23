using InputSystem;

using ScriptableObjects;

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class FirstPersonController : MonoBehaviour
{
    [SerializeField]
    private PlayerData data;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 90.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -90.0f;

    // cinemachine
    private float cinemachineTargetPitch;

    // player
    private float speed;
    private float rotationVelocity;
    private float verticalVelocity;
    private float terminalVelocity = 53.0f;

    // timeout deltatime
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;

    private PlayerInput playerInput;
    private CharacterController controller;
    private MovementInputHandler input;
    private GameObject mainCamera;

    private Camera cam;
    private Ray ray;

    private const float THRESHOLD = 0.01f;

    private bool IsCurrentDeviceMouse => playerInput.currentControlScheme == "KeyboardMouse";

    private void Awake()
    {
        // get a reference to our main camera
        if (mainCamera == null) {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<MovementInputHandler>();
        playerInput = GetComponent<PlayerInput>();

        cam = mainCamera.GetComponent<Camera>();

        // reset our timeouts on start
        jumpTimeoutDelta = data.JumpTimeout;
        fallTimeoutDelta = data.FallTimeout;
    }

    private void Update()
    {
        if (!GameState.Instance.GamePaused) {
            JumpAndGravity();
            GroundedCheck();
            Move();

            DoRaycast();
            Hover();
        }
    }

    private void LateUpdate()
    {
        if (!GameState.Instance.GamePaused) {
            CameraRotation();
        }
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(
            transform.position.x, transform.position.y - data.GroundedOffset, transform.position.z
        );
        data.Grounded = Physics.CheckSphere(
            spherePosition, data.GroundedRadius, data.GroundLayers, QueryTriggerInteraction.Ignore
        );
    }

    private void CameraRotation()
    {
        // if there is an input
        if (input.look.sqrMagnitude >= THRESHOLD) {
            // Don't multiply mouse input by Time.deltaTime
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            cinemachineTargetPitch += input.look.y * data.RotationSpeed * deltaTimeMultiplier;
            rotationVelocity = input.look.x * data.RotationSpeed * deltaTimeMultiplier;

            // clamp our pitch rotation
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

            // Update Cinemachine camera target pitch
            CinemachineCameraTarget.transform.localRotation =
                Quaternion.Euler(cinemachineTargetPitch, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * rotationVelocity);
        }
    }

    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = input.sprint ? data.SprintSpeed : data.MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or
        // iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and
        // is cheaper than magnitude if there is no input, set the target speed to 0
        if (input.move == Vector2.zero)
            targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed =
            new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset
            || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            speed = Mathf.Lerp(
                currentHorizontalSpeed,
                targetSpeed * inputMagnitude,
                Time.deltaTime * data.SpeedChangeRate
            );

            // round speed to 3 decimal places
            speed = Mathf.Round(speed * 1000f) / 1000f;
        } else {
            speed = targetSpeed;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and
        // is cheaper than magnitude if there is a move input rotate player when the player is
        // moving
        if (input.move != Vector2.zero) {
            // move
            inputDirection = transform.right * input.move.x + transform.forward * input.move.y;
        }

        // move the player
        controller.Move(
            inputDirection.normalized * (speed * Time.deltaTime)
            + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime
        );
    }

    private void JumpAndGravity()
    {
        if (data.Grounded) {
            // reset the fall timeout timer
            fallTimeoutDelta = data.FallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (verticalVelocity < 0.0f) {
                verticalVelocity = -2f;
            }

            // Jump
            if (input.jump && jumpTimeoutDelta <= 0.0f) {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                verticalVelocity = Mathf.Sqrt(data.JumpHeight * -2f * data.Gravity);
            }

            // jump timeout
            if (jumpTimeoutDelta >= 0.0f) {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        } else {
            // reset the jump timeout timer
            jumpTimeoutDelta = data.JumpTimeout;

            // fall timeout
            if (fallTimeoutDelta >= 0.0f) {
                fallTimeoutDelta -= Time.deltaTime;
            }

            // if we are not grounded, do not jump
            input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed
        // up over time)
        if (verticalVelocity < terminalVelocity) {
            verticalVelocity += data.Gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f)
            lfAngle += 360f;
        if (lfAngle > 360f)
            lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (data.Grounded)
            Gizmos.color = transparentGreen;
        else
            Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded
        // collider
        Gizmos.DrawSphere(
            new Vector3(
                transform.position.x,
                transform.position.y - data.GroundedOffset,
                transform.position.z
            ),
            data.GroundedRadius
        );
    }

    // custom control behaviors below

    private void DoRaycast()
    {
        ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
    }

    private void Hover()
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, data.HoverDistance)) {
            if (hit.transform.GetComponent<MonoBehaviour>() != null) {
                hit.transform.SendMessage("OnHover", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
