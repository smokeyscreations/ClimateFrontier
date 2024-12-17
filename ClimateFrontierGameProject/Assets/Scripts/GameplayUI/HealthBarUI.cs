using UnityEngine;
using TMPro; // For TextMeshPro
using MoreMountains.Tools;
using System;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private MMProgressBar healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    private BasePlayer player;

    private void OnEnable()
    {
        // Subscribe to GameInitializer's OnPlayerInstantiated event
        GameInitializer gameInitializer = FindAnyObjectByType<GameInitializer>();
        if (gameInitializer != null)
        {
            gameInitializer.OnPlayerInstantiated += HandlePlayerInstantiated;
            Debug.Log("HealthBarUI: Subscribed to OnPlayerInstantiated");
        }
        else
        {
            Debug.LogError("HealthBarUI: GameInitializer not found in the scene.");
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the event to prevent memory leaks
        GameInitializer gameInitializer = FindAnyObjectByType<GameInitializer>();
        if (gameInitializer != null)
        {
            gameInitializer.OnPlayerInstantiated -= HandlePlayerInstantiated;
            Debug.Log("HealthBarUI: Unsubscribed from OnPlayerInstantiated");
        }
    }

    private void HandlePlayerInstantiated(GameObject instantiatedPlayer)
    {
        player = instantiatedPlayer.GetComponent<BasePlayer>();
        if (player != null)
        {
            if (player.healthSystem != null)
            {
                // Subscribe to health change events
                player.healthSystem.OnHealthChanged += UpdateHealthBar;
                Debug.Log("HealthBarUI: Player and PlayerHealth found. Subscribed to OnHealthChanged.");
                UpdateHealthBar(); // Initial update
            }
            else
            {
                Debug.LogError("HealthBarUI: PlayerHealth (healthSystem) is null on BasePlayer.");
            }
        }
        else
        {
            Debug.LogError("HealthBarUI: BasePlayer component not found on instantiated player.");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from health change events
        if (player != null && player.healthSystem != null)
        {
            player.healthSystem.OnHealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar()
    {
        if (player != null && healthBar != null)
        {
            float currentHealth = player.CurrentHealth;
            float maxHealth = player.MaxHealth;
            Debug.Log($"Updating Health Bar: {currentHealth}/{maxHealth}");

            float normalizedHealth = currentHealth / maxHealth;

            healthBar.UpdateBar01(normalizedHealth);
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }
}
