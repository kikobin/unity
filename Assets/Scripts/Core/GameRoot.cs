using UnityEngine;
using System;

public class GameRoot : MonoBehaviour
{
    public static GameRoot Instance { get; private set; }

    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private int currentWave;
    [SerializeField] private float runTimeSeconds;
    [SerializeField, Range(0.2f, 1f)] private float timeTickInterval = 0.5f;
    [SerializeField] private bool gameEnded;

    private float timeTickAccumulator;

    public GameState CurrentState { get; private set; } = GameState.MainMenu;
    public int Score { get; private set; }
    public int CurrentWave => currentWave;
    public float RunTimeSeconds => runTimeSeconds;
    public bool IsPlaying => CurrentState == GameState.Playing;

    public event Action<int> ScoreChanged;
    public event Action<int> WaveChanged;
    public event Action<float> TimeChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("GameRoot.Awake: another GameRoot already exists. Replacing old instance reference.");
        }

        Instance = this;
        Score = 0;
        currentWave = 0;
        runTimeSeconds = 0f;
        timeTickAccumulator = 0f;
        gameEnded = false;
        if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager>();
        }

        NotifyScoreChanged();
        NotifyWaveChanged();
        NotifyTimeChanged();

        SetState(GameState.Playing);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (CurrentState == GameState.Playing)
        {
            runTimeSeconds += Time.deltaTime;
            timeTickAccumulator += Time.deltaTime;

            if (timeTickAccumulator >= timeTickInterval)
            {
                timeTickAccumulator = 0f;
                NotifyTimeChanged();
            }
        }
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
    }

    public void AddScore(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Score += amount;
        NotifyScoreChanged();
    }

    public void SetCurrentWave(int waveNumber)
    {
        int newWave = Mathf.Max(0, waveNumber);
        if (newWave == currentWave)
        {
            return;
        }

        currentWave = newWave;
        NotifyWaveChanged();
    }

    public void EndGame(bool win)
    {
        if (gameEnded)
        {
            Debug.LogWarning("GameRoot.EndGame: already called, ignoring duplicate request.");
            return;
        }

        gameEnded = true;
        SetState(win ? GameState.Win : GameState.Lose);

        RunResultStore.SetResult(
            win,
            Score,
            currentWave,
            runTimeSeconds);

        SetState(GameState.Results);

        if (sceneLoader == null)
        {
            Debug.LogError("GameRoot.EndGame: SceneLoader reference is not assigned.");
            return;
        }

        sceneLoader.LoadSceneByName("Results");
    }

    private void NotifyScoreChanged()
    {
        ScoreChanged?.Invoke(Score);
    }

    private void NotifyWaveChanged()
    {
        WaveChanged?.Invoke(currentWave);
    }

    private void NotifyTimeChanged()
    {
        TimeChanged?.Invoke(runTimeSeconds);
    }

    private void OnValidate()
    {
        timeTickInterval = Mathf.Clamp(timeTickInterval, 0.2f, 1f);
    }
}
