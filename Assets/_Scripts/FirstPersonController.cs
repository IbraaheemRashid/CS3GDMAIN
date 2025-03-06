using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float runSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float lookUpLimit = 90.0f;
    [SerializeField] private float lookDownLimit = -90.0f;

    [Header("Stealth Settings")]
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float crouchingHeight = 1.0f;

    // Internal variables
    private CharacterController controller;
    private Camera playerCamera;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private bool isCrouching = false;
    private float cameraPitch = 0.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Only process input if the game is not paused
        if (Time.timeScale > 0)
        {
            HandleMovement();
            HandleLook();
            HandleCrouch();
            HandleJump();
        }
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -0.5f; // Small negative value instead of 0 for grounding
        }

        // Get input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calculate move direction based on the player's facing
        Vector3 move = transform.right * x + transform.forward * z;

        // Determine speed
        float speed = walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            speed = runSpeed;
        }
        else if (isCrouching)
        {
            speed = crouchSpeed;
        }

        // Apply movement
        controller.Move(move * speed * Time.deltaTime);

        // Apply gravity
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void HandleLook()
    {
        // Skip camera movement if game is paused
        if (Time.timeScale <= 0)
            return;

        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Adjust pitch (up/down) rotation
        cameraPitch -= mouseY; // Subtract to invert Y axis
        cameraPitch = Mathf.Clamp(cameraPitch, lookDownLimit, lookUpLimit);

        // Apply rotations
        playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCrouch();
        }
    }

    // New method to toggle crouch state
    private void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        UpdateCrouchState();
    }

    // New method to update controller based on crouch state
    private void UpdateCrouchState()
    {
        if (isCrouching)
        {
            controller.height = crouchingHeight;
            controller.center = new Vector3(0, crouchingHeight / 2, 0);
            playerCamera.transform.localPosition = new Vector3(0, crouchingHeight - 0.2f, 0);
        }
        else
        {
            controller.height = standingHeight;
            controller.center = new Vector3(0, standingHeight / 2, 0);
            playerCamera.transform.localPosition = new Vector3(0, standingHeight - 0.2f, 0);
        }
    }

    // New method to set crouch state directly (used when loading game)
    public void SetCrouchState(bool crouched)
    {
        if (isCrouching != crouched)
        {
            isCrouching = crouched;
            UpdateCrouchState();
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    // For pausing/menu
    public void SetCursorState(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    // For checking stealth status
    public bool IsCrouching()
    {
        return isCrouching;
    }
}