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
    }

    private void OnDisable()
    {
        PlayerEvents.OnLookedAround -= HandleLookedAround;
        PlayerEvents.OnCameraSwitched -= HandleCameraSwitched;
        PlayerEvents.OnMoved -= HandleMoved;
        BookTrigger.OnEnteredBookArea -= HandleBookAreaReached;
        PlayerEvents.OnBookInteracted -= HandleBookInteracted;
    }

    private void Start()
    {
        dialogueManager.ShowMessage("Look around using your mouse.", 0);
        currentState = TutorialState.WaitingForLookAround;
    }
    public bool IsMovementAllowed => currentState >= TutorialState.WaitingForMovement;

    // --- Helper Coroutine ---
    private IEnumerator DelayedMessage(string message, TutorialState nextState, float messageDuration)
    {
        yield return new WaitForSeconds(messageDuration);
        dialogueManager.ShowMessage(message, 0);
        currentState = nextState;
    }
    private IEnumerator DelayedHide()
    {
        yield return new WaitForSeconds(2f);
        dialogueManager.HideMessage();
        currentState = TutorialState.Completed;
    }

    // --- Event Handlers ---
    private void HandleLookedAround()
    {
        if (currentState == TutorialState.WaitingForLookAround)
        {
            StartCoroutine(DelayedMessage("Press 'V' to switch between first and third person view.", TutorialState.WaitingForCameraSwitch, 2f));
        }
    }
    private IEnumerator DelayedStateUpdate(TutorialState nextState, float delayDuration)
    {
        yield return new WaitForSeconds(delayDuration);
        currentState = nextState;
    }
    private void HandleCameraSwitched()
    {
        if (currentState == TutorialState.WaitingForCameraSwitch)
        {
            StartCoroutine(DelayedMessage("Move around using WASD.", TutorialState.WaitingForMovement, 2f));
        }
    }

    private void HandleMoved()
    {
        if (currentState == TutorialState.WaitingForMovement)
        {
            StartCoroutine(DelayedMessage("Hmm, what's that on the ground?", TutorialState.WaitingForBookArea, 2f));
        }
    }

    private void HandleBookAreaReached()
    {
        if (currentState == TutorialState.WaitingForBookArea)
        {
            StartCoroutine(DelayedMessage("Press 'E' to interact.", TutorialState.WaitingForBookInteraction, 0.1f));
        }
    }

    private void HandleBookInteracted()
    {
        if (currentState == TutorialState.WaitingForBookInteraction)
        {
            dialogueManager.ShowMessage("Press 'Q' to open the book and 'I' to open your inventory.", 3f);
            currentState = TutorialState.Completed;
        }
    }

}
