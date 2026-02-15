using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Data Sources (optional, auto-resolved if empty)")]
    [SerializeField] private GameRoot gameRoot;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private PlayerHealth playerHealth;

    private bool subscribed;

    private void OnEnable()
    {
        ResolveReferences();
        ValidateTextReferences();
        SubscribeEvents();
        RefreshAll();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void ResolveReferences()
    {
        if (gameRoot == null)
        {
            gameRoot = GameRoot.Instance;
            if (gameRoot == null)
            {
                gameRoot = FindObjectOfType<GameRoot>();
            }
        }

        if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager>();
        }

        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
        }
    }

    private void SubscribeEvents()
    {
        if (subscribed)
        {
            return;
        }

        if (gameRoot != null)
        {
            gameRoot.ScoreChanged += OnScoreChanged;
            gameRoot.WaveChanged += OnWaveChanged;
            gameRoot.TimeChanged += OnTimeChanged;
        }
        else
        {
            Debug.LogWarning("HUDController: GameRoot not found. Score/Wave/Time events are unavailable.");
        }

        if (waveManager != null)
        {
            waveManager.WaveChanged += OnWaveChanged;
        }

        if (playerHealth != null)
        {
            playerHealth.HealthChanged += OnHealthChanged;
        }
        else
        {
            Debug.LogWarning("HUDController: PlayerHealth not found. HP updates are unavailable.");
        }

        subscribed = true;
    }

    private void UnsubscribeEvents()
    {
        if (!subscribed)
        {
            return;
        }

        if (gameRoot != null)
        {
            gameRoot.ScoreChanged -= OnScoreChanged;
            gameRoot.WaveChanged -= OnWaveChanged;
            gameRoot.TimeChanged -= OnTimeChanged;
        }

        if (waveManager != null)
        {
            waveManager.WaveChanged -= OnWaveChanged;
        }

        if (playerHealth != null)
        {
            playerHealth.HealthChanged -= OnHealthChanged;
        }

        subscribed = false;
    }

    private void RefreshAll()
    {
        if (playerHealth != null)
        {
            SetHpText(playerHealth.CurrentHp, playerHealth.MaxHp);
        }
        else
        {
            SetHpText(0, 0);
        }

        if (gameRoot != null)
        {
            SetScoreText(gameRoot.Score);
            SetWaveText(gameRoot.CurrentWave);
            SetTimeText(gameRoot.RunTimeSeconds);
            return;
        }

        if (waveManager != null)
        {
            SetWaveText(waveManager.CurrentWave);
        }
        else
        {
            SetWaveText(0);
        }

        SetScoreText(0);
        SetTimeText(0f);
    }

    private void OnHealthChanged(int currentHp, int maxHp)
    {
        SetHpText(currentHp, maxHp);
    }

    private void OnScoreChanged(int score)
    {
        SetScoreText(score);
    }

    private void OnWaveChanged(int wave)
    {
        SetWaveText(wave);
    }

    private void OnTimeChanged(float seconds)
    {
        SetTimeText(seconds);
    }

    private void SetHpText(int currentHp, int maxHp)
    {
        if (hpText == null)
        {
            return;
        }

        hpText.text = $"HP: {Mathf.Max(0, currentHp)}/{Mathf.Max(0, maxHp)}";
    }

    private void SetWaveText(int wave)
    {
        if (waveText == null)
        {
            return;
        }

        waveText.text = $"Wave: {Mathf.Max(0, wave)}";
    }

    private void SetTimeText(float seconds)
    {
        if (timeText == null)
        {
            return;
        }

        timeText.text = $"Time: {Mathf.Max(0f, seconds):0.0}s";
    }

    private void SetScoreText(int score)
    {
        if (scoreText == null)
        {
            return;
        }

        scoreText.text = $"Score: {Mathf.Max(0, score)}";
    }

    private void ValidateTextReferences()
    {
        if (hpText == null)
        {
            Debug.LogError("HUDController: HP Text is not assigned.");
        }

        if (waveText == null)
        {
            Debug.LogError("HUDController: Wave Text is not assigned.");
        }

        if (timeText == null)
        {
            Debug.LogError("HUDController: Time Text is not assigned.");
        }

        if (scoreText == null)
        {
            Debug.LogError("HUDController: Score Text is not assigned.");
        }
    }
}
