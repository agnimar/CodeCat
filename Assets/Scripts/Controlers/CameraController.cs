using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    [Tooltip("GameObject representing the nose or cockpit nose. Will be disabled in first-person view.")]
    public GameObject noseObject;

    [Header("Camera Offsets")]
    public Vector3 thirdPersonOffset = new Vector3(0, 0, 0);
    public Vector3 firstPersonOffset = new Vector3(0, 0.45f, 0);

    [Header("Sensitivity")]
    public float mouseSensitivity = 100f;

    [Header("First Person Rotation Constraints")]
    public float fpMinVerticalAngle = -85f; // Change as needed
    public float fpMaxVerticalAngle = 85f;  // Change as needed

    [Header("Third Person Rotation Constraints")]
    public float tpMinVerticalAngle = -30f; // Minimum angle for looking down
    public float tpMaxVerticalAngle = 30f;  // Maximum angle for looking up

    private bool isFirstPerson = false;
    private float xRotation = 0f; // Vertical rotation
    private float yRotation = 0f; // Horizontal rotation

    private bool hasLookedAround = false;

    void Start()
    {
        UIManager.Instance.SetCursorLockState(true); 
        yRotation = player.eulerAngles.y; 
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
            UpdateCameraPosition();
        }
    }

    private void HandleViewSwitch()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;

            var playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SetFirstPersonMode(isFirstPerson);
            }

            if (isFirstPerson)
            {
                transform.position = player.TransformPoint(firstPersonOffset);

                // Reset the vertical rotation so the camera is not looking down/up unexpectedly.
                xRotation = 0f;
                yRotation = player.eulerAngles.y;
                if (noseObject != null)
                {
                    noseObject.SetActive(false);
                }
            }
            else
            {
                // For third-person, just update yRotation (the inspector-set thirdPersonOffset will now remain unchanged)
                xRotation = 18f;
                yRotation = player.eulerAngles.y;
                if (noseObject != null)
                {
                    noseObject.SetActive(true);
                }
            }
            PlayerEvents.CameraSwitched();
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
            PlayerEvents.LookedAround();
        }
    }


    private void UpdateCameraPosition()
    {
        if (isFirstPerson)
        {
            transform.position = player.TransformPoint(firstPersonOffset);
            transform.rotation = Quaternion.Euler(xRotation, player.eulerAngles.y, 0f);
        }
        else
        {
            Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0);
            Vector3 desiredPosition = player.position + rotation * thirdPersonOffset;

            transform.position = desiredPosition;
            transform.LookAt(player.position + Vector3.up * 1.2f);
        }
    }
}
