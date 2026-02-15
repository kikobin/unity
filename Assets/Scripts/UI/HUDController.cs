using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    private const string LabelColorHex = "#FFD166";
    private const string ValueColorHex = "#FFF4D6";

    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Data Sources (optional, auto-resolved if empty)")]
    [SerializeField] private GameRoot gameRoot;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Visual Tuning")]
    [SerializeField, Min(1f)] private float hudFontSize = 38f;
    [SerializeField, Min(0.5f)] private float centerStatsScale = 1.15f;

    private bool subscribed;

    private void OnEnable()
    {
        ResolveReferences();
        ValidateTextReferences();
        ApplyVisualStyle();
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

        int clampedCurrent = Mathf.Max(0, currentHp);
        int clampedMax = Mathf.Max(0, maxHp);
        hpText.text = FormatStatLine("HP", $"<b>{clampedCurrent:N0}</b><color=#FFFFFFAA> / </color>{clampedMax:N0}");
    }

    private void SetWaveText(int wave)
    {
        if (waveText == null)
        {
            return;
        }

        waveText.text = FormatStatLine("WAVE", $"<b>{Mathf.Max(0, wave):N0}</b>");
    }

    private void SetTimeText(float seconds)
    {
        if (timeText == null)
        {
            return;
        }

        timeText.text = FormatStatLine("TIME", $"<b>{Mathf.Max(0f, seconds):0.0}</b><color=#FFFFFFCC>s</color>");
    }

    private void SetScoreText(int score)
    {
        if (scoreText == null)
        {
            return;
        }

        scoreText.text = FormatStatLine("SCORE", $"<b>{Mathf.Max(0, score):N0}</b>");
    }

    private void ApplyVisualStyle()
    {
        ApplyTextStyle(hpText, TextAlignmentOptions.TopLeft, 1f);
        ApplyTextStyle(timeText, TextAlignmentOptions.TopRight, 1f);
        ApplyTextStyle(scoreText, TextAlignmentOptions.Top, centerStatsScale);
        ApplyTextStyle(waveText, TextAlignmentOptions.Top, centerStatsScale);
    }

    private void ApplyTextStyle(TextMeshProUGUI text, TextAlignmentOptions alignment, float scale)
    {
        if (text == null)
        {
            return;
        }

        text.enableAutoSizing = false;
        text.alignment = alignment;
        text.fontSize = hudFontSize * scale;
    }

    private static string FormatStatLine(string label, string value)
    {
        return $"<uppercase><color={LabelColorHex}>{label}</color></uppercase><color=#FFFFFFBB>  </color><color={ValueColorHex}>{value}</color>";
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
