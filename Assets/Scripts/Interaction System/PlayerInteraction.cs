using UnityEngine;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float interactionCheckRadius = 0.5f;
    [SerializeField] private float maxInteractionDistance = 2f;
    [SerializeField] private LayerMask interactableLayers;
    [SerializeField] private Transform interactionSource;

    [Header("Input Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode switchTargetKey = KeyCode.Tab;
    [SerializeField] private float holdThreshold = 0.5f;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;

    private List<IInteractable> nearbyInteractables = new List<IInteractable>();
    private int currentInteractableIndex = -1;
    private IInteractable currentTarget;
    private float holdTime;
    private bool isInteracting;
    private InventoryManager inventoryManager;

    private void Start()
    {
        if (interactionSource == null)
            interactionSource = Camera.main.transform;
        // Attempt to find InventoryManager on the same GameObject

        inventoryManager = GetComponent<InventoryManager>();

        if (inventoryManager == null)
        {
            Debug.LogError("No InventoryManager found on this GameObject. " +
                           "Please attach an InventoryManager script to the player.");
        }
    }

    private void Update()
    {
        UpdateInteractionTarget();
        HandleInteractionInput();
    }

    private void UpdateInteractionTarget()
    {
        nearbyInteractables.Clear();

        Ray ray = new Ray(interactionSource.position, interactionSource.forward);
        var hits = Physics.RaycastAll(ray, maxInteractionDistance, interactableLayers);

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable)
                && interactable.CanInteract(gameObject))
            {
                nearbyInteractables.Add(interactable);
            }
        }

        if (nearbyInteractables.Count == 0)
        {
            var nearbyColliders = Physics.OverlapSphere(
                interactionSource.position + interactionSource.forward * interactionCheckRadius,
                interactionCheckRadius,
                interactableLayers
            );

            foreach (var col in nearbyColliders)
            {
                if (col.TryGetComponent<IInteractable>(out var interactable)
                    && interactable.CanInteract(gameObject))
                {
                    nearbyInteractables.Add(interactable);
                }
            }
        }

        HandleTargetSelection();
    }

    private void HandleTargetSelection()
    {
        if (nearbyInteractables.Count == 0)
        {
            currentInteractableIndex = -1;
            currentTarget = null;
            return;
        }

        if (Input.GetKeyDown(switchTargetKey))
        {
            currentInteractableIndex = (currentInteractableIndex + 1) % nearbyInteractables.Count;
        }
        else if (currentInteractableIndex == -1)
        {
            currentInteractableIndex = 0;
        }

        currentTarget = nearbyInteractables[currentInteractableIndex];
    }

    private void HandleInteractionInput()
    {
        if (currentTarget == null) return;

        if (Input.GetKeyDown(interactKey))
        {
            var pillar = currentTarget as InteractablePillar;

            if (pillar != null)
            {
                pillar.OnInteractionStart(CreateInteractionData());
            }
            else
            {
                StartInteraction();
            }
        }
        else if (Input.GetKey(interactKey))
        {
            UpdateInteraction();
        }
        else if (Input.GetKeyUp(interactKey))
        {
            EndInteraction();
        }
    }


    private InteractionData CreateInteractionData()
    {
        return new InteractionData(
            gameObject,
            currentTarget.GetType().IsAssignableFrom(typeof(Component)) ?
                ((Component)currentTarget).gameObject : null,
            interactionSource.position + interactionSource.forward * interactionCheckRadius
        );
    }

    private void StartInteraction()
    {
        if (currentTarget == null) return;

        isInteracting = true;
        holdTime = 0f;
        currentTarget.OnInteractionStart(CreateInteractionData());
    }

    private void UpdateInteraction()
    {
        if (!isInteracting || currentTarget == null) return;

        holdTime += Time.deltaTime;

        if (holdTime >= holdThreshold)
        {
            currentTarget.OnInteractionUpdate(CreateInteractionData());
        }
    }

    private void EndInteraction()
    {
        if (!isInteracting || currentTarget == null) return;

        currentTarget.OnInteractionEnd(CreateInteractionData());
        isInteracting = false;
        holdTime = 0f;
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        Transform source = interactionSource != null ? interactionSource : transform;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            source.position + source.forward * interactionCheckRadius,
            interactionCheckRadius
        );

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(source.position, source.forward * maxInteractionDistance);

        if (nearbyInteractables != null)
        {
            foreach (var interactable in nearbyInteractables)
            {
                if (interactable == null || ((Component)interactable).gameObject == null)
                continue;
                Gizmos.color = (interactable == currentTarget) ? Color.green : Color.gray;
                Vector3 targetPos = ((Component)interactable).transform.position;
                Gizmos.DrawLine(source.position, targetPos);
            }
        }
    }
}