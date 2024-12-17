using UnityEngine;
using TMPro;
using MoreMountains.Tools;

public class GameTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private MMProgressBar timerProgressBar;

    private float gameDuration; // Will get from GameTimer.

    private void OnEnable()
    {
        // Subscribe to GameTimer events
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.OnTimeUpdated += HandleTimeUpdated;
            GameTimer.Instance.OnTimerStarted += HandleTimerStarted;
        }
    }

    private void OnDisable()
    {
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.OnTimeUpdated -= HandleTimeUpdated;
            GameTimer.Instance.OnTimerStarted -= HandleTimerStarted;
        }
    }

    private void HandleTimerStarted()
    {
        gameDuration = GameTimer.Instance.GameDuration;
        UpdateUI(GameTimer.Instance.ElapsedTime);
    }

    private void HandleTimeUpdated(float elapsed)
    {
        UpdateUI(elapsed);
    }

    private void UpdateUI(float elapsed)
    {
        float remainingTime = gameDuration - elapsed;
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";

        float normalizedValue = elapsed / gameDuration;
        timerProgressBar.UpdateBar01(normalizedValue);
    }
}
