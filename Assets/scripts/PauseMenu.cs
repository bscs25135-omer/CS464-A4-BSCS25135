using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    [SerializeField] GameObject panel;
    [SerializeField] GameObject firstButton;
    [SerializeField] LevelLoader loader;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject gameOverPanel;

    void Awake()
    {
        IsPaused = false;
    }

    void Update()
    {
        bool escPressed = false;
#if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            escPressed = true;
        }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER || !ENABLE_INPUT_SYSTEM
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escPressed = true;
        }
#endif

        if (escPressed)
        {
            if (winPanel != null && winPanel.activeInHierarchy) return;
            if (gameOverPanel != null && gameOverPanel.activeInHierarchy) return;

            if (IsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        if (winPanel != null && winPanel.activeInHierarchy) return;
        if (gameOverPanel != null && gameOverPanel.activeInHierarchy) return;

        IsPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        if (panel != null) panel.SetActive(true);
        if (EventSystem.current != null && firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        if (panel != null) panel.SetActive(false);
    }

    public void Restart()
    {
        Resume();
        if (loader != null) loader.Reload();
    }

    public void QuitToMenu()
    {
        Resume();
        if (loader != null) loader.LoadMenu();
    }
}
