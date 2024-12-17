using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    public static GameInitializer Instance;

    public event Action<GameObject> OnPlayerInstantiated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Register the scene loaded callback
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if this is a gameplay scene
        if (IsGameplayScene(scene))
        {
            // Reset level gold each time you load a gameplay scene
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetLevelGold();
            }

            // Now spawn the player again
            SpawnPlayer();

            // Start the timer if needed
            if (GameTimer.Instance != null)
            {
                GameTimer.Instance.StartTimer(600f);
                Debug.Log("Game Timer started successfully from GameInitializer.");
            }
            else
            {
                Debug.LogWarning("GameTimer instance not found, cannot start timer.");
            }
        }
    }

    private bool IsGameplayScene(Scene scene)
    {
        // Implement logic to identify if this scene is a gameplay scene
        // For example: return scene.name == "EnvironmentTest";
        return scene.name == "TheAbyss";
    }

    private void SpawnPlayer()
    {
        if (GameManager.Instance != null && GameManager.Instance.selectedCharacterData != null)
        {
            GameObject playerPrefab = GameManager.Instance.selectedCharacterData.inGamePrefab;
            GameObject player = Instantiate(playerPrefab);

            BasePlayer basePlayer = player.GetComponent<BasePlayer>();
            if (basePlayer != null)
            {
                basePlayer.characterData = GameManager.Instance.selectedCharacterData;
                basePlayer.InitializePlayer();

                player.tag = "Player";

                OnPlayerInstantiated?.Invoke(player);
                Debug.Log("GameInitializer: Player instantiated and event fired.");
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
