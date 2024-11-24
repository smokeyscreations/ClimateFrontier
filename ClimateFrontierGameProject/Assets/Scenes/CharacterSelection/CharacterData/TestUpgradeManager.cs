using UnityEngine;

public class TestUpgradeManager : MonoBehaviour
{
    public static TestUpgradeManager Instance;

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
        // Find GameInitializer in the scene
        TestInitializer gameInitializer = FindAnyObjectByType<TestInitializer>();
        if (gameInitializer != null)
        {
            // Subscribe to the OnPlayerInstantiated event
            gameInitializer.OnPlayerInstantiated += HandlePlayerInstantiated;
        }
        else
        {
            Debug.LogError("GameInitializer not found in the scene.");
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the event to prevent memory leaks
        TestInitializer gameInitializer = FindAnyObjectByType<TestInitializer>();
        if (gameInitializer != null)
        {
            gameInitializer.OnPlayerInstantiated -= HandlePlayerInstantiated;
        }
    }

    /// <summary>
    /// Handles the player instantiation event.
    /// </summary>
    /// <param name="instantiatedPlayer">The instantiated player GameObject.</param>
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
        // Display the Upgrade UI
        if (upgradeUIManager != null)
        {
            // For now, create a predefined upgrade
            Upgrade attackUpgrade = new Upgrade
            {
                upgradeType = UpgradeType.IncreaseAttackDamage,
                description = "Increase Attack Damage by +5",
                value = 5,
                icon = null // Assign a default or specific icon if available
            };

            upgradeUIManager.ShowUpgradeUI(attackUpgrade);
        }
        else
        {
            Debug.LogError("UpgradeUIManager is not assigned in the UpgradeManager.");
        }
    }

    /// <summary>
    /// Applies the specified upgrade to the player.
    /// </summary>
    /// <param name="upgrade">The upgrade to apply.</param>
    public void ApplyUpgrade(Upgrade upgrade)
    {
        if (upgrade == null)
        {
            Debug.LogError("Upgrade is null.");
            return;
        }

        switch (upgrade.upgradeType)
        {
            case UpgradeType.IncreaseAttackDamage:
                player.IncreaseAttackDamage(upgrade.value);
                Debug.Log($"Applied Upgrade: {upgrade.description}");
                break;

            // Future cases for different upgrade types can be added here

            default:
                Debug.LogWarning("Unhandled upgrade type.");
                break;
        }
    }

    /// <summary>
    /// Resets all applied upgrades. Call this upon player death.
    /// </summary>
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
