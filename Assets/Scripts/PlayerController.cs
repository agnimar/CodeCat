using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement settings
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float gravity = -9.81f;

    // Components
    private CharacterController controller;

    // Input variables
    private float currentSpeed;
    private Vector3 movement;
    private Vector3 velocity;
    private bool isGrounded;

    // Ground check settings
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Camera Reference")]
    public Transform cameraTransform;
    public Animator animator;

    private bool isFirstPerson = false;

    private bool isMoving = false;
    private bool wasMoving = false;

    void Start()
    {
        // Get the character controller
        controller = GetComponent<CharacterController>();
        
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        HandleMovement();
        HandleAnimation();
    }

    public void SetFirstPerson(bool firstPerson)
    {
        isFirstPerson = firstPerson;
    }

    private void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        float rawMoveX = Input.GetAxisRaw("Horizontal"); 
        float rawMoveZ = Input.GetAxisRaw("Vertical");   

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 targetMovement = (forward * rawMoveZ + right * rawMoveX).normalized;

        movement = Vector3.Lerp(movement, targetMovement, Time.deltaTime * 10f);

        if (!isFirstPerson && movement.sqrMagnitude > 0.01f)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 10f);
        }

        currentSpeed = (movement.magnitude > 0) ?
                       (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed) : 0;

        controller.Move(movement * currentSpeed * Time.deltaTime);
        animator.SetFloat("Speed", currentSpeed);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    private void HandleAnimation()
    {
        isMoving = movement.magnitude > 0.01f;

        if (isMoving && !wasMoving)
        {
            animator.SetTrigger("StartMoving");
        }
        else if (!isMoving && wasMoving)
        {
            animator.SetTrigger("StopMoving");
        }

        // OPTIONAL: Separate trigger for "Standing"
        // Typically you can just rely on "StopMoving" → Idle transition,
        // but if you want a distinct trigger for your standing state:
        if (isGrounded && !isMoving)
        {
            animator.SetTrigger("Standing");
        }

        // Keep track of state for next frame
        wasMoving = isMoving;
    }
}
