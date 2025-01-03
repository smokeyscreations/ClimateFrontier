using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("UI References")]
    public Button characterButton1;
    public Button characterButton2;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI characterDescriptionText;
    public TextMeshProUGUI classText;
    public Button lockInButton;
    public UIAnimator uiAnimator;
    public Button upgradeButton; // New Upgrade button

    [Header("Character Data")]
    public List<CharacterData> characterDataList;

    [Header("Character Rendering")]
    public Transform characterRenderPosition;
    public Camera characterRenderCamera; // Reference to the camera
    private GameObject currentCharacterInstance;

    private int selectedCharacterIndex = -1;

    void Start()
    {
        // Assign button images and click events
        SetupCharacterButtons();

        // Initially disable the lock-in button
        lockInButton.interactable = false;
        lockInButton.onClick.AddListener(OnLockInButtonClicked);

        // Hide the upgrade button initially
        upgradeButton.gameObject.SetActive(false);
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
    }

    void SetupCharacterButtons()
    {
        // Ensure there are enough characters
        if (characterDataList.Count >= 2)
        {
            // Set up Character Button 1
            characterButton1.image.sprite = characterDataList[0].icon;
            characterButton1.onClick.AddListener(() => OnCharacterButtonClicked(0));

            // Set up Character Button 2
            characterButton2.image.sprite = characterDataList[1].icon;
            characterButton2.onClick.AddListener(() => OnCharacterButtonClicked(1));
        }
        else
        {
            Debug.LogError("Not enough character data in characterDataList.");
        }
    }

    void OnCharacterButtonClicked(int index)
    {
        selectedCharacterIndex = index;
        lockInButton.interactable = true;

        // Display the character model immediately
        DisplaySelectedCharacterModel();

        // Play the bars animation
        uiAnimator.PlayBarsAnimation();

        // Start the coroutine to update the UI text during the animation
        StartCoroutine(UpdateUIDuringAnimation());

        // Now that a character is selected, show the upgrade button
        upgradeButton.gameObject.SetActive(true);
    }

    // Correct Coroutine Signature
    IEnumerator UpdateUIDuringAnimation()
    {
        // Wait until the bars have fully covered the original text
        yield return new WaitForSeconds(uiAnimator.animationDuration / 2);

        // Update the character information UI
        CharacterData selectedCharacter = characterDataList[selectedCharacterIndex];

        UpdateCharacterInfoUI();

        // Reveal the info panel
        uiAnimator.ShowInfoPanel();
    }

    void DisplaySelectedCharacterModel()
    {
        // Destroy the previous character instance if it exists
        if (currentCharacterInstance != null)
        {
            Destroy(currentCharacterInstance);
        }

        // Instantiate the selected character's selection prefab
        CharacterData selectedCharacter = characterDataList[selectedCharacterIndex];
        if (selectedCharacter.selectionPrefab != null)
        {
            currentCharacterInstance = Instantiate(
                selectedCharacter.selectionPrefab,
                characterRenderPosition.position,
                Quaternion.identity
            );

            // Set layer to CharacterRenderLayer
            int characterRenderLayer = LayerMask.NameToLayer("CharacterRenderLayer");
            SetLayerRecursively(currentCharacterInstance, characterRenderLayer);

            // Make the character look at the camera
            LookAtCamera(currentCharacterInstance.transform, characterRenderCamera.transform);
        }
        else
        {
            Debug.LogError("Selection prefab is missing for " + selectedCharacter.characterName);
        }
    }

    void UpdateCharacterInfoUI()
    {
        // Update character information UI
        CharacterData selectedCharacter = characterDataList[selectedCharacterIndex];
        characterNameText.text = selectedCharacter.characterName;
        characterDescriptionText.text = selectedCharacter.description;
        classText.text = selectedCharacter.classString;
    }

    void LookAtCamera(Transform characterTransform, Transform cameraTransform)
    {
        // Calculate the direction from the character to the camera
        Vector3 direction = cameraTransform.position - characterTransform.position;

        // Zero out the Y component to prevent tilting up/down
        direction.y = 0;

        // Create a rotation facing the camera
        Quaternion rotation = Quaternion.LookRotation(direction);

        // Apply the rotation to the character
        characterTransform.rotation = rotation;
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null)
            return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (child == null)
                continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    void OnLockInButtonClicked()
    {
        if (selectedCharacterIndex >= 0)
        {
            CharacterData selectedCharacter = characterDataList[selectedCharacterIndex];

            // Check if GameManager exists
            if (GameManager.Instance != null)
            {
                // Set the selected character in the GameManager
                GameManager.Instance.SetSelectedCharacter(selectedCharacter);

                // Load the next scene (gameplay)
                SceneManager.LoadScene("TheAbyss");
            }
            else
            {
                Debug.LogError("GameManager instance not found.");
            }
        }
        else
        {
            Debug.LogWarning("No character selected.");
        }
    }

    void OnUpgradeButtonClicked()
    {
        if (selectedCharacterIndex >= 0)
        {
            CharacterData selectedCharacter = characterDataList[selectedCharacterIndex];

            if (GameManager.Instance != null)
            {
                // Set the selected character in the GameManager
                GameManager.Instance.SetSelectedCharacter(selectedCharacter);

                // Load the upgrade scene
                SceneManager.LoadScene("CharacterUpgradeScene");
            }
            else
            {
                Debug.LogError("GameManager instance not found.");
            }
        }
        else
        {
            Debug.LogWarning("No character selected for upgrade.");
        }
    }
}
