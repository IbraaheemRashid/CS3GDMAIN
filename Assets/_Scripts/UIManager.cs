using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Singleton pattern
    public static UIManager Instance { get; private set; }

    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI itemCounterText;
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private Image detectionMeter;
    [SerializeField] private GameObject miniMap;

    [Header("Game Status")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject missionCompletePanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Tutorial")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;

    private int totalItems = 0;
    private int collectedItems = 0;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Hide game status panels
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (missionCompletePanel != null) missionCompletePanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
    }

    public void SetTotalItems(int count)
    {
        totalItems = count;
        UpdateItemCounter();
    }

    public void AddCollectedItem()
    {
        collectedItems++;
        UpdateItemCounter();

        // Check if all items are collected
        if (collectedItems >= totalItems)
        {
            // Trigger mission complete functionality when implemented
        }
    }

    private void UpdateItemCounter()
    {
        if (itemCounterText != null)
        {
            itemCounterText.text = $"Items: {collectedItems}/{totalItems}";
        }
    }

    public void UpdateObjective(string objective)
    {
        if (objectiveText != null)
        {
            objectiveText.text = objective;
        }
    }

    public void UpdateDetectionMeter(float detectionValue)
    {
        if (detectionMeter != null)
        {
            // Clamp value between 0 and 1
            detectionValue = Mathf.Clamp01(detectionValue);
            detectionMeter.fillAmount = detectionValue;

            // Change color based on detection level
            if (detectionValue < 0.3f)
            {
                detectionMeter.color = Color.green;
            }
            else if (detectionValue < 0.7f)
            {
                detectionMeter.color = Color.yellow;
            }
            else
            {
                detectionMeter.color = Color.red;
            }
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            // Show cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Pause game
            Time.timeScale = 0f;
        }
    }

    public void ShowMissionComplete(int score)
    {
        if (missionCompletePanel != null)
        {
            missionCompletePanel.SetActive(true);

            if (finalScoreText != null)
            {
                finalScoreText.text = $"Final Score: {score}";
            }

            // Show cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Pause game
            Time.timeScale = 0f;
        }
    }

    public void ShowTutorial(string message, float duration = 5.0f)
    {
        if (tutorialPanel != null && tutorialText != null)
        {
            tutorialText.text = message;
            tutorialPanel.SetActive(true);

            // Hide tutorial after duration
            CancelInvoke("HideTutorial");
            Invoke("HideTutorial", duration);
        }
    }

    private void HideTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    public void ToggleMiniMap(bool show)
    {
        if (miniMap != null)
        {
            miniMap.SetActive(show);
        }
    }
}