using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject highScoresPanel;
    [SerializeField] private GameObject levelSelectPanel;

    [Header("High Score Display")]
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Audio")]
    [SerializeField] private AudioSource menuAudioSource;
    [SerializeField] private AudioClip buttonClickSound;

    private void Start()
    {
        // Ensure cursor is visible and unlocked for menu navigation
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Show main panel, hide others
        ShowMainPanel();

        // Load and display high score
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null && highScore > 0)
        {
            highScoreText.text = "High Score: " + highScore;
        }
        else if (highScoreText != null)
        {
            highScoreText.text = "No high score yet!";
        }
    }

    private void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (highScoresPanel != null) highScoresPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
    }

    public void ButtonHandlerPlay()
    {
        PlayButtonSound();

        // If we have a level select panel, show it
        if (levelSelectPanel != null)
        {
            mainPanel.SetActive(false);
            levelSelectPanel.SetActive(true);
        }
        else
        {
            // Otherwise load the first game scene directly
            LoadGameScene("Main");
        }
    }

    public void ButtonHandlerLoadLevel(string levelName)
    {
        PlayButtonSound();
        LoadGameScene(levelName);
    }

    private void LoadGameScene(string sceneName)
    {
        // Use the pattern from Lab 2/3 for scene loading
        SceneManager.LoadScene(sceneName);
    }

    public void ButtonHandlerOptions()
    {
        PlayButtonSound();
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void ButtonHandlerHighScores()
    {
        PlayButtonSound();
        mainPanel.SetActive(false);
        highScoresPanel.SetActive(true);
    }

    public void ButtonHandlerBack()
    {
        PlayButtonSound();
        ShowMainPanel();
    }

    public void ButtonHandlerQuit()
    {
        PlayButtonSound();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void ButtonHandlerResetHighScore()
    {
        PlayButtonSound();
        PlayerPrefs.SetInt("HighScore", 0);

        if (highScoreText != null)
        {
            highScoreText.text = "No high score yet!";
        }
    }

    private void PlayButtonSound()
    {
        if (menuAudioSource != null && buttonClickSound != null)
        {
            menuAudioSource.PlayOneShot(buttonClickSound);
        }
    }
}