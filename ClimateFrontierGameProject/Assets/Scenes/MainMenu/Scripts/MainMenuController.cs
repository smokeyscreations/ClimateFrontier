using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void LoadCharacterSelection()
    {
        SceneManager.LoadScene("CharacterSelection"); 
    }
}
