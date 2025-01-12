using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    [SerializeField] private List<InteractablePillar> pillars;
    [SerializeField] private GameObject entrance; // Reference to the entrance
    [SerializeField] private Animator entranceAnimator; // Animator for the entrance
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

            UnlockEntrance(); // Trigger the entrance unlocking
            onPuzzleCompleted?.Invoke();
        }
    }

    private void UnlockEntrance()
    {
        if (entranceAnimator != null)
        {
            entranceAnimator.SetTrigger("Unlock"); // Play unlock animation
        }
        else
        {
            Debug.LogError("Entrance Animator is not assigned!");
        }
    }
    public bool IsPuzzleSolved()
    {
        return isPuzzleSolved;
    }
}
