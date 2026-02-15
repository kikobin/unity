using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsUIController : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI resultTitleText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalWaveText;
    [SerializeField] private TextMeshProUGUI finalTimeText;

    [Header("Button References")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Optional Dependencies")]
    [SerializeField] private SceneLoader sceneLoader;

    private void Awake()
    {
        if (sceneLoader == null)
        {
            sceneLoader = FindObjectOfType<SceneLoader>();
        }

        ValidateReferences();
    }

    private void OnEnable()
    {
        RefreshFromResultStore();
    }

    public void OnRestartClicked()
    {
        RunResultStore.Reset();
        LoadSceneSafe(SceneLoader.GameSceneName);
    }

    public void OnMainMenuClicked()
    {
        RunResultStore.Reset();
        LoadSceneSafe(SceneLoader.MainMenuSceneName);
    }

    private void RefreshFromResultStore()
    {
        bool hasResult = RunResultStore.TryGetResult(out bool isWin, out int score, out int wave, out float timeSeconds);
        string title = hasResult ? (isWin ? "YOU WIN" : "YOU LOSE") : "RESULTS";

        SetText(resultTitleText, title, nameof(resultTitleText));
        SetText(finalScoreText, $"Score: {Mathf.Max(0, score)}", nameof(finalScoreText));
        SetText(finalWaveText, $"Wave: {Mathf.Max(0, wave)}", nameof(finalWaveText));
        SetText(finalTimeText, $"Time: {Mathf.Max(0f, timeSeconds):0.0}s", nameof(finalTimeText));
    }

    private void LoadSceneSafe(string sceneName)
    {
        if (sceneLoader != null)
        {
            sceneLoader.LoadSceneByName(sceneName);
            return;
        }

        SceneLoader.LoadScene(sceneName);
    }

    private void SetText(TextMeshProUGUI textComponent, string value, string fieldName)
    {
        if (textComponent == null)
        {
            Debug.LogWarning($"ResultsUIController: '{fieldName}' is not assigned.");
            return;
        }

        textComponent.text = value;
    }

    private void ValidateReferences()
    {
        if (resultTitleText == null)
        {
            Debug.LogWarning("ResultsUIController: Result Title Text is not assigned.");
        }

        if (finalScoreText == null)
        {
            Debug.LogWarning("ResultsUIController: Final Score Text is not assigned.");
        }

        if (finalWaveText == null)
        {
            Debug.LogWarning("ResultsUIController: Final Wave Text is not assigned.");
        }

        if (finalTimeText == null)
        {
            Debug.LogWarning("ResultsUIController: Final Time Text is not assigned.");
        }

        if (restartButton == null)
        {
            Debug.LogWarning("ResultsUIController: Restart Button is not assigned.");
        }

        if (mainMenuButton == null)
        {
            Debug.LogWarning("ResultsUIController: Main Menu Button is not assigned.");
        }
    }
}
