using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets {
[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
[RequireComponent(typeof(PlayerInput))]
#endif
public class FirstPersonController : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 4.0f;
    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 6.0f;
    [Tooltip("Rotation speed of the character")]
    public float RotationSpeed = 1.0f;
    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip(
        "Time required to pass before being able to jump again. Set to 0f to instantly jump again"
    )]
    public float JumpTimeout = 0.1f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs"
    )]
    public float FallTimeout = 0.15f;

    [Space(10)]
    [Tooltip("Maximum distance from which the player can interact with an object")]
    public float InteractDistance = 2;
    [Tooltip("Maximum distance from which the player can grab an object")]
    public float GrabDistance = 2;
    [Tooltip(
        "Maximum distance from which the player can hover their mouse over an interactable object"
    )]
    public float HoverDistance = 2;

    [Header("Player Items")]
    [Tooltip("Whether the player can swap items.")]
    public bool SwapItems = true;

    [Header("Player Grounded")]
    [Tooltip(
        "If the character is grounded or not. Not part of the CharacterController built in grounded check"
    )]
    public bool Grounded = true;
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController"
    )]
    public float GroundedRadius = 0.5f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 90.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -90.0f;

    // cinemachine
    private float _cinemachineTargetPitch;

    // player
    private float _speed;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private PlayerInput _playerInput;
    private CharacterController _controller;
    private StarterAssetsInputs _input;
    private GameObject _mainCamera;

    private Camera _cam;
    private Ray _ray;

    private const float _threshold = 0.01f;

    private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";

    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null) {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<StarterAssetsInputs>();
        _playerInput = GetComponent<PlayerInput>();

        _cam = _mainCamera.GetComponent<Camera>();

        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    private void Update()
    {
        if (!GameState.GamePaused) {
            JumpAndGravity();
            GroundedCheck();
            Move();

            DoRaycast();
            Hover();
        }
    }

    private void LateUpdate()
    {
        if (!GameState.GamePaused) {
            CameraRotation();
        }
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(
            transform.position.x, transform.position.y - GroundedOffset, transform.position.z
        );
        Grounded = Physics.CheckSphere(
            spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore
        );
    }

    private void CameraRotation()
    {
        // if there is an input
        if (_input.look.sqrMagnitude >= _threshold) {
            // Don't multiply mouse input by Time.deltaTime
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
            _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

            // clamp our pitch rotation
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Update Cinemachine camera target pitch
            CinemachineCameraTarget.transform.localRotation =
                Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }

    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or
        // iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and
        // is cheaper than magnitude if there is no input, set the target speed to 0
        if (_input.move == Vector2.zero)
            targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed =
            new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset
            || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(
                currentHorizontalSpeed,
                targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate
            );

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        } else {
            _speed = targetSpeed;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and
        // is cheaper than magnitude if there is a move input rotate player when the player is
        // moving
        if (_input.move != Vector2.zero) {
            // move
            inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
        }

        // move the player
        _controller.Move(
            inputDirection.normalized * (_speed * Time.deltaTime)
            + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime
        );
    }

    private void JumpAndGravity()
    {
        if (Grounded) {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f) {
                _verticalVelocity = -2f;
            }

            // Jump
            if (_input.jump && _jumpTimeoutDelta <= 0.0f) {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f) {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        } else {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f) {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            // if we are not grounded, do not jump
            _input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed
        // up over time)
        if (_verticalVelocity < _terminalVelocity) {
            _verticalVelocity += Gravity * Time.deltaTime;
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

        if (Grounded)
            Gizmos.color = transparentGreen;
        else
            Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded
        // collider
        Gizmos.DrawSphere(
            new Vector3(
                transform.position.x, transform.position.y - GroundedOffset, transform.position.z
            ),
            GroundedRadius
        );
    }

    // custom control behaviors below

    public void PauseEventHandler(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (!GameState.GamePaused) {
            GameState.Pause();
        } else {
            GameState.Resume();
        }
    }

    public void NextOrderEventHandler(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        GameObject.Find("OrderList").SendMessage("CycleActiveOrder");
    }

    private void DoRaycast()
    {
        _ray = _cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
    }

    private void Hover()
    {
        RaycastHit hit;
        if (Physics.Raycast(_ray, out hit, HoverDistance)) {
            if (hit.transform.GetComponent<MonoBehaviour>() != null) {
                hit.transform.SendMessage("OnHover", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void InteractEventHandler(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        RaycastHit hit;
        if (Physics.Raycast(_ray, out hit, InteractDistance)) {
            print("I'm looking at " + hit.transform.name);
            // here we'd check for the "Interactable" tag, but that'd get in the way of the
            // other tags so let's not bother
            hit.transform.SendMessage(
                "Interaction", SendMessageOptions.DontRequireReceiver
            ); // fire off the method that makes the object do its thing
        } else {
            print("I'm looking at nothing!");
        }
    }

    public void GrabEventHandler(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        RaycastHit hit;
        if (Physics.Raycast(_ray, out hit, GrabDistance)) // we hit an object
        {
            if (hit.transform.tag == "Holdable") // object is flagged as holdable
            {
                if (Inventory.HeldItem == null) // we aren't holding anything
                {
                    // pick up object
                    Inventory.AddItem(hit.transform.gameObject);
                } else {
                    if (SwapItems == true) {
                        GameObject oldItem = Inventory.RemoveItem();
                        oldItem.transform.position = hit.transform.gameObject.transform.position;
                        oldItem.SetActive(true);
                        Inventory.AddItem(hit.transform.gameObject);
                    } else {
                        print(
                            "Cannot pick up " + hit.transform.name + ", currently holding a "
                            + Inventory.HeldItem.name
                        );
                    }
                }
            } else if (hit.transform.tag == "Container") {
                if (Inventory.HeldItem == null) {
                    hit.transform.SendMessage("Extract");
                } else {
                    hit.transform.SendMessage("Insert");
                }
            } else {
                print(hit.transform.name + " is not grabbable.");
            }
        } else {
            print("I'm reaching for nothing!");
        }
    }
}
}
