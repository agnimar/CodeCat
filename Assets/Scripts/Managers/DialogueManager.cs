using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue Panel Settings")]
    [Tooltip("The panel that will display dialogue messages.")]
    [SerializeField] private GameObject displayInfoPanel;

    [Tooltip("The TextMeshPro component used to show the message.")]
    [SerializeField] private TMP_Text displayInfoText;

    private Coroutine currentMessageCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple DialogueManager instances detected! Destroying duplicate.");
            Destroy(gameObject);
        }

        if (displayInfoPanel != null)
        {
            displayInfoPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Display Info Panel is not assigned in the Inspector!");
        }
    }

    public void ShowMessage(string message, float duration)
    {
        if (currentMessageCoroutine != null)
        {
            StopCoroutine(currentMessageCoroutine);
        }

        if (displayInfoText != null)
        {
            displayInfoText.text = message;
        }
        else
        {
            Debug.LogError("Display Info Text is not assigned in the Inspector!");
            return;
        }

        displayInfoPanel.SetActive(true);
        if (duration > 0)
            currentMessageCoroutine = StartCoroutine(HideAfterDuration(duration));
    }

    private IEnumerator HideAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        HideMessage();
    }

    public void HideMessage()
    {
        if (currentMessageCoroutine != null)
        {
            StopCoroutine(currentMessageCoroutine);
            currentMessageCoroutine = null;
        }

        displayInfoPanel.SetActive(false);
    }
}
