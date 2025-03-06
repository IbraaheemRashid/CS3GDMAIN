using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    // Singleton pattern
    public static SaveSystem Instance { get; private set; }

    [SerializeField] private string saveFileName = "savegame.xml";
    [SerializeField] private string saveFolderName = "Saves";

    private string SavePath => Path.Combine(Application.persistentDataPath, saveFolderName);
    private string SaveFilePath => Path.Combine(SavePath, saveFileName);

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Create save directory if it doesn't exist
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool SaveExists()
    {
        return File.Exists(SaveFilePath);
    }

    public void SaveGame()
    {
        GameStateData gameState = CreateGameStateData();

        try
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlSerializer serializer = new XmlSerializer(typeof(GameStateData));

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, gameState);
                stream.Position = 0;
                xmlDocument.Load(stream);
                xmlDocument.Save(SaveFilePath);
                Debug.Log("Game saved successfully to: " + SaveFilePath);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving game: " + e.Message);
        }
    }

    public bool LoadGame()
    {
        if (!SaveExists())
        {
            Debug.LogWarning("No save file found at: " + SaveFilePath);
            return false;
        }

        try
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(SaveFilePath);

            string xmlString = xmlDocument.OuterXml;

            XmlSerializer serializer = new XmlSerializer(typeof(GameStateData));

            using (StringReader stringReader = new StringReader(xmlString))
            {
                GameStateData gameState = (GameStateData)serializer.Deserialize(stringReader);

                // First load the correct scene if needed
                if (gameState.progressData.currentSceneName != SceneManager.GetActiveScene().name)
                {
                    // Store the game state to be applied after scene load
                    StartCoroutine(LoadSceneAndApplyState(gameState));
                }
                else
                {
                    // Apply the loaded state directly
                    ApplyGameStateData(gameState);
                }

                Debug.Log("Game loaded successfully from: " + SaveFilePath);
                return true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading game: " + e.Message);
            return false;
        }
    }

    System.Collections.IEnumerator LoadSceneAndApplyState(GameStateData gameState)
    {
        // Store the state temporarily
        GameStateData storedState = gameState;

        // Load the scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(gameState.progressData.currentSceneName);

        // Wait until the scene is fully loaded
        while (!operation.isDone)
        {
            yield return null;
        }

        // Wait one more frame to ensure all objects are initialized
        yield return null;

        // Apply the state
        ApplyGameStateData(storedState);
    }

    private GameStateData CreateGameStateData()
    {
        GameStateData gameState = new GameStateData();

        // Get player data
        FirstPersonController player = FindObjectOfType<FirstPersonController>();
        if (player != null)
        {
            gameState.playerData = new PlayerData(player.transform, player.IsCrouching());
        }

        // Get collectibles data
        CollectableItem[] collectibles = FindObjectsOfType<CollectableItem>();
        gameState.collectiblesData = new System.Collections.Generic.List<CollectibleData>();

        foreach (CollectableItem item in collectibles)
        {
            string itemId = item.gameObject.name;
            gameState.collectiblesData.Add(new CollectibleData(itemId, item.transform.position, false));
        }

        // Get game progress data
        if (GameManager.Instance != null)
        {
            gameState.progressData.score = GameManager.Instance.GetScore();
            gameState.progressData.collectedItems = GameManager.Instance.GetCollectedItems();
            gameState.progressData.totalItems = GameManager.Instance.GetTotalItems();
            gameState.progressData.currentSceneName = SceneManager.GetActiveScene().name;
            gameState.progressData.gameStartTime = GameManager.Instance.GetGameStartTime();
        }

        return gameState;
    }

    private void ApplyGameStateData(GameStateData gameState)
    {
        // Apply player data
        FirstPersonController player = FindObjectOfType<FirstPersonController>();
        if (player != null && gameState.playerData != null)
        {
            player.transform.position = gameState.playerData.position.ToVector3();
            player.transform.eulerAngles = gameState.playerData.rotation.ToVector3();

            // Manually set crouch state
            if (gameState.playerData.isCrouching != player.IsCrouching())
            {
                player.SetCrouchState(gameState.playerData.isCrouching);
            }
        }

        // Handle collectibles
        if (gameState.collectiblesData != null)
        {
            // Find all collectibles in the scene
            CollectableItem[] sceneCollectibles = FindObjectsOfType<CollectableItem>();

            foreach (CollectibleData savedItem in gameState.collectiblesData)
            {
                if (savedItem.isCollected)
                {
                    // Find and destroy collected items
                    foreach (CollectableItem sceneItem in sceneCollectibles)
                    {
                        if (sceneItem.gameObject.name == savedItem.id)
                        {
                            Destroy(sceneItem.gameObject);
                            break;
                        }
                    }
                }
            }
        }

        // Apply game progress data
        if (GameManager.Instance != null && gameState.progressData != null)
        {
            GameManager.Instance.LoadGameState(
                gameState.progressData.score,
                gameState.progressData.collectedItems,
                gameState.progressData.totalItems,
                gameState.progressData.gameStartTime
            );

            // Update UI if needed
            if (UIManager.Instance != null)
            {
                UIManager.Instance.SetTotalItems(gameState.progressData.totalItems);
                UIManager.Instance.UpdateObjective(gameState.progressData.currentObjective);
            }
        }
    }
}