using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    private float currentHealth;
    private float maxHealth;

    public float CurrentHealth => currentHealth;

    public void Initialize(float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Player takes {amount} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Player healed by {amount}. Current health: {currentHealth}");
    }

    private void Die()
    {
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.ResetUpgrades();
        }
        else
        {
            Debug.LogError("UpgradeManager instance not found.");
        }
        Debug.Log("Player has died.");

    }
}
