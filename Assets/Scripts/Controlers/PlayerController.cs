using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float gravityForce = -9.81f;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.4f;
    [SerializeField] private LayerMask groundLayer;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Animator animator;

    // Components
    private CharacterController characterController;

    // Movement state
    private float currentMoveSpeed;
    private Vector3 smoothedMovement;
    private Vector3 gravityVelocity;
    private bool isGrounded;

    // Animation state
    private bool isMoving;
    private bool wasMoving;
    private bool isSprinting;

    // First-person mode flag
    private bool isFirstPersonMode = false;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        currentMoveSpeed = walkSpeed;
    }

    private void Update()
    {
        if (!IsAnyUIOpen())
        {
            ProcessMovement();
        }
        UpdateAnimations();
    }
    private bool IsAnyUIOpen()
    {
        bool inventoryOpen = UIManager.Instance != null && UIManager.Instance.IsInventoryOpen;
        bool bookUIOpen = BookUIManager.Instance != null && BookUIManager.Instance.IsBookUIOpen;
        return inventoryOpen || bookUIOpen;
    }
    public void SetFirstPersonMode(bool firstPerson)
    {
        isFirstPersonMode = firstPerson;
    }

    private void ProcessMovement()
    {
        UpdateGroundStatus();
        if (TutorialManager.Instance != null && !TutorialManager.Instance.IsMovementAllowed)
            return;
        Vector3 targetDirection = GetInputDirection();
        UpdateSmoothedMovement(targetDirection);
        RotatePlayer(smoothedMovement);
        UpdateMovementSpeed();
        MoveCharacter(smoothedMovement);
        ApplyGravity();
    }

    private void UpdateGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = -2f;
        }
    }

    private Vector3 GetInputDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        return (forward * vertical + right * horizontal).normalized;
    }

    private void UpdateSmoothedMovement(Vector3 targetDirection)
    {
        smoothedMovement = Vector3.Lerp(smoothedMovement, targetDirection, Time.deltaTime * 10f);
    }

    private void RotatePlayer(Vector3 direction)
    {
        if (!isFirstPersonMode && direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private void UpdateMovementSpeed()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift) && smoothedMovement.sqrMagnitude > 0.01f;
        currentMoveSpeed = smoothedMovement.magnitude > 0 ? (isSprinting ? sprintSpeed : walkSpeed) : 0f;
        animator.SetFloat("Speed", currentMoveSpeed/1.5f);
    }

    private void MoveCharacter(Vector3 direction)
    {
        characterController.Move(direction * currentMoveSpeed * Time.deltaTime);
        
    }

    private void ApplyGravity()
    {
        gravityVelocity.y += gravityForce * Time.deltaTime;
        characterController.Move(gravityVelocity * Time.deltaTime);
    }
    private void UpdateAnimations()
    {
        if (IsAnyUIOpen())
        {
            if (wasMoving)
            {
                animator.SetTrigger("StopMoving");
            }
            animator.SetTrigger("Standing");
            wasMoving = false;
            isMoving = false;
            return;
        }

        isMoving = smoothedMovement.sqrMagnitude > 0.1f;
        if (isMoving && !wasMoving)
        {
            animator.SetTrigger("StartMoving");
            PlayerEvents.Moved();
        }
        else if (!isMoving && wasMoving)
        {
            animator.SetTrigger("StopMoving");
        }

        if (isGrounded && !isMoving)
        {
            animator.SetTrigger("Standing");
        }

        wasMoving = isMoving;
    }

}
