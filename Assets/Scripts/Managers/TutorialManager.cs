using System;
using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public enum TutorialState
    {
        WaitingForLookAround,
        WaitingForCameraSwitch,
        WaitingForMovement,
        WaitingForBookArea,
        WaitingForBookInteraction,
        WaitingForBookAndInventory,
        WaitingForProceedDownRoad,
        Completed
    }

    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;

    private TutorialState currentState = TutorialState.WaitingForLookAround;

    public static TutorialManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        PlayerEvents.OnLookedAround += HandleLookedAround;
        PlayerEvents.OnCameraSwitched += HandleCameraSwitched;
        PlayerEvents.OnMoved += HandleMoved;
        BookTrigger.OnEnteredBookArea += HandleBookAreaReached;
        PlayerEvents.OnBookInteracted += HandleBookInteracted;
        InventoryEvents.OnBookCollected += HandleBookCollected;
        PlayerEvents.OnOpenedBookAndInventory += HandleBookAndInventoryOpened;
        PlayerEvents.OnProceedDownRoad += HandleProceedDownRoad;
    }

    private void OnDisable()
    {
        PlayerEvents.OnLookedAround -= HandleLookedAround;
        PlayerEvents.OnCameraSwitched -= HandleCameraSwitched;
        PlayerEvents.OnMoved -= HandleMoved;
        BookTrigger.OnEnteredBookArea -= HandleBookAreaReached;
        PlayerEvents.OnBookInteracted -= HandleBookInteracted;
        InventoryEvents.OnBookCollected -= HandleBookCollected;
        PlayerEvents.OnOpenedBookAndInventory -= HandleBookAndInventoryOpened;
        PlayerEvents.OnProceedDownRoad -= HandleProceedDownRoad;
    }

    private void Start()
    {
        dialogueManager.ShowMessage("Look around using your mouse.", 0);
        currentState = TutorialState.WaitingForLookAround;
    }
    public bool IsMovementAllowed => currentState >= TutorialState.WaitingForMovement;

    // --- Event Handlers ---
    private void HandleLookedAround()
    {
        if (currentState == TutorialState.WaitingForLookAround)
        {
            StartCoroutine(DelayedCameraSwitchPrompt());
        }
    }

    private IEnumerator DelayedCameraSwitchPrompt()
    {
        yield return new WaitForSeconds(3f);
        dialogueManager.ShowMessage("Press 'V' to switch between first and third person view.", 0);
        currentState = TutorialState.WaitingForCameraSwitch;
    }

    private void HandleCameraSwitched()
    {
        if (currentState == TutorialState.WaitingForCameraSwitch)
        {
            dialogueManager.ShowMessage("Move around using WASD.", 0);
            currentState = TutorialState.WaitingForMovement;
        }
    }

    private void HandleMoved()
    {
        if (currentState == TutorialState.WaitingForMovement)
        {
            dialogueManager.ShowMessage("Hmm, what's that on the ground?", 0);
            currentState = TutorialState.WaitingForBookArea;
        }
    }

    private void HandleBookAreaReached()
    {
        if (currentState == TutorialState.WaitingForBookArea)
        {
            dialogueManager.ShowMessage("Press 'E' to interact.", 0);
            currentState = TutorialState.WaitingForBookInteraction;
        }
    }

    private void HandleBookInteracted()
    {
        if (currentState == TutorialState.WaitingForBookInteraction)
        {
            currentState = TutorialState.WaitingForBookAndInventory;
        }
    }

    private void HandleBookCollected()
    {
        if (currentState == TutorialState.WaitingForBookAndInventory)
        {
            dialogueManager.ShowMessage("Press 'Q' to open the book and 'I' to open your inventory.", 0);
        }
    }

    private void HandleBookAndInventoryOpened()
    {
        if (currentState == TutorialState.WaitingForBookAndInventory)
        {
            dialogueManager.ShowMessage("<color=blue>Those mysterious variables.. I should look around</color>", 0);
            currentState = TutorialState.WaitingForProceedDownRoad;
        }
    }

    private void HandleProceedDownRoad()
    {
        if (currentState == TutorialState.WaitingForProceedDownRoad)
        {
            dialogueManager.HideMessage();
            currentState = TutorialState.Completed;
            // Tutorial is complete.
        }
    }
}
