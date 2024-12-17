using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    private float currentHealth;
    private float maxHealth;

    public event Action OnHealthChanged;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    public void Initialize(float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnHealthChanged?.Invoke();
        Debug.Log($"Player takes {amount} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void IncreaseMaxHealth(float amount)
    {
        float healthPercentage = currentHealth / maxHealth;
        maxHealth += amount;
        currentHealth = Mathf.RoundToInt(maxHealth * healthPercentage);
        OnHealthChanged?.Invoke();
        Debug.Log($"Max Health increased by {amount}. Current health: {Mathf.RoundToInt(currentHealth)}/{maxHealth}");
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
