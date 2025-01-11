using UnityEngine;

public class InteractablePillar : InteractableBase
{
    [SerializeField] private string requiredItem;
    private bool isOccupied = false;

    public bool IsOccupied => isOccupied; // Add this public property for access

    public override void OnInteractionStart(InteractionData data)
    {
        if (isOccupied) return;

        var inventory = data.Interactor.GetComponent<InventoryManager>();
        if (inventory == null) return;

        GameObject itemObject = inventory.GetAndRemoveItem(requiredItem);
        if (itemObject != null)
        {
            isOccupied = true;
            itemObject.SetActive(true);

            itemObject.transform.position = transform.position + Vector3.up * 1.0f; // adjust as needed
            itemObject.transform.SetParent(transform);

            Debug.Log($"{requiredItem} placed on pillar!");
            onInteractionStarted?.Invoke(data);

            PuzzleManager.Instance?.CheckPuzzleState();
        }
        else
        {
            Debug.Log($"Missing required item: {requiredItem} in inventory.");
        }
    }

}
