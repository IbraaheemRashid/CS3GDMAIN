using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject interactiveCrosshair;

    private Camera playerCamera;

    void Start()
    {
        playerCamera = GetComponent<Camera>();

        // Ensure default crosshair is showing
        if (crosshair != null) crosshair.SetActive(true);
        if (interactiveCrosshair != null) interactiveCrosshair.SetActive(false);
    }

    void Update()
    {
        HandleRaycast();
        HandleInteraction();
    }

    void HandleRaycast()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Cast ray to detect interactive objects
        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            // Detected an interactive object
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                // Show interactive crosshair
                if (crosshair != null) crosshair.SetActive(false);
                if (interactiveCrosshair != null) interactiveCrosshair.SetActive(true);
            }
            else
            {
                // Show default crosshair
                if (crosshair != null) crosshair.SetActive(true);
                if (interactiveCrosshair != null) interactiveCrosshair.SetActive(false);
            }
        }
        else
        {
            // Show default crosshair
            if (crosshair != null) crosshair.SetActive(true);
            if (interactiveCrosshair != null) interactiveCrosshair.SetActive(false);
        }
    }

    void HandleInteraction()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.OnInteract();
                }
            }
        }
    }
}