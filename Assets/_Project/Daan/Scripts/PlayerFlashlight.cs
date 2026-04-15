using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerFlashlight : MonoBehaviour
{
    [Tooltip("Assign a Light component to toggle (optional)")]
    public Light flashlight;

    [Tooltip("Or assign a GameObject (e.g. flashlight model) to toggle active state)")]
    public GameObject flashlightObject;

    [Tooltip("Start with flashlight on")]
    public bool startOn = false;

    PlayerInput playerInput;
    InputAction flashlightAction;
    bool isOn;

    void Awake()
    {
        isOn = startOn;
        if (flashlight != null) flashlight.enabled = isOn;
        if (flashlightObject != null) flashlightObject.SetActive(isOn);

        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null && playerInput.actions != null)
        {
            try
            {
                flashlightAction = playerInput.actions["Flashlight"];
            }
            catch
            {
                flashlightAction = null;
            }
        }
    }

    void OnEnable()
    {
        if (flashlightAction != null)
        {
            flashlightAction.Enable();
            flashlightAction.performed += OnFlashlightAction;
        }
    }

    void OnDisable()
    {
        if (flashlightAction != null)
        {
            flashlightAction.performed -= OnFlashlightAction;
            flashlightAction.Disable();
        }
    }

    void Update()
    {
        if (flashlightAction == null)
        {
            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            {
                ToggleFlashlight();
            }
        }
    }

    void OnFlashlightAction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) ToggleFlashlight();
    }

    void ToggleFlashlight()
    {
        isOn = !isOn;
        if (flashlight != null) flashlight.enabled = isOn;
        if (flashlightObject != null) flashlightObject.SetActive(isOn);
    }
}
