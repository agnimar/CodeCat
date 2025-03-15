using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ObjectGrounding : MonoBehaviour
{
    [Header("Grounding Settings")]
    [Tooltip("Layer mask that represents the ground.")]
    public LayerMask groundLayer;

    [Tooltip("Vertical offset from the hit point (e.g., if the object's pivot isn't at its base).")]
    public float groundOffset = 0f;

    [Tooltip("Maximum distance to check for the ground below the object.")]
    public float maxGroundCheckDistance = 100f;

    private void OnEnable()
    {
        GroundObject();
    }

    private void GroundObject()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, maxGroundCheckDistance, groundLayer))
        {
#if UNITY_EDITOR
            Undo.RecordObject(transform, "Ground Object");
#endif
            Vector3 newPosition = transform.position;
            newPosition.y = hit.point.y + groundOffset;
            transform.position = newPosition;
#if UNITY_EDITOR
            EditorUtility.SetDirty(transform);
#endif
        }

#if UNITY_EDITOR
        DestroyImmediate(this);
#else
        Destroy(this);
#endif
    }
}
