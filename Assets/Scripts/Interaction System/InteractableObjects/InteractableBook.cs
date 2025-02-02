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

        //var itemGameObject = gameObject;
        if (BookUIManager.Instance != null)
        {
            BookUIManager.Instance.UnlockBookUI();
            // (Optional) Set the book content if desired:
            // BookUIManager.Instance.SetBookContent("Once upon a time, ...");
        }
        else
        {
            Debug.LogWarning("BookUIManager instance not found in the scene!");
        }
        gameObject.SetActive(false);

    }
}