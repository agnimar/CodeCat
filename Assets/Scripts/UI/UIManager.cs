using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private TMP_Text[] inventorySlotTexts;

    private InventoryManager inventoryManager;
    private Action<GameObject> selectionCallback;
    private bool isInventoryOpen = false;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple UIManager instances detected! Destroying duplicate.");
            Destroy(gameObject);
        }

        inventoryManager = FindAnyObjectByType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found in the scene!");
        }
    }

    private void Update()
    {
        HandleInventoryToggle();
    }

    // Toggle inventory visibility
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
        SetCursorLockState(!isInventoryOpen);

        if (isInventoryOpen)
        {
            UpdateInventoryUI();
        }
    }

    // Show inventory for selection with callback
    public void ShowInventoryForSelection(Action<GameObject> callback)
    {
        selectionCallback = callback;
        inventoryPanel.SetActive(true);
        isInventoryOpen = true;
        SetCursorLockState(false);

        UpdateInventoryUI();
    }

    // Handle clicking on inventory slots
    public void HandleInventorySlotClick(int index)
    {
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found.");
            selectionCallback?.Invoke(null);
            return;
        }

        var items = inventoryManager.GetItems();
        if (index >= 0 && index < items.Count)
        {
            GameObject selectedItem = items[index];
            selectionCallback?.Invoke(selectedItem);
            CloseInventory();
        }
        else
        {
            Debug.LogWarning("Invalid inventory slot clicked.");
            selectionCallback?.Invoke(null);
        }
    }

    // Update inventory UI
    private void UpdateInventoryUI()
    {
        if (inventoryManager == null) return;

        ClearInventorySlots();
        var items = inventoryManager.GetItems();

        for (int i = 0; i < items.Count && i < inventorySlotTexts.Length; i++)
        {
            var interactable = items[i].GetComponent<InteractableBase>();
            inventorySlotTexts[i].text = interactable != null ? interactable.ItemName : "Unknown Item";
        }
    }

    // Clear all inventory slots
    private void ClearInventorySlots()
    {
        foreach (var slotText in inventorySlotTexts)
        {
            slotText.text = ""; // Clear the text in each slot
        }
    }

    // Close the inventory UI
    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        isInventoryOpen = false;
        SetCursorLockState(true);
    }

    // Cursor lock state handling
    public void SetCursorLockState(bool lockCursor)
    {
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.Confined;
        Cursor.visible = !lockCursor;
    }
    private void HandleRightClick()
    {
        if (Input.GetMouseButtonDown(1) && isInventoryOpen) // Right-click while inventory is open
        {
            Vector2 mousePosition = Input.mousePosition;

            for (int i = 0; i < inventorySlotTexts.Length; i++)
            {
                // Check if the mouse position overlaps with a slot
                RectTransform slotRectTransform = inventorySlotTexts[i].rectTransform;
                if (RectTransformUtility.RectangleContainsScreenPoint(slotRectTransform, mousePosition))
                {
                    Debug.Log($"Right-clicked on slot {i}");

                    if (inventoryManager == null)
                    {
                        Debug.LogError("InventoryManager not found!");
                        return;
                    }

                    inventoryManager.DropItem(i); // Drop the item from the inventory
                    UpdateInventoryUI(); // Refresh the inventory display
                    break;
                }
            }
        }
    }
    private void HandleInventoryToggle()
    {
        // Close inventory if ESC is pressed
        if (isInventoryOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventory();
        }

        // Open/close inventory with 'I'
        if (Input.GetKeyDown(KeyCode.I))
        {
            isInventoryOpen = !isInventoryOpen;
            inventoryPanel.SetActive(isInventoryOpen);
            SetCursorLockState(!isInventoryOpen);

            if (isInventoryOpen)
            {
                UpdateInventoryUI();
            }
        }
    }

}
