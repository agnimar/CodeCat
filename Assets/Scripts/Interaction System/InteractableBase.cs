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
        set => itemName = value;
    }
    private bool interactionEnabled = true;

    private void Awake()
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogWarning($"ItemName is not set for {gameObject.name}. Defaulting to prefab name.");
            itemName = gameObject.name; 
        }
    }
    public virtual bool CanInteract(GameObject interactor)
    {
        if (!interactionEnabled) return false;

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

        var itemGameObject = gameObject; 
        if (inventory.AddItem(itemGameObject))
        {
            gameObject.SetActive(false); 
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

    }
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
    public void SetInteractionEnabled(bool isEnabled)
    {
        interactionEnabled = isEnabled;
    }
}