using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BookManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject bookUI;
    [SerializeField] private TMP_Text leftPageText;
    [SerializeField] private TMP_Text rightPageText;

    [Header("Content")]
    [TextArea(5, 10)] public string[] bookPages;

    private int currentPage = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleBook();
        }
    }

    public void ToggleBook()
    {
        bool isActive = bookUI.activeSelf;
        bookUI.SetActive(!isActive);

        if (!isActive)
        {
            UpdateBookContent();
        }
    }

    public void NextPage()
    {
        if (currentPage < bookPages.Length - 2)
        {
            currentPage += 2;
            UpdateBookContent();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage -= 2;
            UpdateBookContent();
        }
    }

    private void UpdateBookContent()
    {
        leftPageText.text = currentPage < bookPages.Length ? bookPages[currentPage] : "";
        rightPageText.text = currentPage + 1 < bookPages.Length ? bookPages[currentPage + 1] : "";
    }
}
