using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public const string MainMenuSceneName = "MainMenu";
    public const string GameSceneName = "Game";
    public const string ResultsSceneName = "Results";

    private bool sceneLoadRequested;

    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("SceneLoader.LoadSceneByName: scene name is null or empty.");
            return;
        }

        if (sceneLoadRequested)
        {
            Debug.LogWarning($"SceneLoader.LoadSceneByName: scene load already requested, ignoring '{sceneName}'.");
            return;
        }

        sceneLoadRequested = true;
        SceneManager.LoadScene(sceneName);
    }

    public void RestartGameScene()
    {
        LoadGameScene();
    }

    public void LoadGameScene()
    {
        LoadSceneByName(GameSceneName);
    }

    public void LoadMainMenuScene()
    {
        LoadSceneByName(MainMenuSceneName);
    }

    public void LoadResultsScene()
    {
        LoadSceneByName(ResultsSceneName);
    }

    public static void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("SceneLoader.LoadScene: scene name is null or empty.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
