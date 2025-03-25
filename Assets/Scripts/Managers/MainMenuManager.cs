using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    public static MainMenuUIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject controlsPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple MainMenuUIManager instances detected! Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        if (controlsPanel != null)
            controlsPanel.SetActive(false);
    }

    public void NewGame()
    {
        Debug.Log("New Game button clicked");
        SceneManager.LoadScene("Chapter_1");
    }

    public void ShowControls()
    {
        Debug.Log("Controls button clicked");
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Controls panel is not assigned in the Inspector!");
        }
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked");
        Application.Quit();
    }

    public void BackToMainMenuFromControls()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(false);
    }
}
