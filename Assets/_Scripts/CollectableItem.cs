using UnityEngine;

public class CollectableItem : MonoBehaviour, IInteractable
{
    [SerializeField] private int scoreValue = 1;
    [SerializeField] private string itemId; // Optional unique identifier

    private void Awake()
    {
        // If no ID is set, use the object name
        if (string.IsNullOrEmpty(itemId))
        {
            itemId = gameObject.name;
        }
    }

    public void OnInteract()
    {
        // Check if GameManager exists
        if (GameManager.Instance != null)
        {
            // Increment score
            GameManager.Instance.AddScore(scoreValue);

            // Play sound effect if needed

            // Destroy the object
            Destroy(gameObject);
        }
    }

    // Add getter for item ID
    public string GetItemId()
    {
        return itemId;
    }
}