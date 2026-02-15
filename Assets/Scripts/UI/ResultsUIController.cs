using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsUIController : MonoBehaviour
{
    private const string LabelColorHex = "#A7F432";
    private const string ValueColorHex = "#F7F9FC";

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

        SetText(resultTitleText, $"<b><color={LabelColorHex}>{title}</color></b>", nameof(resultTitleText));
        SetText(finalScoreText, FormatStatLine("SCORE", $"<b>{Mathf.Max(0, score):N0}</b>"), nameof(finalScoreText));
        SetText(finalWaveText, FormatStatLine("WAVE", $"<b>{Mathf.Max(0, wave):N0}</b>"), nameof(finalWaveText));
        SetText(finalTimeText, FormatStatLine("TIME", $"<b>{Mathf.Max(0f, timeSeconds):0.0}</b><color=#FFFFFFCC>s</color>"), nameof(finalTimeText));
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

    private static string FormatStatLine(string label, string value)
    {
        return $"<uppercase><color={LabelColorHex}>{label}</color></uppercase><color=#FFFFFFBB>  </color><color={ValueColorHex}>{value}</color>";
    }
}
