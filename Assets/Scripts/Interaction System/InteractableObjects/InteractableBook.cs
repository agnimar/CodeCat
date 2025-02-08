using UnityEngine;

public class InteractableBook : InteractableBase
{
    private void Awake()
    {
        ItemName = "Book";
    }

    public override void OnInteractionStart(InteractionData data)
    {
        onInteractionStarted?.Invoke(data);

        // Unlock the Book UI.
        if (BookUIManager.Instance != null)
        {
            BookUIManager.Instance.UnlockBookUI();
        }
        else
        {
            Debug.LogWarning("BookUIManager instance not found in the scene!");
        }

        PlayerEvents.BookInteracted();

        InventoryEvents.BookCollected();

        gameObject.SetActive(false);
    }
}
