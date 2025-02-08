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
        HandleRightClick();
    }

    public void ShowInventoryForSelection(Action<GameObject> callback)
    {
        selectionCallback = callback;
        inventoryManager.IsOpenedByPlayer = false;
        inventoryPanel.SetActive(true);
        isInventoryOpen = true;
        SetCursorLockState(false);

        UpdateInventoryUI();
    }

    public void HandleInventorySlotClick(int index)
    {
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found.");
            selectionCallback?.Invoke(null);
            return;
        }
        if (inventoryManager.IsOpenedByPlayer)
        {
            Debug.LogWarning("Cannot interact with inventory slots when opened manually.");
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
    private void ClearInventorySlots()
    {
        foreach (var slotText in inventorySlotTexts)
        {
            slotText.text = "";
        }
    }
    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        inventoryManager.IsOpenedByPlayer = false;
        isInventoryOpen = false;
        SetCursorLockState(true);
    }
    public void SetCursorLockState(bool lockCursor)
    {
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.Confined;
        Cursor.visible = !lockCursor;
    }
    private void HandleRightClick()
    {
        if (Input.GetMouseButtonDown(1) && isInventoryOpen)
        {
            Vector2 mousePosition = Input.mousePosition;

            for (int i = 0; i < inventorySlotTexts.Length; i++)
            {
                RectTransform slotRectTransform = inventorySlotTexts[i].rectTransform;
                if (RectTransformUtility.RectangleContainsScreenPoint(slotRectTransform, mousePosition))
                {
                    Debug.Log($"Right-clicked on slot {i}");

                    if (inventoryManager == null)
                    {
                        Debug.LogError("InventoryManager not found!");
                        return;
                    }

                    inventoryManager.DropItem(i);
                    UpdateInventoryUI();
                    break;
                }
            }
        }
    }
    private void HandleInventoryToggle()
    {
        if (isInventoryOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventory();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (BookUIManager.Instance != null && BookUIManager.Instance.IsBookUIOpen) return;
            inventoryManager.IsOpenedByPlayer = true;
            SoundManager.PlaySound(SoundType.OPEN_UI);
            isInventoryOpen = !isInventoryOpen;
            inventoryPanel.SetActive(isInventoryOpen);
            SetCursorLockState(!isInventoryOpen);

            if (isInventoryOpen)
            {
                UpdateInventoryUI();
            }
        }
    }
    public bool IsInventoryOpen => inventoryPanel.activeSelf;
}
