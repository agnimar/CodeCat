using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject puzzlePanel;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Transform optionsContainer;
    [SerializeField] private Toggle optionTogglePrefab;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button resumeButton;

    private List<Toggle> optionToggles = new List<Toggle>();
    private InteractablePedestal currentPedestal;
    private int[] correctAnswerIndices;

    private bool showCorrectAnswersFlag = false;
    private void Update()
    {
        if (puzzlePanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePuzzleUI();
        }
    }

    public void ClosePuzzleUI()
    {
        puzzlePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenPuzzleUI(string puzzleQuestion, string[] options, int[] correctIndices, InteractablePedestal pedestal, bool showCorrectAnswers = false)
    {
        currentPedestal = pedestal;
        correctAnswerIndices = correctIndices;
        showCorrectAnswersFlag = showCorrectAnswers;

        puzzlePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        questionText.text = puzzleQuestion;

        foreach (Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }
        optionToggles.Clear();

        for (int i = 0; i < options.Length; i++)
        {
            Toggle toggle = Instantiate(optionTogglePrefab, optionsContainer);
            Text toggleText = toggle.GetComponentInChildren<Text>();
            if (toggleText != null)
            {
                toggleText.text = options[i];
            }

            if (showCorrectAnswersFlag && System.Array.Exists(correctAnswerIndices, index => index == i))
            {
                toggle.isOn = true;
                toggle.interactable = false;
            }
            else
            {
                toggle.isOn = false;
                toggle.interactable = true;
            }
            optionToggles.Add(toggle);
        }

        submitButton.onClick.RemoveAllListeners();
        resumeButton.onClick.RemoveAllListeners();

        if (!showCorrectAnswersFlag)
        {
            submitButton.onClick.AddListener(OnSubmit);
            submitButton.interactable = true;
            resumeButton.onClick.AddListener(OnResume);
            resumeButton.interactable = true;
        }
        else
        {
            submitButton.interactable = false;
            resumeButton.interactable = false;
        }
    }

    private void OnSubmit()
    {
        Debug.Log("Submit button clicked.");
        List<int> selectedIndices = new List<int>();

        for (int i = 0; i < optionToggles.Count; i++)
        {
            if (optionToggles[i].isOn)
            {
                selectedIndices.Add(i);
            }
        }

        bool isCorrect = CheckAnswers(selectedIndices, correctAnswerIndices);
        if (isCorrect)
        {
            currentPedestal.OnPuzzleSolved();
        }
        else
        {
            currentPedestal.OnPuzzleFailed();
        }

        ClosePuzzleUI();
    }
    private void OnResume()
    {
        ClosePuzzleUI();
    }

    private bool CheckAnswers(List<int> selected, int[] correct)
    {
        Debug.Log("Selected Count: " + selected.Count + " | Correct Count: " + correct.Length);
        Debug.Log("Selected indices: " + string.Join(", ", selected));
        Debug.Log("Correct indices: " + string.Join(", ", correct));
        if (selected.Count != correct.Length)
            return false;

        selected.Sort();
        List<int> correctList = new List<int>(correct);
        correctList.Sort();

        for (int i = 0; i < selected.Count; i++)
        {
            if (selected[i] != correctList[i])
                return false;
        }
        return true;
    }

    public void ResetPuzzleUI()
    {
        showCorrectAnswersFlag = false;

        if (puzzlePanel.activeSelf)
        {
            foreach (Toggle toggle in optionToggles)
            {
                toggle.isOn = false;
                toggle.interactable = true;
            }
            submitButton.interactable = true;
            resumeButton.interactable = true;
        }
    }
    public bool isPanelActive()
    {
        return puzzlePanel.activeSelf;
    }
}
