using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private int scorePerItem = 100;
    [SerializeField] private int timeBonus = 500;
    [SerializeField] private float maxTimeForBonus = 180f; // 3 minutes

    // Game state tracking
    private int score = 0;
    private float gameStartTime;
    private bool gameIsActive = false;
    private int totalItems = 0;
    private int collectedItems = 0;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Only initialize if we're in a game scene (not in the menu)
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            InitializeGame();
        }
    }

    // Called when scene is loaded
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Initialize the game if we loaded a game scene
        if (scene.name != "MainMenu")
        {
            InitializeGame();
        }
    }

    private void InitializeGame()
    {
        // Count all collectible items in the scene
        CollectableItem[] items = FindObjectsOfType<CollectableItem>();
        totalItems = items.Length;

        // Update UI if UIManager exists
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetTotalItems(totalItems);
            UIManager.Instance.UpdateObjective("Collect all valuable items and escape!");
        }

        // Reset game state
        score = 0;
        collectedItems = 0;
        gameStartTime = Time.time;
        gameIsActive = true;
    }

    public void AddScore(int points)
    {
        score += points * scorePerItem;
        collectedItems++;

        // Update UI if UIManager exists
        if (UIManager.Instance != null)
        {
            UIManager.Instance.AddCollectedItem();

            // Check if all items collected
            if (collectedItems >= totalItems)
            {
                UIManager.Instance.UpdateObjective("All items collected! Find the exit.");
            }
        }
    }

    public void CompleteLevel()
    {
        if (!gameIsActive) return;

        gameIsActive = false;

        // Calculate time bonus
        float timeTaken = Time.time - gameStartTime;
        int bonus = 0;

        if (timeTaken < maxTimeForBonus)
        {
            float bonusMultiplier = 1 - (timeTaken / maxTimeForBonus);
            bonus = Mathf.RoundToInt(timeBonus * bonusMultiplier);
            score += bonus;
        }

        // Save high score if better
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }

        // Show mission complete screen
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowMissionComplete(score);
        }
    }

    public void GameOver()
    {
        gameIsActive = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver();
        }
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        // Load next level
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // No more levels, return to menu
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // Save and Load methods
    public void SaveGame()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGame();
        }
        else
        {
            Debug.LogError("SaveSystem instance not found!");
        }
    }

    public void LoadGame()
    {
        if (SaveSystem.Instance != null)
        {
            bool success = SaveSystem.Instance.LoadGame();
            if (!success)
            {
                Debug.LogWarning("Failed to load game or no save file exists");
            }
        }
        else
        {
            Debug.LogError("SaveSystem instance not found!");
        }
    }

    // Methods to get game state data
    public int GetScore()
    {
        return score;
    }

    public int GetCollectedItems()
    {
        return collectedItems;
    }

    public int GetTotalItems()
    {
        return totalItems;
    }

    public float GetGameStartTime()
    {
        return gameStartTime;
    }

    // Method to load game state
    public void LoadGameState(int newScore, int newCollectedItems, int newTotalItems, float newGameStartTime)
    {
        score = newScore;
        collectedItems = newCollectedItems;
        totalItems = newTotalItems;
        gameStartTime = newGameStartTime;
        gameIsActive = true;
    }
}