using System.Collections.Generic;
using UnityEngine;
using TMPro; // Voeg dit toe voor TextMeshPro

public class InventoryManager : MonoBehaviour
{
    [Header("Resources")]
    public int keycardPieces = 0;
    
    [Header("Items")]
    public int spongeTraps = 0;

    [Header("Blueprints")]
    // List of Strings containing unlocked blueprints (e.g. "UVFlashlight", "BetterBattery")
    public List<string> unlockedBlueprints = new List<string>();

    [Header("UI Elements")]
    public TMP_Text keycardText;
    public TMP_Text spongeText;

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (keycardText != null) keycardText.text = "Keycards: " + keycardPieces;
        if (spongeText != null) spongeText.text = "Sponges: " + spongeTraps;
    }

    public void AddBlueprint(string blueprintId)
    {
        if (!unlockedBlueprints.Contains(blueprintId))
        {
            unlockedBlueprints.Add(blueprintId);
            Debug.Log("New Blueprint Unlocked: " + blueprintId);
        }
    }

    public bool HasBlueprint(string blueprintId)
    {
        return unlockedBlueprints.Contains(blueprintId);
    }

    public void AddKeycardPiece(int amount = 1)
    {
        keycardPieces += amount;
        Debug.Log("Keycard Pieces: " + keycardPieces);
        UpdateUI();
    }

    public bool UseKeycardPieces(int amount)
    {
        if (keycardPieces >= amount)
        {
            keycardPieces -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void AddSpongeTrap()
    {
        spongeTraps++;
        Debug.Log("Sponge Traps: " + spongeTraps);
        UpdateUI();
    }

    public bool UseSpongeTrap()
    {
        if (spongeTraps > 0)
        {
            spongeTraps--;
            UpdateUI();
            return true;
        }
        return false;
    }
}
