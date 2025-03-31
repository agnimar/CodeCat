using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Added for scene loading
using System.Collections;

public class EndSceneManager : MonoBehaviour
{
    [Header("Timeline")]
    public PlayableDirector endTimeline;

    [Header("UI Fade")]
    public Image fadePanel;
    public float fadeDuration = 2f;

    [Header("Player and Camera Control")]
    public MonoBehaviour playerController;
    public CameraController cameraController;

    [Header("Cinematic Camera Position")]
    public Transform endCameraPosition;
    public Camera mainCamera;

    [Header("Scene Loading")]
    public string sceneToLoad;

    private void Start()
    {
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 0f;
            fadePanel.color = c;
            fadePanel.gameObject.SetActive(true);
        }

        if (endTimeline != null)
        {
            endTimeline.stopped += OnTimelineFinished;
        }
    }

    public void StartEndSequence()
    {
        LockPlayerControl();
        ForceThirdPersonCamera();
        LockAndPositionCamera();

        if (endTimeline != null)
        {
            endTimeline.Play();
        }
    }

    private void LockPlayerControl()
    {
        if (playerController != null)
            playerController.enabled = false;
    }

    private void ForceThirdPersonCamera()
    {
        if (cameraController != null)
        {
            cameraController.SetFirstPersonExternally(false);
            cameraController.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void LockAndPositionCamera()
    {
        if (mainCamera != null && endCameraPosition != null)
        {
            mainCamera.transform.position = endCameraPosition.position;
            mainCamera.transform.rotation = endCameraPosition.rotation;
        }
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        Debug.Log("Timeline finished. Starting UI fade.");
        StartCoroutine(FadeInPanel());
    }

    private IEnumerator FadeInPanel()
    {
        if (fadePanel == null)
        {
            Debug.LogError("fadePanel is null!");
            yield break;
        }

        float elapsed = 0f;
        Color startColor = fadePanel.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            fadePanel.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        fadePanel.color = targetColor;

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No scene specified to load. Please assign a scene name in the inspector.");
        }
    }

    public void StartFade()
    {
        StartCoroutine(FadeInPanel());
    }
}
