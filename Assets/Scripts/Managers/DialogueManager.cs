using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Settings")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Prompt Settings")]
    [Tooltip("Configure the interactable prompt entries.")]
    public PromptEntry[] promptEntries;

    [Header("Player Reference")]
    [Tooltip("Assign the player's transform here.")]
    [SerializeField] private Transform playerTransform;

    private string lastCombinedPrompt = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple DialogueManager instances detected. Destroying duplicate.");
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

    private void Update()
    {
        if (playerTransform == null)
            return;

        string combinedPrompt = "";
        foreach (PromptEntry entry in promptEntries)
        {
            Collider[] hits = Physics.OverlapSphere(playerTransform.position, entry.distanceThreshold, entry.targetLayer);
            if (hits.Length > 0)
            {
                combinedPrompt += entry.message + "\n";
            }
        }
        combinedPrompt = combinedPrompt.TrimEnd('\n');

        if (!string.IsNullOrEmpty(combinedPrompt))
        {
            if (combinedPrompt != lastCombinedPrompt)
            {
                ShowMessage(combinedPrompt, 0f);
                lastCombinedPrompt = combinedPrompt;
            }
        }
        else if (!string.IsNullOrEmpty(lastCombinedPrompt))
        {
            HideMessage();
            lastCombinedPrompt = "";
        }
    }

    public void ShowMessage(string message, float duration = 0f)
    {
        StopAllCoroutines();
        dialogueText.text = message;
        dialoguePanel.SetActive(true);

        if (duration > 0f)
        {
            StartCoroutine(HideAfterDuration(duration));
        }
    }

    private IEnumerator HideAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        HideMessage();
    }

    public void HideMessage()
    {
        dialoguePanel.SetActive(false);
    }
}

[Serializable]
public class PromptEntry
{
    public string message;
    public LayerMask targetLayer;
    public float distanceThreshold = 2f;
    public KeyCode interactKey = KeyCode.E;
    public bool autoShow = true;
}
