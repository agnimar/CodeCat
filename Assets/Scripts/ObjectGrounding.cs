using UnityEngine;

public class ObjectGrounding : MonoBehaviour
{
    [Header("Grounding Settings")]
    [Tooltip("Layer mask that represents the ground.")]
    public LayerMask groundLayer;

    [Tooltip("Vertical offset from the hit point (e.g., if the tree's pivot isn't at its base).")]
    public float groundOffset = 0f;

    [Tooltip("Maximum distance to check for the ground below the tree.")]
    public float maxGroundCheckDistance = 100f;

    private void OnEnable()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, maxGroundCheckDistance, groundLayer))
        {
            Vector3 newPosition = transform.position;
            newPosition.y = hit.point.y + groundOffset;
            transform.position = newPosition;
        }
    }
}
