using System;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    // Define a delegate and event for player instantiation
    public event Action<GameObject> OnPlayerInstantiated;

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

                // Tag the player for easy identification
                player.tag = "Player";

                // Invoke the event to notify that the player has been instantiated
                OnPlayerInstantiated?.Invoke(player);
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
