using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Target to follow (typically the player)
    public Transform player;

    // Camera offset for third-person view
    public Vector3 thirdPersonOffset = new Vector3(0, 4, -5);

    // First-person camera position relative to the player's head
    public Vector3 firstPersonOffset = new Vector3(0, 1.2f, 0);

    // Mouse sensitivity
    public float mouseSensitivity = 100f;

    // Internal state
    private bool isFirstPerson = false;
    private float xRotation = 0f;

    void Start()
    {
        // Lock cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleViewSwitch();
        HandleMouseLook();
    }

    void LateUpdate()
    {
        if (isFirstPerson)
        {
            // Attach the camera to the player's "eyes"
            transform.position = player.TransformPoint(firstPersonOffset);
            transform.rotation = Quaternion.Euler(xRotation, player.eulerAngles.y, 0f);
        }
        else
        {
            // Position the camera behind the player
            Vector3 desiredPosition = player.position + player.TransformDirection(thirdPersonOffset);
            transform.position = desiredPosition;

            // Look at the player
            transform.LookAt(player.position + Vector3.up * 1.2f);
        }
    }

    private void HandleViewSwitch()
    {
        // Toggle between first-person and third-person views
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;

            if (isFirstPerson)
            {
                // Snap camera to player's eyes
                transform.position = player.TransformPoint(firstPersonOffset);
                xRotation = transform.eulerAngles.x;
            }
        }
    }

    private void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (isFirstPerson)
        {
            // Rotate player horizontally with mouse X
            player.Rotate(Vector3.up * mouseX);

            // Rotate camera vertically with mouse Y
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limit vertical rotation
        }
        else
        {
            // Rotate third-person camera around the player
            transform.RotateAround(player.position, Vector3.up, mouseX);
        }
    }
}
