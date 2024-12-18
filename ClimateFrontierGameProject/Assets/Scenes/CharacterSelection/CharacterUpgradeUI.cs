using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;
using System.Collections;

public class CharacterUpgradeUI : MonoBehaviour
{
    // Singleton Instance
    public static CharacterUpgradeUI Instance { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;
    public MMProgressBar xpProgressBar;

    public Button upgradeHealthButton;
    public Button upgradeSpeedButton;
    public Button upgradeAttackButton;
    public Button returnButton;
    public Button revertAllButton;

    [Header("Upgrade Costs")]
    public int healthUpgradeCost = 50;
    public int speedUpgradeCost = 40;
    public int attackUpgradeCost = 60;

    [Header("Character Display")]
    public CharacterUpgradeManager characterUpgradeManager;
    public float characterDisplayScale = 0.2f;

    [Header("Upgrade Slots")]
    public Image[] healthSlots;
    public Image[] speedSlots;
    public Image[] attackSlots;

    private int healthUpgradesCount = 0;
    private int speedUpgradesCount = 0;
    private int attackUpgradesCount = 0;

    // XP & Leveling
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float currentXp = 0f;
    [SerializeField] private float xpToNextLevel = 2100f;
    [SerializeField] private float baseXpPerUpgrade = 500f;
    [SerializeField] private float xpIncreasePerLevel = 300f;
    [SerializeField] private float xpIncreaseFactor = 1.9f;

    private CharacterData currentCharacter;
    private CharacterData runtimeCharacterData; // Clone for runtime modifications
    private string characterKey;

    // Flags to prevent multiple simultaneous upgrades
    private bool isUpgradingHealth = false;
    private bool isUpgradingSpeed = false;
    private bool isUpgradingAttack = false;

    // Define constants for PlayerPrefs keys to ensure consistency
    private string OriginalHealthKey => $"{characterKey}_OriginalHealth";
    private string HealthKey => $"{characterKey}_Health";
    private string OriginalSpeedKey => $"{characterKey}_OriginalSpeed";
    private string SpeedKey => $"{characterKey}_Speed";
    private string OriginalAttackKey => $"{characterKey}_OriginalAttack";
    private string AttackKey => $"{characterKey}_Attack";
    private string OriginalGlobalGoldKey => "OriginalGlobalGold";
    private string GlobalGoldKey => "GlobalGold";
    private string OriginalLevelKey => $"{characterKey}_OriginalLevel";
    private string LevelKey => $"{characterKey}_Level";
    private string OriginalXPKey => $"{characterKey}_OriginalXP";
    private string CurrentXPKey => $"{characterKey}_CurrentXP";
    private string OriginalXPToNextLevelKey => $"{characterKey}_OriginalXPToNextLevel";
    private string XPToNextLevelKey => $"{characterKey}_XPToNextLevel";
    private string OriginalBaseXPPerUpgradeKey => $"{characterKey}_OriginalBaseXPPerUpgrade";
    private string BaseXPPerUpgradeKey => $"{characterKey}_BaseXPPerUpgrade";
    private string OriginalHealthUpgradesKey => $"{characterKey}_OriginalHealthUpgrades";
    private string HealthUpgradesKey => $"{characterKey}_HealthUpgrades";
    private string OriginalSpeedUpgradesKey => $"{characterKey}_OriginalSpeedUpgrades";
    private string SpeedUpgradesKey => $"{characterKey}_SpeedUpgrades";
    private string OriginalAttackUpgradesKey => $"{characterKey}_OriginalAttackUpgrades";
    private string AttackUpgradesKey => $"{characterKey}_AttackUpgrades";

    void Awake()
    {
        // Implement Singleton Pattern without DontDestroyOnLoad
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple instances of CharacterUpgradeUI detected in the scene. Destroying duplicate.");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        // Removed DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        Debug.Log("CharacterUpgradeUI Start called");

        currentCharacter = GameManager.Instance.selectedCharacterData;
        if (currentCharacter == null)
        {
            Debug.LogError("No character selected!");
            return;
        }

        characterKey = currentCharacter.characterName;

        // Clone the CharacterData ScriptableObject for runtime modifications
        runtimeCharacterData = Instantiate(currentCharacter);

        // Check if original keys are set; if not, set them now
        if (!PlayerPrefs.HasKey(OriginalHealthKey))
        {
            // Store original stats in permanent original keys
            PlayerPrefs.SetFloat(OriginalHealthKey, runtimeCharacterData.maxHealth);
            PlayerPrefs.SetFloat(OriginalSpeedKey, runtimeCharacterData.baseWalkingSpeed);
            PlayerPrefs.SetFloat(OriginalAttackKey, runtimeCharacterData.baseAttackDamage);

            PlayerPrefs.SetInt(OriginalGlobalGoldKey, GameManager.Instance.globalGold);

            PlayerPrefs.SetInt(OriginalHealthUpgradesKey, healthUpgradesCount);
            PlayerPrefs.SetInt(OriginalSpeedUpgradesKey, speedUpgradesCount);
            PlayerPrefs.SetInt(OriginalAttackUpgradesKey, attackUpgradesCount);

            PlayerPrefs.Save();
            Debug.Log("Original stats saved to PlayerPrefs.");
        }

        // Now load current stats (if any) from PlayerPrefs
        LoadCurrentStats();

        characterUpgradeManager.DisplayCharacter(runtimeCharacterData, characterDisplayScale);
        RefreshUI();

        // Remove existing listeners to prevent duplicates
        upgradeHealthButton.onClick.RemoveAllListeners();
        upgradeSpeedButton.onClick.RemoveAllListeners();
        upgradeAttackButton.onClick.RemoveAllListeners();
        returnButton.onClick.RemoveAllListeners();
        revertAllButton.onClick.RemoveAllListeners();

        // Set button listeners
        upgradeHealthButton.onClick.AddListener(OnUpgradeHealthClicked);
        upgradeSpeedButton.onClick.AddListener(OnUpgradeSpeedClicked);
        upgradeAttackButton.onClick.AddListener(OnUpgradeAttackClicked);
        returnButton.onClick.AddListener(OnReturnToSelectionClicked);
        revertAllButton.onClick.AddListener(OnRevertAllClicked);

        // Optional: Log the number of instances for debugging
        CharacterUpgradeUI[] instances = FindObjectsOfType<CharacterUpgradeUI>();
        Debug.Log($"Number of CharacterUpgradeUI instances in scene: {instances.Length}");
    }

    private void LoadCurrentStats()
    {
        // Load current modified stats if available
        if (PlayerPrefs.HasKey(HealthKey))
        {
            runtimeCharacterData.maxHealth = PlayerPrefs.GetFloat(HealthKey, runtimeCharacterData.maxHealth);
            runtimeCharacterData.baseWalkingSpeed = PlayerPrefs.GetFloat(SpeedKey, runtimeCharacterData.baseWalkingSpeed);
            runtimeCharacterData.baseAttackDamage = PlayerPrefs.GetFloat(AttackKey, runtimeCharacterData.baseAttackDamage);
        }

        GameManager.Instance.globalGold = PlayerPrefs.GetInt(GlobalGoldKey, GameManager.Instance.globalGold);

        currentLevel = PlayerPrefs.GetInt(LevelKey, currentLevel);
        currentXp = PlayerPrefs.GetFloat(CurrentXPKey, currentXp);
        xpToNextLevel = PlayerPrefs.GetFloat(XPToNextLevelKey, xpToNextLevel); // Corrected
        baseXpPerUpgrade = PlayerPrefs.GetFloat(BaseXPPerUpgradeKey, baseXpPerUpgrade); // Corrected

        healthUpgradesCount = PlayerPrefs.GetInt(HealthUpgradesKey, healthUpgradesCount);
        speedUpgradesCount = PlayerPrefs.GetInt(SpeedUpgradesKey, speedUpgradesCount);
        attackUpgradesCount = PlayerPrefs.GetInt(AttackUpgradesKey, attackUpgradesCount);

        Debug.Log($"Loaded Stats - Level: {currentLevel}, XP: {currentXp}/{xpToNextLevel}, Base XP per Upgrade: {baseXpPerUpgrade}");
        Debug.Log($"Character Stats - Health: {runtimeCharacterData.maxHealth}, Speed: {runtimeCharacterData.baseWalkingSpeed}, Attack: {runtimeCharacterData.baseAttackDamage}");
    }

    private void SaveCurrentStats()
    {
        PlayerPrefs.SetFloat(HealthKey, runtimeCharacterData.maxHealth);
        PlayerPrefs.SetFloat(SpeedKey, runtimeCharacterData.baseWalkingSpeed);
        PlayerPrefs.SetFloat(AttackKey, runtimeCharacterData.baseAttackDamage);

        PlayerPrefs.SetInt(GlobalGoldKey, GameManager.Instance.globalGold);

        PlayerPrefs.SetInt(LevelKey, currentLevel);
        PlayerPrefs.SetFloat(CurrentXPKey, currentXp);
        PlayerPrefs.SetFloat(XPToNextLevelKey, xpToNextLevel);
        PlayerPrefs.SetFloat(BaseXPPerUpgradeKey, baseXpPerUpgrade);

        PlayerPrefs.SetInt(HealthUpgradesKey, healthUpgradesCount);
        PlayerPrefs.SetInt(SpeedUpgradesKey, speedUpgradesCount);
        PlayerPrefs.SetInt(AttackUpgradesKey, attackUpgradesCount);

        PlayerPrefs.Save();
        Debug.Log("Current stats saved to PlayerPrefs.");
    }

    private void RefreshUI()
    {
        characterNameText.text = runtimeCharacterData.characterName;
        goldText.text = $"{GameManager.Instance.globalGold}";
        healthText.text = $"Health: {runtimeCharacterData.maxHealth}";
        speedText.text = $"Speed: {runtimeCharacterData.baseWalkingSpeed}";
        attackText.text = $"Attack: {runtimeCharacterData.baseAttackDamage}";

        levelText.text = $"{currentLevel}";
        xpText.text = $"{Mathf.Floor(currentXp)}/{Mathf.Floor(xpToNextLevel)} XP";

        // Normalize XP for the progress bar
        float normalizedXp = Mathf.Clamp01(currentXp / xpToNextLevel);
        xpProgressBar.UpdateBar(normalizedXp, 0f, 1f);
        Debug.Log($"Updating Progress Bar: {currentXp} / {xpToNextLevel} (Normalized: {normalizedXp})");

        UpdateSlotsUI(healthSlots, healthUpgradesCount);
        UpdateSlotsUI(speedSlots, speedUpgradesCount);
        UpdateSlotsUI(attackSlots, attackUpgradesCount);
    }

    private void UpdateSlotsUI(Image[] slotArray, int count)
    {
        for (int i = 0; i < slotArray.Length; i++)
        {
            slotArray[i].color = (i < count) ? Color.red : Color.white;
        }
    }

    public void OnUpgradeHealthClicked()
    {
        if (isUpgradingHealth) return;
        StartCoroutine(UpgradeHealthRoutine());
    }

    private IEnumerator UpgradeHealthRoutine()
    {
        isUpgradingHealth = true;
        Debug.Log("Upgrade Health Clicked");
        if (TryPurchaseUpgrade(healthUpgradeCost))
        {
            runtimeCharacterData.maxHealth += 10f;
            healthUpgradesCount++;
            GainXPFromUpgrade();
            SaveCurrentStats();
            RefreshUI();
            Debug.Log($"Health upgraded to {runtimeCharacterData.maxHealth}");
        }
        else
        {
            Debug.Log("Not enough gold for health upgrade!");
        }
        yield return null;
        isUpgradingHealth = false;
    }

    public void OnUpgradeSpeedClicked()
    {
        if (isUpgradingSpeed) return;
        StartCoroutine(UpgradeSpeedRoutine());
    }

    private IEnumerator UpgradeSpeedRoutine()
    {
        isUpgradingSpeed = true;
        Debug.Log("Upgrade Speed Clicked");
        if (TryPurchaseUpgrade(speedUpgradeCost))
        {
            runtimeCharacterData.baseWalkingSpeed += 0.5f;
            speedUpgradesCount++;
            GainXPFromUpgrade();
            SaveCurrentStats();
            RefreshUI();
            Debug.Log($"Speed upgraded to {runtimeCharacterData.baseWalkingSpeed}");
        }
        else
        {
            Debug.Log("Not enough gold for speed upgrade!");
        }
        yield return null;
        isUpgradingSpeed = false;
    }

    public void OnUpgradeAttackClicked()
    {
        if (isUpgradingAttack) return;
        StartCoroutine(UpgradeAttackRoutine());
    }

    private IEnumerator UpgradeAttackRoutine()
    {
        isUpgradingAttack = true;
        Debug.Log("Upgrade Attack Clicked");
        if (TryPurchaseUpgrade(attackUpgradeCost))
        {
            runtimeCharacterData.baseAttackDamage += 5f;
            attackUpgradesCount++;
            GainXPFromUpgrade();
            SaveCurrentStats();
            RefreshUI();
            Debug.Log($"Attack upgraded to {runtimeCharacterData.baseAttackDamage}");
        }
        else
        {
            Debug.Log("Not enough gold for attack upgrade!");
        }
        yield return null;
        isUpgradingAttack = false;
    }

    private bool TryPurchaseUpgrade(int cost)
    {
        if (GameManager.Instance.globalGold >= cost)
        {
            GameManager.Instance.globalGold -= cost;
            Debug.Log($"Purchased upgrade for {cost} gold. Remaining gold: {GameManager.Instance.globalGold}");
            return true;
        }
        return false;
    }

    public void OnReturnToSelectionClicked()
    {
        Debug.Log("Return to Selection Clicked");
        SaveCurrentStats();
        SceneManager.LoadScene("CharacterSelection");
    }

    public void OnRevertAllClicked()
    {
        Debug.Log("Revert All Clicked");
        // Load original values from PlayerPrefs (never overwritten)
        float originalHealth = PlayerPrefs.GetFloat(OriginalHealthKey);
        float originalSpeed = PlayerPrefs.GetFloat(OriginalSpeedKey);
        float originalAttack = PlayerPrefs.GetFloat(OriginalAttackKey);
        int originalGold = PlayerPrefs.GetInt(OriginalGlobalGoldKey, GameManager.Instance.globalGold);

        int originalLevel = PlayerPrefs.GetInt(OriginalLevelKey, 1);
        float originalXP = PlayerPrefs.GetFloat(OriginalXPKey, 0f);
        float originalXPToNextLevel = PlayerPrefs.GetFloat(OriginalXPToNextLevelKey, 2100f);

        int originalHealthUpgradesCount = PlayerPrefs.GetInt(OriginalHealthUpgradesKey, 0);
        int originalSpeedUpgradesCount = PlayerPrefs.GetInt(OriginalSpeedUpgradesKey, 0);
        int originalAttackUpgradesCount = PlayerPrefs.GetInt(OriginalAttackUpgradesKey, 0);

        // Calculate how much gold was spent on all upgrades
        int totalHealthGoldSpent = (healthUpgradesCount - originalHealthUpgradesCount) * healthUpgradeCost;
        int totalSpeedGoldSpent = (speedUpgradesCount - originalSpeedUpgradesCount) * speedUpgradeCost;
        int totalAttackGoldSpent = (attackUpgradesCount - originalAttackUpgradesCount) * attackUpgradeCost;
        int totalGoldSpent = Mathf.Max(0, totalHealthGoldSpent) + Mathf.Max(0, totalSpeedGoldSpent) + Mathf.Max(0, totalAttackGoldSpent);

        Debug.Log($"Reverting all upgrades. Total gold spent: {totalGoldSpent}");

        // Revert to original stats
        runtimeCharacterData.maxHealth = originalHealth;
        runtimeCharacterData.baseWalkingSpeed = originalSpeed;
        runtimeCharacterData.baseAttackDamage = originalAttack;

        // Refund gold spent by returning globalGold to original
        GameManager.Instance.globalGold = originalGold;

        // Revert XP and Level
        currentLevel = originalLevel;
        currentXp = originalXP;
        xpToNextLevel = originalXPToNextLevel;

        // Revert upgrade counts
        healthUpgradesCount = originalHealthUpgradesCount;
        speedUpgradesCount = originalSpeedUpgradesCount;
        attackUpgradesCount = originalAttackUpgradesCount;

        // Save current state after revert
        SaveCurrentStats();
        RefreshUI();
    }

    #region XP & Leveling Logic
    private void GainXPFromUpgrade()
    {
        currentXp += baseXpPerUpgrade;
        Debug.Log($"Gained XP: {currentXp}/{xpToNextLevel}");
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        while (currentXp >= xpToNextLevel)
        {
            currentXp -= xpToNextLevel;
            currentLevel++;
            xpToNextLevel *= xpIncreaseFactor;
            baseXpPerUpgrade += xpIncreasePerLevel;
            Debug.Log($"Leveled up! New Level: {currentLevel}, Next XP Threshold: {xpToNextLevel}, New Base XP per Upgrade: {baseXpPerUpgrade}");
            SaveCurrentStats(); // Ensure changes are saved after leveling up
            RefreshUI();
        }
    }
    #endregion

    #region Testing and Debugging (Optional)
    // Optional: Add methods to reset or debug PlayerPrefs if needed
    // Ensure these are only used during development/testing
    [ContextMenu("Reset PlayerPrefs")]
    public void ResetPlayerPrefs()
    {
        //PlayerPrefs.DeleteAll();
        //Debug.Log("PlayerPrefs have been reset.");
    }
    #endregion
}
