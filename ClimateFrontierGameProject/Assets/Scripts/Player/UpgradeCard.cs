using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeCard : MonoBehaviour, IPoolable
{
    [SerializeField] private Image upgradeIcon;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button upgradeButton;

    private Upgrade upgrade;
    private Action<Upgrade> onUpgradeSelected;

    public void Initialize(Upgrade upgrade, Action<Upgrade> onUpgradeSelected)
    {
        this.upgrade = upgrade;
        this.onUpgradeSelected = onUpgradeSelected;

        // Update UI elements
        if (upgradeIcon != null)
        {
            if (upgrade.icon != null)
            {
                upgradeIcon.sprite = upgrade.icon;
                upgradeIcon.enabled = true;
            }
            else
            {
                // Hide the icon if none is provided
                upgradeIcon.enabled = false;
            }
        }

        if (descriptionText != null)
        {
            descriptionText.text = upgrade.description;
        }

        // Assign the button listener
        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogError("Upgrade Button is not assigned in the UpgradeCard.");
        }
    }

    private void OnButtonClicked()
    {
        if (onUpgradeSelected != null)
        {
            onUpgradeSelected(upgrade);
        }
        else
        {
            Debug.LogError("onUpgradeSelected callback is not set.");
        }
    }

    // Implement IPoolable methods
    public void OnObjectSpawn()
    {
  
    }

    public void OnObjectReturn()
    {
        // Reset the card's state
        ResetCard();
    }

    private void ResetCard()
    {
        // Reset UI elements and state
        if (upgradeIcon != null)
        {
            upgradeIcon.sprite = null;
            upgradeIcon.enabled = false;
        }

        if (descriptionText != null)
        {
            descriptionText.text = "";
        }

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
        }

        // Clear references
        upgrade = null;
        onUpgradeSelected = null;
    }
}
