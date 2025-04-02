using UnityEngine;
using UnityEngine.Events;

public class InteractablePedestal : InteractableBase
{
    [Header("Puzzle Settings")]
    [SerializeField] private string puzzleQuestion;
    [SerializeField] private string[] multipleChoiceOptions;
    [SerializeField] private int[] correctAnswerIndices;

    [Header("Crystal Settings")]
    [SerializeField] private CrystalActivation linkedCrystal;

    [Header("Global Manager")]
    [SerializeField] private CrystalManager crystalManager;

    [Header("UI Settings")]
    [SerializeField] private PuzzleUIManager puzzleUIManager;

    [Header("State Visuals")]
    [SerializeField] private GameObject activeStateGO;
    [SerializeField] private GameObject inactiveStateGO;

    private bool puzzleSolved = false;
    private bool isLocked = false;

    private void Awake()
    {
        UpdateVisualState();
    }

    public override void OnInteractionStart(InteractionData data)
    {
        if (puzzleSolved)
        {
            Debug.Log($"{itemName} puzzle already solved. No further interaction allowed.");
            return;
        }

        if (isLocked)
        {
            Debug.Log($"{itemName} is temporarily locked. Please wait.");
            return;
        }

        onInteractionStarted?.Invoke(data);

        if (puzzleUIManager != null)
        {
            if (!puzzleUIManager.isPanelActive())
                puzzleUIManager.OpenPuzzleUI(puzzleQuestion, multipleChoiceOptions, correctAnswerIndices, this);
            else puzzleUIManager.ClosePuzzleUI();
        }
        else
        {
            Debug.LogWarning("PuzzleUIManager is not assigned on " + gameObject.name);
        }
        SoundManager.PlaySound(SoundType.INTERACT);
    }

    public void OnPuzzleSolved()
    {
        puzzleSolved = true;
        Debug.Log($"{itemName} puzzle solved. Activating linked crystal.");

        if (linkedCrystal != null)
        {
            linkedCrystal.Activate();
            if (crystalManager != null)
            {
                crystalManager.CheckCrystalsState();
            }
            else
            {
                Debug.LogWarning("CrystalManager is not assigned on " + gameObject.name);
            }
        }
        else
        {
            Debug.LogWarning("Linked Crystal is not assigned on " + gameObject.name);
        }
        SoundManager.PlaySound(SoundType.SUCCESS);
        UpdateVisualState();
    }

    public void OnPuzzleFailed()
    {
        Debug.Log($"{itemName} puzzle failed. Deactivating all crystals.");
        if (crystalManager != null)
        {
            crystalManager.DeactivateAllCrystals();
        }
        else
        {
            Debug.LogWarning("CrystalManager is not assigned on " + gameObject.name);
        }

        isLocked = true;
        Invoke(nameof(ResetAllPedestals), 2f);
    }

    private void ResetAllPedestals()
    {
        InteractablePedestal[] allPedestals = FindObjectsOfType<InteractablePedestal>();
        foreach (InteractablePedestal pedestal in allPedestals)
        {
            pedestal.ResetPuzzle();
        }
    }

    public void ResetPuzzle()
    {
        puzzleSolved = false;
        isLocked = false;

        if (puzzleUIManager != null)
        {
            puzzleUIManager.ResetPuzzleUI();
        }

        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        if (activeStateGO != null)
        {
            activeStateGO.SetActive(puzzleSolved);
        }

        if (inactiveStateGO != null)
        {
            inactiveStateGO.SetActive(!puzzleSolved);
        }
    }
}
