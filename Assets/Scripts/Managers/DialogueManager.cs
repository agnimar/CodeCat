using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Settings")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Tutorial Settings")]
    [SerializeField] private bool playTutorialOnStart = true;
    [SerializeField] private DialogueEntry[] tutorialEntries;

    [Header("Player References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInteraction playerInteraction;

    private Coroutine currentDialogueCoroutine;

    [System.Serializable]
    public class DialogueEntry
    {
        public string message;
        public float duration;
        public bool lockMovement;
        public bool lockInteraction;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple DialogueManagerAdvanced instances found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Dialogue Panel not assigned in the Inspector!");
        }
    }

    private void Start()
    {
        if (playTutorialOnStart && tutorialEntries != null && tutorialEntries.Length > 0)
        {
            StartCoroutine(RunTutorialSequence());
        }
    }

    public void ShowMessage(string message, float duration = 0f)
    {
        if (currentDialogueCoroutine != null)
            StopCoroutine(currentDialogueCoroutine);

        dialogueText.text = message;
        dialoguePanel.SetActive(true);

        if (duration > 0)
        {
            currentDialogueCoroutine = StartCoroutine(HideAfterDuration(duration));
        }
    }

    private IEnumerator HideAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        HideMessage();
    }

    public void HideMessage()
    {
        if (currentDialogueCoroutine != null)
        {
            StopCoroutine(currentDialogueCoroutine);
            currentDialogueCoroutine = null;
        }
        dialoguePanel.SetActive(false);
    }

    private IEnumerator RunTutorialSequence()
    {
        if (playerController != null)
            playerController.enabled = false;
        if (playerInteraction != null)
            playerInteraction.enabled = false;

        foreach (var entry in tutorialEntries)
        {
            if (entry.lockMovement && playerController != null)
                playerController.enabled = false;
            if (entry.lockInteraction && playerInteraction != null)
                playerInteraction.enabled = false;

            ShowMessage(entry.message, entry.duration);
            yield return new WaitForSeconds(entry.duration + 0.5f); // Small delay between messages.
        }

        if (playerController != null)
            playerController.enabled = true;
        if (playerInteraction != null)
            playerInteraction.enabled = true;

        HideMessage();
    }

    public void ShowInteractPrompt(string prompt = "Press E to interact")
    {
        ShowMessage(prompt, 0);
    }
}
