using UnityEngine;
using TMPro;

public class BookUIManager : MonoBehaviour
{
    public static BookUIManager Instance { get; private set; }

    [Header("Book UI Elements")]
    [SerializeField] private GameObject bookPanel;
    //[SerializeField] private TMP_Text bookText;

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

    private void Update()
    {
        if (isBookUnlocked && Input.GetKeyDown(KeyCode.Q))
        {
            ToggleBookUI();
        }
    }

    public void UnlockBookUI()
    {
        isBookUnlocked = true;
        Debug.Log("Book UI unlocked! Press Q to open.");
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
    }

    public void SetBookContent(string content)
    {
        //if (bookText != null)
        //{
        //    bookText.text = content;
        //}
    }

    public bool IsBookUIOpen => bookPanel.activeSelf;
}
