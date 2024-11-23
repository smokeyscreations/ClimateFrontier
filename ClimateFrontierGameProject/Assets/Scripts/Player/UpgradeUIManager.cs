// UpgradeUIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject upgradeUIPanel; // Assign via Inspector
    [SerializeField] private Image upgradeIcon;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button upgradeButton;

    private Upgrade currentUpgrade;

    private void Start()
    {
        // Initially hide the Upgrade UI
        HideUpgradeUI();

        // Assign the button listener
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        }
        else
        {
            Debug.LogError("Upgrade Button is not assigned in the UpgradeUIManager.");
        }
    }

    /// <summary>
    /// Displays the Upgrade UI with the specified upgrade details.
    /// </summary>
    /// <param name="upgrade">The upgrade to display.</param>
    public void ShowUpgradeUI(Upgrade upgrade)
    {
        currentUpgrade = upgrade;

        // Update UI elements
        if (upgradeIcon != null && currentUpgrade.icon != null)
        {
            upgradeIcon.sprite = currentUpgrade.icon;
            upgradeIcon.enabled = true;
        }
        else if (upgradeIcon != null)
        {
            // Hide the icon if none is provided
            upgradeIcon.enabled = false;
        }

        if (descriptionText != null)
        {
            descriptionText.text = currentUpgrade.description;
        }

        // Show the panel
        if (upgradeUIPanel != null)
        {
            upgradeUIPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("UpgradeUIPanel is not assigned in the UpgradeUIManager.");
        }
    }

    /// <summary>
    /// Hides the Upgrade UI.
    /// </summary>
    private void OnUpgradeButtonClicked()
    {
        Debug.Log("Upgrade Button Clicked");
        if (UpgradeManager.Instance != null && currentUpgrade != null)
        {
            Debug.Log($"Applying Upgrade: {currentUpgrade.description}");
            UpgradeManager.Instance.ApplyUpgrade(currentUpgrade);
            HideUpgradeUI(); // Ensure this is called to hide the panel
        }
        else
        {
            Debug.LogError("UpgradeManager instance or currentUpgrade is not set.");
        }
    }

    public void HideUpgradeUI()
    {
        if (upgradeUIPanel != null)
        {
            upgradeUIPanel.SetActive(false);
            Debug.Log("Upgrade Panel Hidden");
        }
        else
        {
            Debug.LogError("UpgradeUIPanel is not assigned in the UpgradeUIManager.");
        }
    }

}
