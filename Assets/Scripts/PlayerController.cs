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

    private void HandleMovement()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to ensure consistent ground collision
        }

        // Get input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Calculate movement direction
        movement = transform.right * moveX + transform.forward * moveZ;

        // Sprinting (Shift key)
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        // Apply movement
        controller.Move(movement * currentSpeed * Time.deltaTime);

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
