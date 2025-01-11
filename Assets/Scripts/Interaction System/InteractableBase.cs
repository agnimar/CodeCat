using UnityEngine;
using UnityEngine.Events;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [SerializeField] protected bool requiresLineOfSight = true;
    [SerializeField] protected bool allowContinuousInteraction = false;

    [Header("Events")]
    public UnityEvent<InteractionData> onInteractionStarted;
    public UnityEvent<InteractionData> onInteractionEnded;

    [Header("Item Settings")]
    [SerializeField] protected string itemName;
    public string ItemName
    {
        get => itemName;
        set => itemName = value; // Allow assignment
    }

    private void Awake()
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogWarning($"ItemName is not set for {gameObject.name}. Defaulting to prefab name.");
            itemName = gameObject.name; // Default to the prefab name if ItemName is not set
        }
    }
    public virtual bool CanInteract(GameObject interactor)
    {
        if (interactor == null) return false;

        if (requiresLineOfSight)
        {
            Vector3 direction = (transform.position - interactor.transform.position).normalized;
            if (Physics.Raycast(interactor.transform.position, direction, out RaycastHit hit))
            {
                return hit.collider.gameObject == gameObject;
            }
            return false;
        }

        return true;
    }

    public virtual void OnInteractionStart(InteractionData data)
    {
        onInteractionStarted?.Invoke(data);

        var inventory = data.Interactor.GetComponent<InventoryManager>();
        if (inventory == null) return;

        var itemGameObject = gameObject; // This is the actual in-scene object
        if (inventory.AddItem(itemGameObject))
        {
            Debug.Log($"{ItemName} collected!");
            // Do NOT destroy it; the item is now inactive and stored in the inventory.
        }
        else
        {
            Debug.Log($"{ItemName} is not added. Either it's already in the inventory or inventory is full.");
        }

    }

    public virtual void OnInteractionEnd(InteractionData data)
    {
        onInteractionEnded?.Invoke(data);
    }

    public virtual void OnInteractionUpdate(InteractionData data)
    {
        // Override in child classes if needed
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

}