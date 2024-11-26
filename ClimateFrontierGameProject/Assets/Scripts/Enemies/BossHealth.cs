// Assets/Scripts/Boss/BossHealth.cs
using UnityEngine;

public class BossHealth : MonoBehaviour, IDamageable
{
    [Header("Boss Health Settings")]
    public float maxHealth = 1000f;
    private float currentHealth;

    public event System.Action OnBossDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} takes {amount} damage. Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has been defeated!");
        OnBossDeath?.Invoke();
        // Implement additional death logic here (e.g., play death animation, drop loot)
        Destroy(gameObject);
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"{gameObject.name} healed by {amount}. Current Health: {currentHealth}");
    }
}
