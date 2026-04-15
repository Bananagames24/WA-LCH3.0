using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthSlider;

    [Header("Battery")]
    public float maxBattery = 100f;
    [HideInInspector]
    public float currentBattery;
    public float batteryDrainRate = 1f; // Drain per second
    public Slider batterySlider;

    private GameManager gameManager;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        currentBattery = maxBattery;

        gameManager = FindFirstObjectByType<GameManager>();

        UpdateUI();
    }

    void Update()
    {
        if (isDead) return;

        DrainBattery();
        UpdateUI();
    }

    void DrainBattery()
    {
        currentBattery -= batteryDrainRate * Time.deltaTime;

        if (currentBattery <= 0)
        {
            currentBattery = 0;
            Die("Battery Empty");
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die("Health Depleted");
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public void RechargeBattery(float amount)
    {
        currentBattery += amount;
        if (currentBattery > maxBattery) currentBattery = maxBattery;
    }

    void Die(string reason)
    {
        isDead = true;
        Debug.Log("Player Died: " + reason);

        if (gameManager != null)
        {
            gameManager.TriggerRescue(this);
        }
    }

    public void Revive()
    {
        isDead = false;
        currentHealth = maxHealth;
        currentBattery = maxBattery;
        UpdateUI();
        Debug.Log("Player Revived by Rescue Bot!");
    }

    void UpdateUI()
    {
        if (healthSlider != null) healthSlider.value = currentHealth / maxHealth;
        if (batterySlider != null) batterySlider.value = currentBattery / maxBattery;
    }
}
