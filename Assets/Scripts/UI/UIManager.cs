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
        inventoryManager = FindObjectOfType<InventoryManager>();
        // Ensure a single instance of UIManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple UIManager instances detected! Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        HandleInventoryToggle();
        HandleRightClick();
    }

    public void ShowInventoryForSelection(Action<GameObject> callback)
    {
        selectionCallback = callback;
        inventoryPanel.SetActive(true);
        isInventoryOpen = true;
        SetCursorLockState(false);

        // Ensure the inventory UI is updated
        UpdateInventoryUI();
    }

    public void HandleInventorySlotClick(int index)
    {
        var inventoryManager = FindObjectOfType<InventoryManager>();
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

            // Close inventory after selection
            CloseInventory();
        }
        else
        {
            Debug.LogWarning("Invalid inventory slot clicked.");
            selectionCallback?.Invoke(null);
        }
    }

    private void HandleInventoryToggle()
    {
        // Close inventory if ESC is pressed
        if (isInventoryOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventory();
        }

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

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        isInventoryOpen = false;
        SetCursorLockState(true);
    }

    private void UpdateInventoryUI()
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
    public void SetCursorLockState(bool lockCursor)
    {
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.Confined;
        Cursor.visible = !lockCursor;
    }
    private void HandleRightClick()
    {
        if (Input.GetMouseButtonDown(1) && isInventoryOpen) // Right-click
        {
            Vector2 mousePosition = Input.mousePosition;
            for (int i = 0; i < inventorySlotTexts.Length; i++)
            {
                RectTransform slotRectTransform = inventorySlotTexts[i].rectTransform;
                if (RectTransformUtility.RectangleContainsScreenPoint(slotRectTransform, mousePosition))
                {
                    Debug.Log($"Right-clicked on slot {i}");
                    inventoryManager.DropItem(i);
                    UpdateInventoryUI(); // Refresh the UI after dropping the item
                    break;
                }
            }
        }
    }
}
