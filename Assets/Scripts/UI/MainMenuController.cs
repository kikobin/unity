using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Optional Dependencies")]
    [SerializeField] private SceneLoader sceneLoader;

    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = SceneLoader.GameSceneName;

    private void Awake()
    {
        if (sceneLoader == null)
        {
            sceneLoader = FindObjectOfType<SceneLoader>();
        }
    }

    public void OnStartClicked()
    {
        AudioManager audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.PlaySfx(SfxType.UIClick);
        }

        RunResultStore.Reset();

        if (string.IsNullOrWhiteSpace(gameSceneName))
        {
            Debug.LogError("MainMenuController.OnStartClicked: game scene name is null or empty.");
            return;
        }

        if (sceneLoader != null)
        {
            sceneLoader.LoadSceneByName(gameSceneName);
            return;
        }

        SceneLoader.LoadScene(gameSceneName);
    }

    public void OnQuitClicked()
    {
        AudioManager audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.PlaySfx(SfxType.UIClick);
        }

#if UNITY_EDITOR
        Debug.Log("MainMenuController.OnQuitClicked: Application.Quit() called in Editor.");
#endif
        Application.Quit();
    }
}
