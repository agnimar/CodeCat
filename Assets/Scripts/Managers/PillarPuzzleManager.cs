using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PillarPuzzleManager : MonoBehaviour
{
    public static PillarPuzzleManager Instance { get; private set; }

    [SerializeField] private List<InteractablePillar> pillars;
    [SerializeField] private GameObject entrance; 
    [SerializeField] private Animator entranceAnimator; 
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
            SoundManager.PlaySound(SoundType.PUZZLE_SOLVED);
            UnlockEntrance();
            onPuzzleCompleted?.Invoke();
        }
    }

    private void UnlockEntrance()
    {
        if (entranceAnimator != null)
        {
            entranceAnimator.SetTrigger("Unlock"); 
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
