using UnityEngine;

public class InteractableBook : InteractableBase
{
    [Header("Interaction Range Settings")]
    [SerializeField] private GameObject rangeGameObject;

    private void Awake()
    {
        ItemName = "Book";

        if (rangeGameObject != null)
        {
            Collider col = rangeGameObject.GetComponent<Collider>();
            if (col == null)
            {
                SphereCollider sphere = rangeGameObject.AddComponent<SphereCollider>();
                sphere.isTrigger = true;
                sphere.radius = 7f; 
            }
            else
            {
                col.isTrigger = true;
            }
            if (rangeGameObject.GetComponent<BookTrigger>() == null)
            {
                rangeGameObject.AddComponent<BookTrigger>();
            }
        }
        else
        {
            Debug.LogWarning("Range GameObject not assigned on InteractableBook. The book interaction area will not be detected.");
        }
    }

    private void OnEnable()
    {
        BookTrigger.OnEnteredBookArea += HandleRangeEntered;
    }

    private void OnDisable()
    {
        BookTrigger.OnEnteredBookArea -= HandleRangeEntered;
    }

    private void HandleRangeEntered()
    {
        if (rangeGameObject != null)
        {
            rangeGameObject.SetActive(false);
        }
    }

    public override void OnInteractionStart(InteractionData data)
    {
        onInteractionStarted?.Invoke(data);

        if (BookUIManager.Instance != null)
        {
            BookUIManager.Instance.UnlockBookUI();
            BookUIManager.Instance.ToggleBookUI();
        }
        else
        {
            Debug.LogWarning("BookUIManager instance not found in the scene!");
        }

        PlayerEvents.BookInteracted();

        gameObject.SetActive(false);
    }
}
