using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [SerializeField] private UpgradeUIManager upgradeUIManager; // Assign via Inspector

    private BasePlayer player;
    private PlayerExperience playerExperience;

    private void Awake()
    {
        // Implement Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes if necessary
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameInitializer gameInitializer = FindAnyObjectByType<GameInitializer>();
        if (gameInitializer != null)
        {
            gameInitializer.OnPlayerInstantiated += HandlePlayerInstantiated;
            Debug.Log("Subscribed to GameInitializer.OnPlayerInstantiated");
        }
        else
        {
            Debug.LogError("GameInitializer not found in the scene.");
        }
    }

    private void OnDisable()
    {
        GameInitializer gameInitializer = FindAnyObjectByType<GameInitializer>();
        if (gameInitializer != null)
        {
            gameInitializer.OnPlayerInstantiated -= HandlePlayerInstantiated;
            Debug.Log("Unsubscribed from GameInitializer.OnPlayerInstantiated");
        }

        if (playerExperience != null)
        {
            playerExperience.OnUpgradeAvailable -= HandleUpgradeAvailable;
            Debug.Log("Unsubscribed from PlayerExperience.OnUpgradeAvailable");
        }
    }

    private void HandlePlayerInstantiated(GameObject instantiatedPlayer)
    {
        if (instantiatedPlayer != null)
        {
            player = instantiatedPlayer.GetComponent<BasePlayer>();
            if (player != null)
            {
                playerExperience = player.GetComponent<PlayerExperience>();
                if (playerExperience != null)
                {
                    // Subscribe to the OnUpgradeAvailable event
                    playerExperience.OnUpgradeAvailable += HandleUpgradeAvailable;
                    Debug.Log("Upgrade Available");
                }
                else
                {
                    Debug.LogError("PlayerExperience component not found on the player.");
                }
            }
            else
            {
                Debug.LogError("BasePlayer component not found on the instantiated player.");
            }
        }
        else
        {
            Debug.LogError("Instantiated player is null.");
        }
    }

    private void HandleUpgradeAvailable()
    {
        Debug.Log("Handling Upgrade");
        // Display the Upgrade UI
        if (upgradeUIManager != null)
        {
            // Generate a list of upgrades
            List<Upgrade> upgrades = GenerateUpgrades();

            // Show the upgrade UI with multiple options
            upgradeUIManager.ShowUpgradeUI(upgrades, ApplyUpgrade);
        }
        else
        {
            Debug.LogError("UpgradeUIManager is not assigned in the UpgradeManager.");
        }
    }

    private List<Upgrade> GenerateUpgrades()
    {
        List<Upgrade> upgrades = new List<Upgrade>();

        // For now, create predefined upgrades
        upgrades.Add(new Upgrade
        {
            upgradeType = UpgradeType.IncreaseAttackDamage,
            description = "Increase Attack Damage by +5",
            value = 5,
            icon = null // Assign a default or specific icon if available
        });

        upgrades.Add(new Upgrade
        {
            upgradeType = UpgradeType.IncreaseMaxHealth,
            description = "Increase Max Health by +10",
            value = 10,
            icon = null
        });

        upgrades.Add(new Upgrade
        {
            upgradeType = UpgradeType.IncreaseMovementSpeed,
            description = "Increase Movement Speed by 10%",
            value = 10,
            icon = null
        });

        // Randomize or select upgrades as needed
        return upgrades;
    }

    public void ApplyUpgrade(Upgrade upgrade)
    {
        if (upgrade == null)
        {
            Debug.LogError("Upgrade is null.");
            return;
        }

        if (player == null)
        {
            Debug.LogError("Player is null.");
            return;
        }

        switch (upgrade.upgradeType)
        {
            case UpgradeType.IncreaseAttackDamage:
                player.IncreaseAttackDamage(upgrade.value);
                break;

            case UpgradeType.IncreaseMaxHealth:
                player.IncreaseMaxHealth(upgrade.value);
                break;

            case UpgradeType.IncreaseMovementSpeed:
                player.IncreaseMovementSpeed(upgrade.value);
                break;

            // Handle other upgrade types as needed

            default:
                Debug.LogWarning("Unhandled upgrade type.");
                break;
        }

        Debug.Log($"Applied Upgrade: {upgrade.description}");
    }

    public void ResetUpgrades()
    {
        if (player != null)
        {
            player.ResetStats();
            Debug.Log("All upgrades have been reset.");
        }
        else
        {
            Debug.LogError("Player instance is not assigned in UpgradeManager.");
        }
    }
}
