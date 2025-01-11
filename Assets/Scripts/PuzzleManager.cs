using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        if (isPuzzleSolved) return;

        foreach (var pillar in pillars)
        {
            if (!pillar.IsOccupied)
            {
                return; // Puzzle not solved yet
            }
        }

        isPuzzleSolved = true;
        Debug.Log("Puzzle Solved!");
        onPuzzleCompleted.Invoke(); // Trigger completion event
    }
}
