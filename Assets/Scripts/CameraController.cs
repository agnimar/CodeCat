using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform player;
    public Vector3 thirdPersonOffset = new Vector3(0, 4, -5);
    public Vector3 firstPersonOffset = new Vector3(0, 1.2f, 0);
    public float mouseSensitivity = 100f;

    private bool isFirstPerson = false;
    private float xRotation = 0f;

    void Start()
    {
        UIManager.Instance.SetCursorLockState(true); // Lock the cursor when the game starts
    }

    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked) // Only handle camera if cursor is locked
        {
            HandleViewSwitch();
            HandleMouseLook();
        }
    }

    void LateUpdate()
    {
        if (Cursor.lockState == CursorLockMode.Locked) // Only update the camera if cursor is locked
        {
            UpdateCameraPosition();
        }
    }

    private void HandleViewSwitch()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;

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
            transform.RotateAround(player.position, Vector3.up, mouseX);
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
            Vector3 desiredPosition = player.position + player.TransformDirection(thirdPersonOffset);
            transform.position = desiredPosition;
            transform.LookAt(player.position + Vector3.up * 1.2f);
        }
    }
}
