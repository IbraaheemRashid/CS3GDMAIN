using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("Menu Elements")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Audio")]
    [SerializeField] private AudioSource menuAudioSource;
    [SerializeField] private AudioClip buttonClickSound;

    private bool isPaused = false;
    private FirstPersonController playerController;

    void Start()
    {
        playerController = FindObjectOfType<FirstPersonController>();

        // Ensure panels are hidden at start
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    void Update()
    {
        // Toggle pause menu with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        // Show/hide pause menu
        pauseMenuPanel.SetActive(isPaused);

        // Stop/start time
        Time.timeScale = isPaused ? 0f : 1f;

        // Toggle cursor visibility and control lock
        if (playerController != null)
        {
            playerController.SetCursorState(!isPaused);
        }
        else
        {
            Cursor.visible = isPaused;
            Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        }

        // Hide options panel if returning to game
        if (!isPaused && optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    public void ButtonHandlerResume()
    {
        PlayButtonSound();
        TogglePause();
    }

    public void ButtonHandlerOptions()
    {
        PlayButtonSound();
        pauseMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void ButtonHandlerBackToPause()
    {
        PlayButtonSound();
        optionsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    public void ButtonHandlerSaveGame()
    {
        PlayButtonSound();
        // Call GameManager save function
        GameManager.Instance.SaveGame();
    }

    public void ButtonHandlerLoadGame()
    {
        PlayButtonSound();
        // Call GameManager load function
        GameManager.Instance.LoadGame();
        // TogglePause(); // Resume game after loading
    }

    public void ButtonHandlerMainMenu()
    {
        PlayButtonSound();
        // Reset time scale before loading new scene
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void PlayButtonSound()
    {
        if (menuAudioSource != null && buttonClickSound != null)
        {
            menuAudioSource.PlayOneShot(buttonClickSound);
        }
    }
}