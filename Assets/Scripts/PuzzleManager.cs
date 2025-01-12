using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    [SerializeField] private List<InteractablePillar> pillars;
    [SerializeField] private UnityEvent onPuzzleCompleted;

    private bool isPuzzleSolved = false;
    private bool allPillarAreCorrect = false;

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
        allPillarAreCorrect = true;
        Debug.Log("Checking puzzle state...");
        foreach (var pillar in pillars)
        {
            //Debug.Log($"Pillar {pillar.name}: IsOccupied = {pillar.IsOccupied}, IsCorrectlyOccupied = {pillar.IsCorrectlyOccupied}");

            if (!pillar.IsCorrectlyOccupied)
            {
                Debug.Log($"Pillar {pillar.name} is not correctly occupied or is empty.");
                allPillarAreCorrect = false;
                //return; // At least one pillar is incorrect
            }
        }
        // If all pillars are correctly occupied, mark the puzzle as solved
        if (allPillarAreCorrect)
        {
            isPuzzleSolved = true;
            Debug.Log("Puzzle Solved!");
            onPuzzleCompleted?.Invoke();
        }
    }

}
