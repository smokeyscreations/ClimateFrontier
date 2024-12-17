using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.OnPause += HandlePause;
            PauseManager.Instance.OnResume += HandleResume;
        }
    }

    private void OnDisable()
    {
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.OnPause -= HandlePause;
            PauseManager.Instance.OnResume -= HandleResume;
        }
    }

    private void Start()
    {
        
        resumeButton.onClick.AddListener(OnResumeClicked);
        optionsButton.onClick.AddListener(OnOptionsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

       
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void HandlePause()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    private void HandleResume()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void OnResumeClicked()
    {
        PauseManager.Instance.ResumeGame();
    }

    private void OnOptionsClicked()
    {
        
        Debug.Log("Options button clicked. Show options menu if available.");
    }

    private void OnQuitClicked()
    {
        
        Time.timeScale = 1f; 
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
