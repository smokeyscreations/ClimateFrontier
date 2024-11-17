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
    public Button lockInButton;

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
    }

    void SetupCharacterButtons()
    {
        // Set up Character Button 1
        characterButton1.image.sprite = characterDataList[0].icon;
        characterButton1.onClick.AddListener(() => OnCharacterButtonClicked(0));

        // Set up Character Button 2
        characterButton2.image.sprite = characterDataList[1].icon;
        characterButton2.onClick.AddListener(() => OnCharacterButtonClicked(1));
    }

    void OnCharacterButtonClicked(int index)
    {
        selectedCharacterIndex = index;
        DisplaySelectedCharacter();
        lockInButton.interactable = true;
    }

    void DisplaySelectedCharacter()
    {
        // Destroy the previous character instance if it exists
        if (currentCharacterInstance != null)
        {
            Destroy(currentCharacterInstance);
        }

        // Instantiate the selected character prefab
        CharacterData selectedCharacter = characterDataList[selectedCharacterIndex];
        currentCharacterInstance = Instantiate(selectedCharacter.characterPrefab, characterRenderPosition.position, Quaternion.identity);

        // Set layer to CharacterRenderLayer
        int characterRenderLayer = LayerMask.NameToLayer("CharacterRenderLayer");
        SetLayerRecursively(currentCharacterInstance, characterRenderLayer);

        // Make the character look at the camera
        LookAtCamera(currentCharacterInstance.transform, characterRenderCamera.transform);

        // Update character information UI
        characterNameText.text = selectedCharacter.characterName;
        characterDescriptionText.text = selectedCharacter.description;
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
        if (null == obj)
            return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
                continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    void OnLockInButtonClicked()
    {
        if (selectedCharacterIndex >= 0)
        {
            // Store the selected character index
            PlayerPrefs.SetInt("SelectedCharacterIndex", selectedCharacterIndex);
            // Optionally, save more data or perform additional logic

            // Load the next scene (e.g., the main game scene)
            SceneManager.LoadScene("EnvironmentTest");
        }
    }
}
