using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class PickupActionLoadTimer : MonoBehaviour
{
    [Header("Interactable Settings")]
    [Tooltip("Only measure objects on these layers")]
    public LayerMask interactableLayer;

    [Header("Log File Settings")]
    [Tooltip("Written to Application.persistentDataPath")]
    public string logFileName = "PickupLoadTimes.csv";

    private string _logFilePath;

    private void Awake()
    {
        _logFilePath = Path.Combine(Application.persistentDataPath, logFileName);
        Debug.Log($"[PickupTimer] Logging to: {_logFilePath}");

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath));
        }
        catch (Exception e)
        {
            Debug.LogError($"[PickupTimer] Failed to create directory: {e.Message}");
        }

        if (!File.Exists(_logFilePath))
        {
            try
            {
                File.WriteAllText(_logFilePath, "Timestamp,ItemName,PickupTimeMs\n");
            }
            catch (Exception e)
            {
                Debug.LogError($"[PickupTimer] Failed to create log file: {e.Message}");
            }
        }
    }

    private void Start()
    {
        foreach (var interactable in FindObjectsOfType<InteractableBase>())
        {
            if (((1 << interactable.gameObject.layer) & interactableLayer.value) != 0)
            {
                interactable.onInteractionStarted.AddListener(_ =>
                    StartCoroutine(MeasurePickupTime(interactable))
                );
            }
        }
    }

    private IEnumerator MeasurePickupTime(InteractableBase interactable)
    {
        float startTime = Time.realtimeSinceStartup;

        yield return new WaitUntil(() => !interactable.gameObject.activeSelf);
        yield return new WaitForEndOfFrame();

        float elapsedMs = (Time.realtimeSinceStartup - startTime) * 1000f;
        Debug.Log($"[PickupTimer] {interactable.ItemName} picked up in {elapsedMs:F2} ms");

        LogToFile(interactable.ItemName, elapsedMs);
    }

    private void LogToFile(string itemName, float elapsedMs)
    {
        string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff},{itemName},{elapsedMs:F2}\n";
        try
        {
            File.AppendAllText(_logFilePath, line);
        }
        catch (Exception e)
        {
            Debug.LogError($"[PickupTimer] Failed to append log: {e.Message}");
        }
    }
}
