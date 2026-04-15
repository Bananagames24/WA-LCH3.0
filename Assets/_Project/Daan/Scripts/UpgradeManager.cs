using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject upgradeMenuUI; // Sleep je upgrade menu Canvas/Panel hierin

    private PlayerStats playerStats;
    private PlayerTools playerTools;
    private PlayerMovement playerMovement;

    void Start()
    {
        // Typically these are on the same GameObject or known in scene
        playerStats = GetComponent<PlayerStats>();
        playerTools = GetComponent<PlayerTools>();
        playerMovement = GetComponent<PlayerMovement>();

        if (upgradeMenuUI != null)
        {
            upgradeMenuUI.SetActive(false); // Zorg dat het menu standaard uit staat
        }
    }

    public void ToggleUpgradeMenu()
    {
        if (upgradeMenuUI != null)
        {
            bool isActive = upgradeMenuUI.activeSelf;
            upgradeMenuUI.SetActive(!isActive);

            // Muis ontgrendelen als menu open is (zodat je kan klikken)
            if (!isActive)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void CloseUpgradeMenu()
    {
        if (upgradeMenuUI != null)
        {
            upgradeMenuUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Call dit vanaf de knoppen in je UI menu!
    public void ApplyUpgrade(string upgradeName)
    {
        switch (upgradeName)
        {
            case "UVFlashlight":
                if (playerTools != null) playerTools.UnlockUpgrade("UVFlashlight");
                break;
            case "MoreHealth":
                if (playerStats != null)
                {
                    playerStats.maxHealth += 50f;
                    playerStats.currentHealth = playerStats.maxHealth;
                }
                break;
            case "BetterBattery":
                if (playerTools != null) playerTools.UnlockUpgrade("BetterBattery");
                if (playerStats != null)
                {
                    playerStats.maxBattery += 50f;
                    playerStats.currentBattery = playerStats.maxBattery;
                }
                break;
            case "BetterFlashlight":
                if (playerTools != null) playerTools.UnlockUpgrade("BetterFlashlight");
                break;
            case "NewWheels":
                if (playerMovement != null)
                {
                    playerMovement.walkSpeed += 2f;
                    playerMovement.sprintSpeed += 3f;
                }
                break;
            case "SoapCannon":
                // TODO: Unlock a SoapCannon script or flag
                Debug.Log("Soap Cannon unlocked!");
                break;
            case "Map":
                // TODO: Enable a UI map element
                Debug.Log("Map unlocked!");
                break;
            default:
                Debug.LogWarning("Unknown upgrade: " + upgradeName);
                break;
        }
        Debug.Log("Applied Upgrade: " + upgradeName);
    }
}
