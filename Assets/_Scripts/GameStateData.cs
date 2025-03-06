using System;
using System.Collections.Generic;
using UnityEngine;

// This class will contain all the data we want to save
[Serializable]
public class GameStateData
{
    // Player data
    public PlayerData playerData;

    // Collectibles data
    public List<CollectibleData> collectiblesData;

    // Game progress data
    public GameProgressData progressData;

    public GameStateData()
    {
        playerData = new PlayerData();
        collectiblesData = new List<CollectibleData>();
        progressData = new GameProgressData();
    }
}

// Player position, rotation, etc.
[Serializable]
public class PlayerData
{
    public Vector3Data position;
    public Vector3Data rotation;
    public bool isCrouching;

    public PlayerData()
    {
        position = new Vector3Data();
        rotation = new Vector3Data();
        isCrouching = false;
    }

    // Create from a player GameObject
    public PlayerData(Transform playerTransform, bool crouchState)
    {
        position = new Vector3Data(playerTransform.position);
        rotation = new Vector3Data(playerTransform.eulerAngles);
        isCrouching = crouchState;
    }
}

// Information about each collectible (collected or not, position)
[Serializable]
public class CollectibleData
{
    public string id;
    public Vector3Data position;
    public bool isCollected;

    public CollectibleData()
    {
        id = "";
        position = new Vector3Data();
        isCollected = false;
    }

    public CollectibleData(string itemId, Vector3 itemPosition, bool collected)
    {
        id = itemId;
        position = new Vector3Data(itemPosition);
        isCollected = collected;
    }
}

// Overall game progress
[Serializable]
public class GameProgressData
{
    public int score;
    public int collectedItems;
    public int totalItems;
    public string currentObjective;
    public float gameStartTime;
    public string currentSceneName;

    public GameProgressData()
    {
        score = 0;
        collectedItems = 0;
        totalItems = 0;
        currentObjective = "";
        gameStartTime = 0f;
        currentSceneName = "";
    }
}

// Helper class to serialize Vector3 (since Unity's Vector3 isn't serializable by default)
[Serializable]
public class Vector3Data
{
    public float x;
    public float y;
    public float z;

    public Vector3Data()
    {
        x = 0;
        y = 0;
        z = 0;
    }

    public Vector3Data(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}