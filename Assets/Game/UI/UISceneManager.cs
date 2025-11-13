// UISceneManager.cs
// Handles scene transitions and game exit functionality
// Attach to a persistent UI manager or button handlers

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages UI-triggered scene transitions and application exit.
/// Use this for restart buttons, main menu navigation, and quit functionality.
/// </summary>
public class UISceneManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string gameplaySceneName = "Gameplay";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Settings")]
    [SerializeField] private bool useAsyncLoading = false;
    [SerializeField] private float restartDelay = 0f; // Optional delay before restart

    /// <summary>
    /// Restart the game by reloading the Gameplay scene.
    /// </summary>
    public void Restart()
    {
        Debug.Log("[UISceneManager] Restarting game...");

        if (restartDelay > 0f)
        {
            Invoke(nameof(LoadGameplayScene), restartDelay);
        }
        else
        {
            LoadGameplayScene();
        }
    }

    /// <summary>
    /// Load the Gameplay scene.
    /// </summary>
    private void LoadGameplayScene()
    {
        if (useAsyncLoading)
        {
            LoadSceneAsync(gameplaySceneName);
        }
        else
        {
            SceneManager.LoadScene(gameplaySceneName);
            Debug.Log($"[UISceneManager] Loaded scene: {gameplaySceneName}");
        }
    }

    /// <summary>
    /// Load a scene asynchronously.
    /// </summary>
    private void LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        if (asyncLoad != null)
        {
            asyncLoad.completed += (operation) =>
            {
                Debug.Log($"[UISceneManager] Async load complete: {sceneName}");
            };
        }
    }

    /// <summary>
    /// Go to Main Menu scene.
    /// </summary>
    public void GoToMainMenu()
    {
        Debug.Log("[UISceneManager] Loading Main Menu...");
        
        if (useAsyncLoading)
        {
            LoadSceneAsync(mainMenuSceneName);
        }
        else
        {
            SceneManager.LoadScene(mainMenuSceneName);
            Debug.Log($"[UISceneManager] Loaded scene: {mainMenuSceneName}");
        }
    }

    /// <summary>
    /// Exit the application.
    /// </summary>
    public void Exit()
    {
        Debug.Log("[UISceneManager] Exiting application...");

#if UNITY_EDITOR
        // Stop play mode in editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit application in build
        Application.Quit();
#endif
    }

    /// <summary>
    /// Restart after a delay (useful for game over screens).
    /// </summary>
    public void RestartWithDelay(float delay)
    {
        Debug.Log($"[UISceneManager] Restarting in {delay} seconds...");
        Invoke(nameof(LoadGameplayScene), delay);
    }

    /// <summary>
    /// Reload the current active scene.
    /// </summary>
    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"[UISceneManager] Reloading current scene: {currentScene}");
        SceneManager.LoadScene(currentScene);
    }

    // ====== Button Click Handlers (wire these to UI buttons) ======

    /// <summary>
    /// Called by Restart button.
    /// </summary>
    public void OnRestartButtonClicked()
    {
        Restart();
    }

    /// <summary>
    /// Called by Main Menu button.
    /// </summary>
    public void OnMainMenuButtonClicked()
    {
        GoToMainMenu();
    }

    /// <summary>
    /// Called by Exit/Quit button.
    /// </summary>
    public void OnExitButtonClicked()
    {
        Exit();
    }
}
