using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform player;
    public Vector3 thirdPersonOffset = new Vector3(0, 4, -5);
    public Vector3 firstPersonOffset = new Vector3(0, 1.2f, 0);
    public float mouseSensitivity = 100f;

    private bool isFirstPerson = false;
    private float xRotation = 0f; // Vertical rotation
    private float yRotation = 0f; // Horizontal rotation

    [Header("Rotation Constraints")]
    public float minVerticalAngle = -30f; // Minimum angle for looking down
    public float maxVerticalAngle = 30f;  // Maximum angle for looking up

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
                playerController.SetFirstPerson(isFirstPerson);
            }

            if (isFirstPerson)
            {
                transform.position = player.TransformPoint(firstPersonOffset);
                xRotation = transform.eulerAngles.x;
            }
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
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        }
        else
        {
            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
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
