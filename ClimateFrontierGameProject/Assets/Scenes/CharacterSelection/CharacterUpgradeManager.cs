using UnityEngine;

public class CharacterUpgradeManager : MonoBehaviour
{
    public Transform characterSpawnPoint;
    private GameObject currentCharacterInstance;
    private Animator animator;
    // Call this method from CharacterUpgradeUI once you know which character to display
    public void DisplayCharacter(CharacterData characterData, float scale = 1.0f)
    {
        if (characterData == null || characterData.selectionPrefab == null)
        {
            Debug.LogWarning("No valid character prefab to display.");
            return;
        }

        // Destroy old instance if exists
        if (currentCharacterInstance != null)
        {
            Destroy(currentCharacterInstance);
        }

        currentCharacterInstance = Instantiate(
            characterData.selectionPrefab,
            characterSpawnPoint.position,
            characterSpawnPoint.rotation
        );
        // After you've created the instance and done other setup
       animator = currentCharacterInstance.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Upgrade");
        }

        // Adjust scale
        currentCharacterInstance.transform.localScale = Vector3.one * scale;

        FaceTowardsCamera(currentCharacterInstance.transform);
    }

    private void FaceTowardsCamera(Transform characterTransform)
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Vector3 direction = mainCam.transform.position - characterTransform.position;
            direction.y = 0;
            characterTransform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
