using System;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    public float GameDuration { get; private set; } = 600f; // Default 10 minutes
    public float ElapsedTime { get; private set; } = 0f;
    public bool IsRunning { get; private set; } = false;

    // Example events
    public event Action OnTimerStarted;
    public event Action OnTimerStopped;
    public event Action OnTimerEnded;
    public event Action<float> OnTimeUpdated; // passes elapsed time
    // You could add interval-specific events too, like OnEvery2_5MinutesPassed.

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (IsRunning)
        {
            ElapsedTime += Time.deltaTime;

            // Dispatch time updated event
            OnTimeUpdated?.Invoke(ElapsedTime);

            if (ElapsedTime >= GameDuration)
            {
                ElapsedTime = GameDuration;
                StopTimer();
                OnTimerEnded?.Invoke();
            }
        }
    }

    public void StartTimer(float duration)
    {
        GameDuration = duration;
        ElapsedTime = 0f;
        IsRunning = true;
        OnTimerStarted?.Invoke();
    }

    public void StopTimer()
    {
        IsRunning = false;
        OnTimerStopped?.Invoke();
    }

    public void PauseTimer()
    {
        IsRunning = false;
    }

    public void ResumeTimer()
    {
        IsRunning = true;
    }
}
