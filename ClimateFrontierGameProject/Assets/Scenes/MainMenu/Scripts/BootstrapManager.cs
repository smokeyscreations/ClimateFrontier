using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapManager : MonoBehaviour
{
    [SerializeField] private string initialSceneName = "MainMenu";

    private void Start()
    {

        Invoke("LoadInitialScene", 0.1f);
    }

    private void LoadInitialScene()
    {
        SceneManager.LoadScene(initialSceneName);
    }
}
