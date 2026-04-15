using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;
    public float stamina = 5f; // seconds of sprinting available
    [Tooltip("How fast stamina drains while sprinting (points per second). 1 = 1 second of stamina per second")] public float staminaDrainRate = 1f;
    [Tooltip("How fast stamina regenerates when not sprinting (points per second)")] public float staminaRegenRate = 0.75f;
    [Tooltip("Minimum stamina required to start sprinting") ] public float minStaminaToSprint = 0.1f;
    [HideInInspector]
    [Tooltip("Current stamina (visible at runtime)")]
    public float currentStamina;
    [Tooltip("Delay (seconds) before stamina starts regenerating after stopping sprinting")] public float staminaRegenDelay = 1f;
    float regenDelayTimer = 0f;
    [Header("UI")]
    [Tooltip("Optional UI Slider to display current stamina")]
    public Slider staminaSlider;

    [Header("Look")]
    public Transform playerCamera; // assign main camera transform
    public float lookSensitivity = 0.1f;
    public float maxLookAngle = 85f;
    [Tooltip("Smoothing time for camera rotation. Lower = snappier")] public float lookSmoothTime = 0.02f;

    CharacterController controller;
    PlayerInput playerInput;

    InputAction moveAction;
    InputAction lookAction;
    InputAction jumpAction;
    InputAction sprintAction;

    Vector3 velocity;
    float cameraPitch = 0f;
    // displayed (smoothed) camera pitch and its smoothing velocity
    float displayedCameraPitch = 0f;
    float pitchSmoothVelocity = 0f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        if (playerInput != null && playerInput.actions != null)
        {
            moveAction = playerInput.actions["Move"];
            lookAction = playerInput.actions["Look"];
            jumpAction = playerInput.actions["Jump"];
            sprintAction = playerInput.actions["Sprint"];
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // initialize stamina
        currentStamina = stamina;
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = stamina;
            staminaSlider.minValue = 0f;
            staminaSlider.value = currentStamina;
        }
    }

    void OnEnable()
    {
        if (moveAction != null) moveAction.Enable();
        if (lookAction != null) lookAction.Enable();
        if (jumpAction != null) { jumpAction.Enable(); jumpAction.performed += OnJump; }
        if (sprintAction != null) sprintAction.Enable();
    }

    void OnDisable()
    {
        if (moveAction != null) moveAction.Disable();
        if (lookAction != null) lookAction.Disable();
        if (jumpAction != null) { jumpAction.performed -= OnJump; jumpAction.Disable(); }
        if (sprintAction != null) sprintAction.Disable();
    }

    void Update()
    {
        HandleMovement();
        ApplyGravity();
    }

    void LateUpdate()
    {
        // handle look in LateUpdate to reduce perceived camera lag
        HandleLook();
    }

    void HandleMovement()
    {
        Vector2 input = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        bool sprinting = false;
        if (sprintAction != null)
        {
            // sprint action could be a button (float) or a toggle; treat >0.5 as active
            float val = sprintAction.ReadValue<float>();
            sprinting = val > 0.5f;
        }

        // only allow sprinting when there is enough stamina
        if (sprinting && currentStamina <= minStaminaToSprint)
        {
            sprinting = false;
        }

        float speed = sprinting ? sprintSpeed : walkSpeed;

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        Vector3 horizontal = move * speed;

        controller.Move((horizontal + new Vector3(0, velocity.y, 0)) * Time.deltaTime);

        // update stamina: drain while sprinting, regenerate otherwise after a delay
        if (sprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina < 0f) currentStamina = 0f;
            // reset regen delay when sprinting
            regenDelayTimer = staminaRegenDelay;
        }
        else
        {
            // count down the regen delay first
            if (regenDelayTimer > 0f)
            {
                regenDelayTimer -= Time.deltaTime;
                if (regenDelayTimer < 0f) regenDelayTimer = 0f;
            }
            else
            {
                if (currentStamina < stamina)
                {
                    currentStamina += staminaRegenRate * Time.deltaTime;
                    if (currentStamina > stamina) currentStamina = stamina;
                }
            }
        }

        // update UI slider if assigned
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }
    }

    void HandleLook()
    {
        if (lookAction == null || playerCamera == null) return;

        Vector2 look = lookAction.ReadValue<Vector2>();

        // determine input device: mouse should not be scaled by Time.deltaTime (it's already delta per frame),
        // gamepad sticks should be scaled by Time.deltaTime for frame-rate independence
        var active = lookAction.activeControl;
        bool isMouse = active != null && active.device is UnityEngine.InputSystem.Mouse;

        if (isMouse)
        {
            // mouse: sensitivity only
            look *= lookSensitivity;
        }
        else
        {
            // gamepad/other: sensitivity and frame-rate scale
            look *= lookSensitivity * Time.deltaTime;
        }

        float yaw = look.x;
        float pitch = look.y;

        // rotate player (yaw)
        transform.Rotate(Vector3.up * yaw);

        // update target camera pitch (invert Y as typical behavior)
        cameraPitch -= pitch;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);

        // apply smoothing only to the displayed camera pitch (mouse remains snappy)
        if (isMouse || lookSmoothTime <= 0f)
        {
            displayedCameraPitch = cameraPitch;
        }
        else
        {
            displayedCameraPitch = Mathf.SmoothDampAngle(displayedCameraPitch, cameraPitch, ref pitchSmoothVelocity, lookSmoothTime);
        }

        playerCamera.localEulerAngles = new Vector3(displayedCameraPitch, 0f, 0f);
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // small negative to keep grounded
        }

        velocity.y += gravity * Time.deltaTime;
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        if (controller.isGrounded)
        {
            // v = sqrt(2 * g * h) but gravity is negative, so use -gravity
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
