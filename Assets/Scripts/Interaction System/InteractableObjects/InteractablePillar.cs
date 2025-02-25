using UnityEngine;

public class InteractablePillar : InteractableBase
{
    [SerializeField] private string requiredItemName;
    [SerializeField] private Transform itemPlacementPoint;
    private bool isOccupied = false;
    private GameObject currentItem; 
    private GameObject interactingPlayer;

    public bool IsOccupied => isOccupied;
    public bool IsCorrectlyOccupied => currentItem?.GetComponent<InteractableBase>()?.ItemName == requiredItemName;
    public override void OnInteractionStart(InteractionData data)
    {
        if (PillarPuzzleManager.Instance.IsPuzzleSolved())
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
            if(!UIManager.Instance.IsInventoryOpen)
                UIManager.Instance.ShowInventoryForSelection(OnItemSelected);
            else UIManager.Instance.CloseInventory();
        }
        SoundManager.PlaySound(SoundType.INTERACT);

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
        itemObject.transform.position = itemPlacementPoint.position; 
        itemObject.transform.SetParent(transform);
        SoundManager.PlaySound(SoundType.PLACE_ON_PILLAR);
        PillarPuzzleManager.Instance?.CheckPuzzleState();
    }

    private void RemoveItemFromPillar()
    {
        if (PillarPuzzleManager.Instance.IsPuzzleSolved())
        {
            Debug.LogWarning("Puzzle is solved. Items cannot be removed from the pillars.");
            return;
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
            currentItem.transform.position = transform.position + Vector3.down * 0.5f;
            currentItem.transform.SetParent(null);
            Debug.Log($"{currentItem.name} dropped at the base of the pillar.");
        }

        var interactable = currentItem.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.SetInteractionEnabled(true);
        }

        currentItem = null;
        isOccupied = false;
        SoundManager.PlaySound(SoundType.PICK_UP_OBJECT);

        PillarPuzzleManager.Instance?.CheckPuzzleState();
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
