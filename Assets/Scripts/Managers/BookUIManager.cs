using UnityEngine;
using UnityEngine.UI; // Needed for Button
using TMPro;

public class BookUIManager : MonoBehaviour
{
    public static BookUIManager Instance { get; private set; }

    [Header("Book UI Elements")]
    [SerializeField] private GameObject bookPanel;
    [SerializeField] private Button nextPageButton; 
    [SerializeField] private Button prevPageButton;
    [SerializeField] public bool isBookUnlocked = false;

    [Header("Book Pages")]
    [SerializeField] private GameObject[] pageGroups;

    private int currentPageIndex = 0;
    private bool isBookUIOpen = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Multiple instances of BookUIManager detected! Destroying duplicate.");
            Destroy(gameObject);
        }

        if (bookPanel != null)
        {
            bookPanel.SetActive(false);
        }
        
    }

    private void Start()
    {
        UpdatePageVisibility();
    }

    private void Update()
    {
        if (isBookUnlocked && Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleBookUI();
        }

        if (isBookUIOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleBookUI();
        }
    }

    public void UnlockBookUI()
    {
        isBookUnlocked = true;
        PlayerPrefs.SetInt("BookUnlocked", 1);
        PlayerPrefs.Save();
        SoundManager.PlaySound(SoundType.PICK_UP_BOOK);
    }

    public void ToggleBookUI()
    {
        if (!isBookUnlocked) return;
        if (SettingsOptionsUI.Instance != null && SettingsOptionsUI.Instance.IsAnyMenuOpen) return;
        if (!isBookUIOpen && UIManager.Instance != null && UIManager.Instance.IsInventoryOpen) return;

        isBookUIOpen = !isBookUIOpen;

        if (bookPanel != null)
        {
            bookPanel.SetActive(isBookUIOpen);
        }

        if (isBookUIOpen)
        {
            //PlayerEvents.OpenedBookAndInventory();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            //PlayerEvents.ProceedDownRoad();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        SoundManager.PlaySound(SoundType.OPEN_UI);
    }

    public void NextPage()
    {
        if (pageGroups == null || pageGroups.Length == 0) return;
        if (currentPageIndex >= pageGroups.Length - 1) return;

        pageGroups[currentPageIndex].SetActive(false);
        currentPageIndex++;
        SoundManager.PlaySound(SoundType.OPEN_UI);
        pageGroups[currentPageIndex].SetActive(true);
        UpdatePageVisibility(); 
    }

    public void PrevPage()
    {
        if (pageGroups == null || pageGroups.Length == 0) return;
        if (currentPageIndex <= 0) return;

        pageGroups[currentPageIndex].SetActive(false);
        currentPageIndex--;
        SoundManager.PlaySound(SoundType.OPEN_UI);
        pageGroups[currentPageIndex].SetActive(true);
        UpdatePageVisibility();
    }

    private void UpdatePageVisibility()
    {
        if (pageGroups == null || pageGroups.Length == 0) return;

        for (int i = 0; i < pageGroups.Length; i++)
        {
            pageGroups[i].SetActive(i == currentPageIndex);
        }

        bool canGoPrev = currentPageIndex > 0;
        bool canGoNext = currentPageIndex < pageGroups.Length - 1;
        
        if (prevPageButton != null)
        {
            prevPageButton.interactable = canGoPrev;
            TextMeshProUGUI prevText = prevPageButton.GetComponentInChildren<TextMeshProUGUI>();
            if (prevText != null)
            {
                Color color = prevText.color;
                color.a = canGoPrev ? 1f : 0.5f;
                prevText.color = color;
            }
        }

        if (nextPageButton != null)
        {
            nextPageButton.interactable = canGoNext;
            TextMeshProUGUI nextText = nextPageButton.GetComponentInChildren<TextMeshProUGUI>();
            if (nextText != null)
            {
                Color color = nextText.color;
                color.a = canGoNext ? 1f : 0.5f;
                nextText.color = color;
            }
        }
    }


    public bool IsBookUIOpen => bookPanel.activeSelf;
}
