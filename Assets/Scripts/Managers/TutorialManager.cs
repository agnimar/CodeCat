using System;
using UnityEngine;
using System.Collections;
using TMPro;

public enum TutorialInputType
{
    None,
    KeyPress,
    MouseMovement,
    CharacterMovement,
    ObjectInteraction,
    Custom
}

[Serializable]
public class TutorialStep
{
    [Tooltip("The tutorial message to display.")]
    public string message;

    [Tooltip("If greater than 0, auto-advance after this duration (plus a short delay). Set to 0 to wait for input.")]
    public float duration;

    [Tooltip("If duration is 0, select the input type required to advance this step.")]
    public TutorialInputType inputType;

    [Tooltip("If using KeyPress input, which key should be pressed?")]
    public KeyCode keyToPress = KeyCode.Space;

    [Tooltip("For CharacterMovement or ObjectInteraction steps, assign the target GameObject.")]
    public GameObject targetObject;

    [Tooltip("For CharacterMovement/ObjectInteraction, the required distance to the target to complete this step.")]
    public float targetDistanceThreshold = 2f;

    [Tooltip("Delay in seconds before advancing to the next step after conditions are met.")]
    public float delayBeforeAdvance = 0f;

    [Tooltip("Lock player movement during this step?")]
    public bool lockMovement;

    [Tooltip("Lock player interaction during this step?")]
    public bool lockInteraction;
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Tutorial Steps")]
    public TutorialStep[] tutorialSteps;

    [Header("Player References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInteraction playerInteraction;

    [Header("Tutorial UI")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text tutorialText;

    private int currentStepIndex = 0;
    private Coroutine autoAdvanceCoroutine;
    private bool waitingForInput = false;
    private bool isAdvancing = false;
    private bool objectMessageDisplayed = false;
    private Coroutine hideTutorialCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple TutorialManager instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (playerController != null) playerController.enabled = false;
        if (playerInteraction != null) playerInteraction.enabled = false;

        if (tutorialSteps != null && tutorialSteps.Length > 0)
        {
            currentStepIndex = 0;
            ShowCurrentStep();
        }
        else
        {
            EndTutorial();
        }
    }

    private void ShowCurrentStep()
    {
        TutorialStep step = tutorialSteps[currentStepIndex];

        if (playerController != null)
            playerController.enabled = !step.lockMovement;
        if (playerInteraction != null)
            playerInteraction.enabled = !step.lockInteraction;

        ShowTutorialMessage(step.message, step.duration > 0 ? step.duration : 0f);

        if (step.duration > 0)
        {
            waitingForInput = false;
            if (autoAdvanceCoroutine != null)
                StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = StartCoroutine(AutoAdvance(step.duration + step.delayBeforeAdvance));
        }
        else
        {
            waitingForInput = true;
            if (step.inputType == TutorialInputType.ObjectInteraction)
            {
                if (step.targetObject != null && playerController != null)
                {
                    float dist = Vector3.Distance(playerController.transform.position, step.targetObject.transform.position);
                    if (dist <= step.targetDistanceThreshold)
                    {
                        ShowTutorialMessage(step.message, 0f);
                        objectMessageDisplayed = true;
                    }
                    else
                    {
                        HideTutorialMessage();
                        objectMessageDisplayed = false;
                    }
                }
            }
            else
            {
                ShowTutorialMessage(step.message, 0f);
            }
        }

        isAdvancing = false;
    }

    private IEnumerator AutoAdvance(float totalDelay)
    {
        yield return new WaitForSeconds(totalDelay);
        NextStep();
    }

    private void Update()
    {
        if (!waitingForInput || isAdvancing) return;

        TutorialStep step = tutorialSteps[currentStepIndex];

        switch (step.inputType)
        {
            case TutorialInputType.KeyPress:
                if (Input.GetKeyDown(step.keyToPress))
                {
                    TryAdvanceStep(step.delayBeforeAdvance);
                }
                break;
            case TutorialInputType.MouseMovement:
                if (Mathf.Abs(Input.GetAxis("Mouse X")) > 0.01f || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.01f)
                {
                    TryAdvanceStep(step.delayBeforeAdvance);
                }
                break;
            case TutorialInputType.CharacterMovement:
                if (playerController != null)
                {
                    float horizontal = Input.GetAxis("Horizontal");
                    float vertical = Input.GetAxis("Vertical");
                    if (Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f)
                    {
                        TryAdvanceStep(step.delayBeforeAdvance);
                    }
                }
                break;
            case TutorialInputType.ObjectInteraction:
                if (!objectMessageDisplayed)
                {
                    ShowTutorialMessage(step.message, 0f);
                    objectMessageDisplayed = true;
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (step.targetObject != null && playerController != null)
                    {
                        float dist = Vector3.Distance(playerController.transform.position, step.targetObject.transform.position);
                        Debug.Log("Distance to target: " + dist);
                        if (dist <= step.targetDistanceThreshold)
                        {
                            TryAdvanceStep(step.delayBeforeAdvance);
                        }
                        else
                        {
                            Debug.Log("Player not in range. Required: " + step.targetDistanceThreshold);
                        }
                    }
                }
                break;
            case TutorialInputType.None:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    TryAdvanceStep(step.delayBeforeAdvance);
                }
                break;
            case TutorialInputType.Custom:
                break;
            default:
                break;
        }
    }

    private void TryAdvanceStep(float delay)
    {
        if (isAdvancing) return;
        isAdvancing = true;
        if (delay > 0)
            StartCoroutine(DelayedNextStep(delay));
        else
            NextStep();
    }

    private IEnumerator DelayedNextStep(float delay)
    {
        yield return new WaitForSeconds(delay);
        NextStep();
    }

    public void NextStep()
    {
        waitingForInput = false;
        currentStepIndex++;
        objectMessageDisplayed = false;
        if (currentStepIndex >= tutorialSteps.Length)
        {
            EndTutorial();
        }
        else
        {
            ShowCurrentStep();
        }
    }

    private void EndTutorial()
    {
        if (playerController != null) playerController.enabled = true;
        if (playerInteraction != null) playerInteraction.enabled = true;
        HideTutorialMessage();
    }

    private void ShowTutorialMessage(string message, float duration = 0f)
    {
        if (hideTutorialCoroutine != null)
            StopCoroutine(hideTutorialCoroutine);

        tutorialText.text = message;
        tutorialPanel.SetActive(true);

        if (duration > 0f)
        {
            hideTutorialCoroutine = StartCoroutine(HideTutorialAfterDuration(duration));
        }
    }

    private IEnumerator HideTutorialAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        HideTutorialMessage();
    }

    private void HideTutorialMessage()
    {
        tutorialPanel.SetActive(false);
    }

}