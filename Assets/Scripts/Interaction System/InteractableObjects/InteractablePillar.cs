using UnityEngine;

public class InteractablePillar : InteractableBase
{
    [SerializeField] private string requiredItemName; // Specify the required item name for this pillar
    private bool isOccupied = false;
    private GameObject currentItem; // The item currently placed on the pillar
    private GameObject interactingPlayer;

    public bool IsOccupied => isOccupied;
    public bool IsCorrectlyOccupied => currentItem?.GetComponent<InteractableBase>()?.ItemName == requiredItemName;
    public override void OnInteractionStart(InteractionData data)
    {
        if (PuzzleManager.Instance.IsPuzzleSolved())
        {
            Debug.Log("Puzzle is solved; pillars are locked.");
            return;
        }
        interactingPlayer = data.Interactor;

        if (isOccupied)
        {
            RemoveItemFromPillar();
        }
        else
        {
            UIManager.Instance.ShowInventoryForSelection(OnItemSelected);
        }
    }

    private void OnItemSelected(GameObject selectedItem)
    {
        if (selectedItem == null)
        {
            Debug.Log("No item selected.");
            return;
        }

        var inventory = interactingPlayer.GetComponent<InventoryManager>();
        if (inventory == null)
        {
            Debug.LogError("Interactor missing InventoryManager!");
            return;
        }

        PlaceItemOnPillar(selectedItem);
        inventory.GetAndRemoveItem(selectedItem.GetComponent<InteractableBase>().ItemName);
    }

    private void PlaceItemOnPillar(GameObject itemObject)
    {
        if (itemObject == null)
        {
            Debug.LogError($"Cannot place a null item on {name}");
            return;
        }

        currentItem = itemObject;
        isOccupied = true;


        var interactable = currentItem.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.SetInteractionEnabled(false);
        }

        itemObject.SetActive(true);
        itemObject.transform.position = transform.position + Vector3.up * 1.0f; // Adjust height
        itemObject.transform.SetParent(transform);

        PuzzleManager.Instance?.CheckPuzzleState();
    }

    private void RemoveItemFromPillar()
    {
        if (PuzzleManager.Instance.IsPuzzleSolved())
        {
            Debug.LogWarning("Puzzle is solved. Items cannot be removed from the pillars.");
            return; // Prevent item removal if the puzzle is solved
        }
        if (currentItem == null)
        {
            Debug.LogWarning("No item to remove from the pillar.");
            return;
        }

        var inventory = interactingPlayer.GetComponent<InventoryManager>();
        if (inventory != null && inventory.AddItem(currentItem))
        {
            Debug.Log($"{currentItem.name} returned to inventory.");
        }
        else
        {
            currentItem.SetActive(true);
            currentItem.transform.position = transform.position + Vector3.down * 0.5f; // Adjust drop height
            currentItem.transform.SetParent(null);
            Debug.Log($"{currentItem.name} dropped at the base of the pillar.");
        }

        // Re-enable interaction on the removed item
        var interactable = currentItem.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.SetInteractionEnabled(true);
        }

        currentItem = null;
        isOccupied = false;

        PuzzleManager.Instance?.CheckPuzzleState();
    }

    public void ResetPillar()
    {
        if (currentItem != null)
        {
            currentItem.SetActive(false);
            currentItem.transform.SetParent(null);
        }

        isOccupied = false;
        currentItem = null;
    }
}
