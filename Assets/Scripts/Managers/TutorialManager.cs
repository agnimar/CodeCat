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
        TutorialEndState,
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
        PlayerEvents.OnTutorialEnded += HandleTutorialEnd;
    }

    private void OnDisable()
    {
        PlayerEvents.OnLookedAround -= HandleLookedAround;
        PlayerEvents.OnCameraSwitched -= HandleCameraSwitched;
        PlayerEvents.OnMoved -= HandleMoved;
        BookTrigger.OnEnteredBookArea -= HandleBookAreaReached;
        PlayerEvents.OnBookInteracted -= HandleBookInteracted;
        PlayerEvents.OnTutorialEnded -= HandleTutorialEnd;
    }

    private void Start()
    {
        dialogueManager.ShowMessage("Look around using your mouse.", 0);
        currentState = TutorialState.WaitingForLookAround;
    }
    public bool IsMovementAllowed => currentState >= TutorialState.WaitingForMovement;

    // --- Helper Coroutine ---
    private IEnumerator DelayedMessage(string message, TutorialState nextState, float messageDuration, float delayDuration)
    {
        yield return new WaitForSeconds(delayDuration);
        dialogueManager.ShowMessage(message, messageDuration);
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
            StartCoroutine(DelayedMessage("Press 'V' to switch between first and third person view.", TutorialState.WaitingForCameraSwitch, 0, 2f));
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
            StartCoroutine(DelayedMessage("Move around using WASD. Sprint using SHIFT", TutorialState.WaitingForMovement, 0, 2f));
        }
    }

    private void HandleMoved()
    {
        if (currentState == TutorialState.WaitingForMovement)
        {
            StartCoroutine(DelayedMessage("Hmm, what's that on the ground?", TutorialState.WaitingForBookArea, 0, 2f));
        }
    }

    private void HandleBookAreaReached()
    {
        if (currentState == TutorialState.WaitingForBookArea)
        {
            StartCoroutine(DelayedMessage("Press 'E' to interact with the book.", TutorialState.WaitingForBookInteraction, 0, 0));
        }
    }

    private void HandleBookInteracted()
    {
        if (currentState == TutorialState.WaitingForBookInteraction)
        {
            StartCoroutine(DelayedMessage("Press 'Tab' to open the book and 'I' to open your inventory.", TutorialState.TutorialEndState, 3f, 0.3f));
        }
    }
    private void HandleTutorialEnd()
    {
        if (currentState == TutorialState.TutorialEndState)
        {
            StartCoroutine(DelayedMessage("Proceed down the road and refer to the book for guidance!", TutorialState.Completed, 3f, 0));
        }
    }
}
