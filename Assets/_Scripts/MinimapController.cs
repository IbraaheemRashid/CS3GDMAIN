using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float height = 20f;

    // Camera settings
    [SerializeField] private bool rotateWithPlayer = false;

    private Camera minimapCamera;

    void Start()
    {
        minimapCamera = GetComponent<Camera>();

        if (player == null)
        {
            // Try to find player if not assigned
            FirstPersonController playerController = FindObjectOfType<FirstPersonController>();
            if (playerController != null)
            {
                player = playerController.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            // Update position to follow player
            Vector3 newPosition = player.position;
            newPosition.y = player.position.y + height;
            transform.position = newPosition;

            // Update rotation if needed
            if (rotateWithPlayer)
            {
                transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            }
        }
    }

    // Toggle rotation with M key (optional)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            rotateWithPlayer = !rotateWithPlayer;
        }
    }
}