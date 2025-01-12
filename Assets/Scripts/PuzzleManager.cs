using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    [SerializeField] private List<InteractablePillar> pillars;
    [SerializeField] private UnityEvent onPuzzleCompleted;

    private bool isPuzzleSolved = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple PuzzleManager instances found. Destroying the duplicate.");
            Destroy(gameObject);
        }
    }

    public void CheckPuzzleState()
    {
        if (isPuzzleSolved)
        {
            Debug.Log("Puzzle is already solved.");
            return;
        }

        bool allPillarsCorrect = true;
        foreach (var pillar in pillars)
        {
            if (!pillar.IsCorrectlyOccupied)
            {
                allPillarsCorrect = false;
                break;
            }
        }

        if (allPillarsCorrect)
        {
            isPuzzleSolved = true;
            Debug.Log("Puzzle Solved!");
            onPuzzleCompleted?.Invoke();
        }
    }

    public bool IsPuzzleSolved()
    {
        return isPuzzleSolved;
    }
}
