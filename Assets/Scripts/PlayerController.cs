using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement settings
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float gravity = -9.81f; // Gravity force

    // Components
    private CharacterController controller;

    // Input variables
    private float currentSpeed;
    private Vector3 movement;
    private Vector3 velocity;
    private bool isGrounded;

    // Ground check settings
    public Transform groundCheck; // Empty GameObject to check ground collision
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Camera Reference")]
    public Transform cameraTransform; // Reference to the camera transform

    private bool isFirstPerson = false; // First-person mode flag

    void Start()
    {
        // Get the character controller
        controller = GetComponent<CharacterController>();
        currentSpeed = walkSpeed; // Set default speed to walk
    }

    void Update()
    {
        HandleMovement();
    }

    public void SetFirstPerson(bool firstPerson)
    {
        isFirstPerson = firstPerson; // Update the first-person mode state
    }

    private void HandleMovement()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to ensure consistent ground collision
        }

        // Get input
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right arrow keys
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down arrow keys

        // Movement direction relative to the camera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Ignore vertical movement (Y-axis)
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        if (isFirstPerson)
        {
            // In first-person mode, movement is based on camera direction without rotating the player
            movement = (forward * moveZ + right * moveX).normalized;
        }
        else
        {
            // In third-person mode, movement is still relative to the camera but player rotates
            movement = (forward * moveZ + right * moveX).normalized;

            // Rotate the player to face the movement direction
            if (movement.magnitude > 0)
            {
                Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 10f);
            }
        }

        // Apply movement with speed
        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        controller.Move(movement * currentSpeed * Time.deltaTime);

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
