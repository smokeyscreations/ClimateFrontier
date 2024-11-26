using System;
using UnityEngine;

public class TestInitializer : MonoBehaviour
{
    public static TestInitializer Instance { get; private set; }

    [Header("Test Settings")]
    public GameObject playerPrefab; // Assign via Inspector
    public CharacterData testCharacterData; // Assign via Inspector
    public event Action<GameObject> OnPlayerInstantiated;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize GameManager if not already present
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.Log("TestInitializer: GameManager instance not found. Creating a new one.");
            GameObject gmObject = new GameObject("GameManager");
            gameManager = gmObject.AddComponent<GameManager>();
            DontDestroyOnLoad(gmObject);
        }
        else
        {
            Debug.Log("TestInitializer: GameManager instance found.");
        }

        // Assign CharacterData to GameManager
        if (testCharacterData != null)
        {
            playerPrefab.tag = "Player";
            Debug.Log("TestInitializer: Assigning CharacterData to GameManager.");
            gameManager.SetSelectedCharacter(testCharacterData);
        }
        else
        {
            Debug.LogError("TestInitializer: testCharacterData is not assigned.");
        }
    }

    void Start()
    {
        // Instantiate the Player after GameManager is set up
        if (GameManager.Instance != null && GameManager.Instance.selectedCharacterData != null)
        {
            Debug.Log("TestInitializer: Instantiating player prefab.");
            GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            BasePlayer basePlayer = player.GetComponent<BasePlayer>();
            if (basePlayer != null)
            {
                Debug.Log("TestInitializer: Assigning characterData to BasePlayer.");
                basePlayer.characterData = GameManager.Instance.selectedCharacterData;

                basePlayer.InitializePlayer();

                player.tag = "Player"; // Ensure the tag is set
                Debug.Log("TestInitializer: Player tag set to 'Player'.");

                OnPlayerInstantiated?.Invoke(player);
                Debug.Log("TestInitializer: OnPlayerInstantiated event invoked.");
            }
            else
            {
                Debug.LogError("TestInitializer: BasePlayer component not found on player prefab.");
            }
        }
        else
        {
            Debug.LogError("TestInitializer: No character data found in GameManager.");
        }
    }
}
