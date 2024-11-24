using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private int mainMenuMusicIndex = 0;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(mainMenuMusicIndex);
        }
        else
        {
            Debug.LogError("AudioManager instance not found. Ensure AudioManager is in the Bootstrap scene and persists across scenes.");
        }
    }

    /// <summary>
    /// Loads the Options Menu scene.
    /// </summary>
    public void LoadOptionsMenu()
    {
        SceneManager.LoadScene("Options");
    }

    public void LoadCharacterSelection()
    {
        SceneManager.LoadScene("CharacterSelection");
    }

    /// <summary>
    /// Method to be called when clicking the Options button.
    /// </summary>
    public void OnOptionsButtonPressed()
    {
        LoadOptionsMenu();
    }

    /// <summary>
    /// Method to be called when clicking the Quit button.
    /// </summary>
    public void OnQuitButtonPressed()
    {
        Application.Quit();
        Debug.Log("Application Quit.");
    }
}
