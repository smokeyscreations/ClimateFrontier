using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CharacterData selectedCharacterData;

    public int globalGold = 0;
    public int levelGold = 0; // gold earned during this level

    // Event fired when gold changes (pass the new local gold amount)
    public event System.Action<int> OnGoldChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSelectedCharacter(CharacterData characterData)
    {
        selectedCharacterData = Instantiate(characterData);
    }

    // Call this at the start of each level to reset level gold
    public void ResetLevelGold()
    {
        levelGold = 0;
        OnGoldChanged?.Invoke(levelGold);
    }

    public void AddGold(int amount)
    {
        globalGold += amount;
        levelGold += amount;

        // Save globalGold if you wish (e.g., PlayerPrefs)
        PlayerPrefs.SetInt("GlobalGold", globalGold);
        PlayerPrefs.Save();

        Debug.Log($"Added {amount} gold. Total global gold: {globalGold}, level gold: {levelGold}.");

        OnGoldChanged?.Invoke(levelGold);
    }

    private void Start()
    {
        // Load global gold from PlayerPrefs if available
        globalGold = PlayerPrefs.GetInt("GlobalGold", 0);
    }
}
