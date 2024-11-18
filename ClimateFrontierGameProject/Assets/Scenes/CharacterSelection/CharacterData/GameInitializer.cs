using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.selectedCharacterData != null)
        {
            // Instantiate the in-game character prefab
            GameObject playerPrefab = GameManager.Instance.selectedCharacterData.inGamePrefab;
            GameObject player = Instantiate(playerPrefab);

            // Assign the CharacterData to the player
            BasePlayer basePlayer = player.GetComponent<BasePlayer>();
            if (basePlayer != null)
            {
                basePlayer.characterData = GameManager.Instance.selectedCharacterData;
                // Initialize the character after assigning characterData
                basePlayer.InitializePlayer();
            }
            else
            {
                Debug.LogError("BasePlayer component not found on player prefab.");
            }
        }
        else
        {
            Debug.LogError("No character data found in GameManager.");
        }
    }
}
