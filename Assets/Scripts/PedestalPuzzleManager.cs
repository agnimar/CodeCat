using UnityEngine;
using UnityEngine.Events;

public class PedestalPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Progress Settings")]
    [SerializeField] private int totalPuzzles = 5; 
    private int puzzlesSolved = 0;

    [Header("Events")]
    public UnityEvent onAllPuzzlesSolved;
    public UnityEvent onResetPuzzles;

    public void RegisterPuzzleSolved()
    {
        puzzlesSolved++;
        Debug.Log("Puzzle solved. Total solved: " + puzzlesSolved);

        if (puzzlesSolved >= totalPuzzles)
        {
            AllPuzzlesCompleted();
        }
    }

    public void ResetPuzzleProgress()
    {
        puzzlesSolved = 0;
        Debug.Log("Puzzle progress reset.");
        onResetPuzzles?.Invoke();
    }

    private void AllPuzzlesCompleted()
    {
        Debug.Log("All puzzles solved! Unlocking the Crystal Chamber.");
        onAllPuzzlesSolved?.Invoke();
    }
}
