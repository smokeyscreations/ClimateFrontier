using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class UpgradeUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject upgradeUIPanel; // Panel containing all upgrade cards
    [SerializeField] private Transform upgradesContainer; // Parent transform for the upgrade cards
    // Removed upgradeCardPrefab since we use ObjectPooler

    private Action<Upgrade> onUpgradeSelected;

    private string upgradeCardPoolTag = "UpgradeCard"; // The tag used in ObjectPooler

    private List<GameObject> activeUpgradeCards = new List<GameObject>(); // Keep track of active cards


    private void Start()
    {
        // Initially hide the Upgrade UI
        HideUpgradeUI();
    }

    public void ShowUpgradeUI(List<Upgrade> upgrades, Action<Upgrade> onUpgradeSelected)
    {
        this.onUpgradeSelected = onUpgradeSelected;

        // Clear existing upgrade cards (return them to the pool)
        foreach (var cardObj in activeUpgradeCards)
        {
            ObjectPooler.Instance.ReturnToPool(upgradeCardPoolTag, cardObj);
        }
        activeUpgradeCards.Clear();

        // Spawn upgrade cards from the pool
        foreach (var upgrade in upgrades)
        {
            Debug.Log($"Spawning UpgradeCard with tag: {upgradeCardPoolTag}");
            GameObject cardObj = ObjectPooler.Instance.SpawnFromPool(upgradeCardPoolTag, Vector3.zero, Quaternion.identity);
            cardObj.transform.SetParent(upgradesContainer, false); // Set parent without altering scale
            UpgradeCard card = cardObj.GetComponent<UpgradeCard>();
            if (card != null)
            {
                card.Initialize(upgrade, OnUpgradeButtonClicked);
                activeUpgradeCards.Add(cardObj);
            }
            else
            {
                Debug.LogError("UpgradeCard component not found on the pooled object.");
            }
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

    private void OnUpgradeButtonClicked(Upgrade selectedUpgrade)
    {
        Debug.Log("Upgrade Button Clicked");
        if (onUpgradeSelected != null && selectedUpgrade != null)
        {
            onUpgradeSelected(selectedUpgrade);
            HideUpgradeUI(); // Ensure this is called to hide the panel
        }
        else
        {
            Debug.LogError("onUpgradeSelected callback or selectedUpgrade is not set.");
        }
    }

    public void HideUpgradeUI()
    {
        // Hide the panel
        if (upgradeUIPanel != null)
        {
            upgradeUIPanel.SetActive(false);
            Debug.Log("Upgrade Panel Hidden");
        }
        else
        {
            Debug.LogError("UpgradeUIPanel is not assigned in the UpgradeUIManager.");
        }

        // Return active upgrade cards to the pool
        foreach (var cardObj in activeUpgradeCards)
        {
            Debug.Log($"Returning UpgradeCard to pool with tag: {upgradeCardPoolTag}");
            cardObj.transform.SetParent(ObjectPooler.Instance.transform, false);
            ObjectPooler.Instance.ReturnToPool(upgradeCardPoolTag, cardObj);
        }
        activeUpgradeCards.Clear();
    }
}
