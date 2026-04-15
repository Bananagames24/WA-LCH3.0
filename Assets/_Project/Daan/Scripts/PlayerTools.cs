using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTools : MonoBehaviour
{
    [Header("Flashlight")]
    public Light flashlight;
    public Color normalLightColor = Color.white;
    public Color uvLightColor = new Color(0.5f, 0f, 1f); // Purple UV
    
    [Header("Upgrades Status")]
    public bool hasUVLight = false;
    public bool hasBetterBattery = false;
    public bool hasBetterFlashlight = false;
    
    [Header("Battery settings")]
    public float flashlightDrainRate = 2f;
    public float uvDrainRate = 5f; // UV costs more battery

    [Header("Cleaning Settings")]
    public float cleaningRange = 2f;
    public LayerMask dirtyObjectLayer;

    private PlayerInput playerInput;
    private PlayerStats playerStats;
    private InputAction interactAction;
    private InputAction flashlightAction;

    private bool isFlashlightOn = false;
    private bool isUVMode = false;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerStats = GetComponent<PlayerStats>();

        if (playerInput != null && playerInput.actions != null)
        {
            interactAction = playerInput.actions["Interact"]; // Used for cleaning and UI interaction
            flashlightAction = playerInput.actions["Flashlight"]; // Used for toggle
        }

        if (flashlight != null)
        {
            flashlight.enabled = false;
        }
    }

    void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.Enable();
            interactAction.performed += OnInteract;
        }
        if (flashlightAction != null)
        {
            flashlightAction.Enable();
            flashlightAction.performed += OnToggleFlashlight;
        }
    }

    void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.performed -= OnInteract;
            interactAction.Disable();
        }
        if (flashlightAction != null)
        {
            flashlightAction.performed -= OnToggleFlashlight;
            flashlightAction.Disable();
        }
    }

    void Update()
    {
        if (isFlashlightOn && playerStats != null && playerStats.currentBattery > 0)
        {
            // Drain extra battery when flashlight is on
            float drain = isUVMode ? uvDrainRate : flashlightDrainRate;
            if (hasBetterBattery) drain *= 0.5f; // Upgrade makes it drain slower
            
            playerStats.currentBattery -= drain * Time.deltaTime;

            if (playerStats.currentBattery <= 0)
            {
                TurnOffFlashlight();
            }
        }
    }

    private void OnToggleFlashlight(InputAction.CallbackContext ctx)
    {
        if (playerStats != null && playerStats.currentBattery <= 0) return;

        // Logic to cycle between Off -> Normal -> UV (if unlocked) -> Off
        if (!isFlashlightOn)
        {
            isFlashlightOn = true;
            isUVMode = false;
            UpdateFlashlightVisuals();
        }
        else if (isFlashlightOn && !isUVMode && hasUVLight)
        {
            isUVMode = true;
            UpdateFlashlightVisuals();
        }
        else
        {
            TurnOffFlashlight();
        }
    }

    void TurnOffFlashlight()
    {
        isFlashlightOn = false;
        isUVMode = false;
        UpdateFlashlightVisuals();
    }

    void UpdateFlashlightVisuals()
    {
        if (flashlight != null)
        {
            flashlight.enabled = isFlashlightOn;

            // Apply Better Flashlight upgrade
            flashlight.intensity = hasBetterFlashlight ? 3f : 1f;
            flashlight.range = hasBetterFlashlight ? 20f : 10f;

            flashlight.color = isUVMode ? uvLightColor : normalLightColor;
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        // Try cleaning
        CleanArea();
    }

    void CleanArea()
    {
        RaycastHit hit;
        // Search forward from camera center
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, cleaningRange, dirtyObjectLayer))
        {
            Debug.Log("Cleaned: " + hit.collider.name);
            // Example: trigger a "Clean()" function on the object
            // var dirt = hit.collider.GetComponent<DirtBehavior>();
            // if(dirt != null) dirt.Clean();

            hit.collider.gameObject.SetActive(false); // Temporary destroying dirt
        }
    }

    // Call this from an UpgradeManager
    public void UnlockUpgrade(string upgradeName)
    {
        switch (upgradeName)
        {
            case "UVFlashlight":
                hasUVLight = true;
                break;
            case "BetterBattery":
                hasBetterBattery = true;
                break;
            case "BetterFlashlight":
                hasBetterFlashlight = true;
                UpdateFlashlightVisuals(); // Update instantly if on
                break;
        }
    }
}
