using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CharacterData selectedCharacterData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSelectedCharacter(CharacterData characterData)
    {
        // Create a runtime clone to prevent modifying the original asset
        selectedCharacterData = Instantiate(characterData);
    }
}
