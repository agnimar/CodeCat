using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    [Tooltip("Pivot point for the camera, placed at head level of the player.")]
    public Transform cameraPivot;

    [Header("Player Rendering")]
    [Tooltip("The layer name assigned to the player game object that should be excluded from the camera when in first-person.")]
    public string playerLayerName = "Player";

    [Header("Camera Offsets")]
    public Vector3 thirdPersonOffset = new Vector3(0, 2, -4);
    public Vector3 firstPersonOffset = new Vector3(0, 0, 0);
    public Vector3 sprintFirstPersonOffset = new Vector3(0, 0.45f, 0.2f);

    [Header("Sensitivity")]
    public float mouseSensitivity = 100f;

    [Header("Smoothing")]
    public float positionLerpSpeed = 10f;
    public float rotationLerpSpeed = 10f;

    [Header("First Person Rotation Constraints")]
    [SerializeField] public float fpMinVerticalAngle = -85f;
    [SerializeField] public float fpMaxVerticalAngle = 85f;

    [Header("Third Person Rotation Constraints")]
    public float tpMinVerticalAngle = -30f;
    public float tpMaxVerticalAngle = 30f;

    private bool isFirstPerson = false;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private bool hasLookedAround = false;
    private PlayerController playerController;

    private Camera cam;
    private int playerLayer;

    void Start()
    {
        UIManager.Instance.SetCursorLockState(true);
        yRotation = player.eulerAngles.y;
        playerController = player.GetComponent<PlayerController>();
        cam = GetComponent<Camera>();
        playerLayer = LayerMask.NameToLayer(playerLayerName);
    }

    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            HandleViewSwitch();
            HandleMouseLook();
        }
    }

    void LateUpdate()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            SmoothUpdateCameraPosition();
        }
    }

    private void HandleViewSwitch()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;

            if (playerController != null)
            {
                playerController.SetFirstPersonMode(isFirstPerson);
            }
            if (cam != null)
            {
                if (isFirstPerson)
                {
                    cam.cullingMask &= ~(1 << playerLayer);
                }
                else
                {
                    cam.cullingMask |= (1 << playerLayer);
                }
            }

            xRotation = isFirstPerson ? 0f : 18f;
            yRotation = player.eulerAngles.y;
        }
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (isFirstPerson)
        {
            player.Rotate(Vector3.up * mouseX);
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, fpMinVerticalAngle, fpMaxVerticalAngle);
        }
        else
        {
            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, tpMinVerticalAngle, tpMaxVerticalAngle);
        }
        if (!hasLookedAround && (Mathf.Abs(mouseX) > 0.01f || Mathf.Abs(mouseY) > 0.01f))
        {
            hasLookedAround = true;
        }
    }

    private void SmoothUpdateCameraPosition()
    {
        if (isFirstPerson)
        {
            Vector3 offset = (playerController != null && playerController.isSprinting) ? sprintFirstPersonOffset : firstPersonOffset;
            targetPosition = cameraPivot.position + cameraPivot.TransformDirection(offset);
            targetRotation = Quaternion.Euler(xRotation, player.eulerAngles.y, 0f);
        }
        else
        {
            Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0);
            targetPosition = player.position + rotation * thirdPersonOffset;
            targetRotation = Quaternion.LookRotation((player.position + Vector3.up * 1.2f) - targetPosition);
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * positionLerpSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationLerpSpeed);
    }

    public void SetMouseSensitivity(float value)
    {
        mouseSensitivity = value;
    }
    public void SetFirstPersonExternally(bool isFirstPersonView)
    {
        isFirstPerson = isFirstPersonView;

        if (playerController != null)
        {
            playerController.SetFirstPersonMode(isFirstPerson);
        }

        if (cam != null)
        {
            if (isFirstPerson)
            {
                cam.cullingMask &= ~(1 << playerLayer);
            }
            else
            {
                cam.cullingMask |= (1 << playerLayer);
            }
        }

        xRotation = isFirstPerson ? 0f : 18f;
        yRotation = player.eulerAngles.y;
    }

}
