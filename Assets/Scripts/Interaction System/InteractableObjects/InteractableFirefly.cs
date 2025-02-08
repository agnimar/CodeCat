using UnityEngine;

public class InteractableFirefly : InteractableBase
{
    [Header("Floating Settings")]
    [Tooltip("Vertical movement amplitude (in world units)")]
    public float floatAmplitude = 0.5f;
    [Tooltip("Speed of the bobbing motion")]
    public float floatFrequency = 1f;

    private float baseY;
    private float phaseOffset;
    private bool useLocal = false;

    private void Awake()
    {
        ItemName = "Firefly";
    }

    private void OnEnable()
    {
        InitializeFloating();
    }

    private void InitializeFloating()
    {
        phaseOffset = Random.Range(0f, 2 * Mathf.PI);

        if (transform.parent != null)
        {
            baseY = transform.localPosition.y;
            useLocal = true;
        }
        else
        {
            baseY = transform.position.y;
            useLocal = false;
        }
    }

    private void OnTransformParentChanged()
    {
        if (transform.GetComponentInParent<InteractablePillar>() == null)
        {
            InitializeFloating();
        }
    }

    private void Update()
    {
        if (transform.GetComponentInParent<InteractablePillar>() != null)
        {
            return;
        }

        float newY = baseY + Mathf.Sin(Time.time * floatFrequency + phaseOffset) * floatAmplitude;

        if (useLocal)
        {
            Vector3 localPos = transform.localPosition;
            localPos.y = newY;
            transform.localPosition = localPos;
        }
        else
        {
            Vector3 pos = transform.position;
            pos.y = newY;
            transform.position = pos;
        }
    }
}
