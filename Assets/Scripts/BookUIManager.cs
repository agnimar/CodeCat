using UnityEngine;
using TMPro;

public class BookUIManager : MonoBehaviour
{
    public static BookUIManager Instance { get; private set; }

    [Header("Book UI Elements")]
    [SerializeField] private GameObject bookPanel;

    [Header("Book Pages")]
    [SerializeField] private GameObject[] pageGroups;

    private int currentPageIndex = 0;
    private bool isBookUnlocked = false;
    private bool isBookUIOpen = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
        if (isBookUnlocked && Input.GetKeyDown(KeyCode.Q))
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
        SoundManager.PlaySound(SoundType.PICK_UP_BOOK);
    }

    public void ToggleBookUI()
    {
        if (!isBookUnlocked) return;
        isBookUIOpen = !isBookUIOpen;
        
        if (bookPanel != null)
        {
            bookPanel.SetActive(isBookUIOpen);
        }

        if (isBookUIOpen)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        SoundManager.PlaySound(SoundType.OPEN_UI);

    }

    public void SetBookContent(string content)
    {
        //if (bookText != null)
        //{
        //    bookText.text = content;
        //}
    }
    public void NextPage()
    {
        if (pageGroups == null || pageGroups.Length == 0) return;
        if (currentPageIndex >= pageGroups.Length - 1) return;

        pageGroups[currentPageIndex].SetActive(false);

        currentPageIndex++;
        if (currentPageIndex >= pageGroups.Length)
        {
            currentPageIndex = 0;
        }

        pageGroups[currentPageIndex].SetActive(true);
    }
    public void PrevPage()
    {
        if (pageGroups == null || pageGroups.Length == 0) return;
        if (currentPageIndex <= 0) return;

        pageGroups[currentPageIndex].SetActive(false);

        currentPageIndex--;
        if (currentPageIndex < 0)
        {
            currentPageIndex = pageGroups.Length - 1;
        }

        pageGroups[currentPageIndex].SetActive(true);
    }
    private void UpdatePageVisibility()
    {
        if (pageGroups == null || pageGroups.Length == 0) return;

        for (int i = 0; i < pageGroups.Length; i++)
        {
            pageGroups[i].SetActive(i == currentPageIndex);
        }
    }
    public bool IsBookUIOpen => bookPanel.activeSelf;
}
