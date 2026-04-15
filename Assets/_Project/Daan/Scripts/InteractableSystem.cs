using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public enum StationType
{
    Crafter,
    ResearchCenter,
    UpgradeStation
}

[RequireComponent(typeof(BoxCollider))]
public class InteractableSystem : MonoBehaviour
{
    public StationType stationType;
    public string promptMessage = "Press [E] to Interact";
    public TMP_Text interactionPromptText; // Sleep je UI tekst hierin in Unity
    
    [Header("Events (Optional UI links)")]
    public UnityEvent OnInteract;

    private bool isPlayerInRange = false;
    private InventoryManager playerInventory;
    private UpgradeManager playerUpgradeManager;
    private PlayerInput playerInput;
    private InputAction interactAction;

    void Awake()
    {
        // Must be set as a trigger for OnTriggerEnter to work correctly
        GetComponent<BoxCollider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            if (interactionPromptText != null)
            {
                interactionPromptText.text = promptMessage;
                interactionPromptText.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log(promptMessage); // Fallback if no UI attached
            }

            playerInput = other.GetComponent<PlayerInput>();
            playerInventory = other.GetComponent<InventoryManager>();
            playerUpgradeManager = other.GetComponent<UpgradeManager>();

            if (playerInput != null && playerInput.actions != null)
            {
                interactAction = playerInput.actions["Interact"];
                interactAction.Enable();
                interactAction.performed += Interact;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            if (interactionPromptText != null)
            {
                interactionPromptText.gameObject.SetActive(false);
            }

            if (interactAction != null)
            {
                interactAction.performed -= Interact;
                interactAction = null;
            }
        }
    }

    private void Interact(InputAction.CallbackContext ctx)
    {
        if (!isPlayerInRange) return;

        Debug.Log("Interacting with: " + stationType);
        OnInteract.Invoke();

        switch (stationType)
        {
            case StationType.Crafter:
                HandleCrafter();
                break;
            case StationType.ResearchCenter:
                HandleResearch();
                break;
            case StationType.UpgradeStation:
                HandleUpgradeStation();
                break;
        }
    }

    void HandleCrafter()
    {
        // Example: Craft a Sponge Trap
        // In reality, this opens a UI, but for now we do a direct check
        if (playerInventory != null && playerInventory.HasBlueprint("SpongeTrap"))
        {
            playerInventory.AddSpongeTrap();
            Debug.Log("Crafted a Sponge Trap!");
        }
        else
        {
            Debug.Log("Crafter: Missing Blueprint to craft anything.");
        }
    }

    void HandleResearch()
    {
        // Combine keycard pieces
        int piecesNeeded = 3;
        if (playerInventory != null && playerInventory.UseKeycardPieces(piecesNeeded))
        {
            Debug.Log("Research Center: Pieces combined! New map area unlocked.");
            // Open door logic or GameMgr flag
        }
        else
        {
            Debug.Log($"Research Center: Need {piecesNeeded} pieces.");
        }
    }

    void HandleUpgradeStation()
    {
        // Open the Upgrade UI visually instead of applying directly
        if (playerUpgradeManager != null)
        {
            playerUpgradeManager.ToggleUpgradeMenu();
        }
        else
        {
            Debug.LogWarning("Player missing UpgradeManager!");
        }
    }
}
