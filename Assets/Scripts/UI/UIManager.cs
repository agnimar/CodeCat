using UnityEngine;
using TMPro; // Include TextMeshPro namespace

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject inventoryPanel; // Panel to display inventory
    [SerializeField] private TMP_Text[] inventorySlotTexts; // Array of TMP_Text for 8 slots
    private bool isInventoryOpen = false;
    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false); // Start with inventory hidden
        }
        else
        {
            Debug.LogError("Inventory panel is not assigned!");
        }

        ClearInventorySlots(); // Ensure all slots are initially empty
    }

    private void Update()
    {
        HandleInventoryToggle();
    }

    private void HandleInventoryToggle()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isInventoryOpen = !isInventoryOpen;
            inventoryPanel.SetActive(isInventoryOpen);

            if (isInventoryOpen)
            {
                UpdateInventoryUI();
            }
        }
    }

    public void UpdateInventoryUI()
    {
        ClearInventorySlots();

        var items = inventoryManager.GetItems();
        for (int i = 0; i < items.Count && i < inventorySlotTexts.Length; i++)
        {
            var interactable = items[i].GetComponent<InteractableBase>();
            if (interactable != null)
            {
                inventorySlotTexts[i].text = interactable.ItemName; // Use the ItemName property
            }
            else
            {
                inventorySlotTexts[i].text = "Unknown Item";
            }
        }
    }


    private void ClearInventorySlots()
    {
        foreach (var slotText in inventorySlotTexts)
        {
            slotText.text = ""; // Clear the text in each slot
        }
    }
}
