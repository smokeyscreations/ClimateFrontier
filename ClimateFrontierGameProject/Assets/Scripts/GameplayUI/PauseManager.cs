using UnityEngine;
using System;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    public bool IsPaused { get; private set; } = false;

    public event Action OnPause;
    public event Action OnResume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Check for Escape key to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (IsPaused) return;
        IsPaused = true;
        Time.timeScale = 0f;
        OnPause?.Invoke();
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        if (!IsPaused) return;
        IsPaused = false;
        Time.timeScale = 1f;
        OnResume?.Invoke();
        Debug.Log("Game Resumed");
    }
}
