using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;

public class UIPanelActionLoadTimer : MonoBehaviour
{
    [Header("UI Panels to Measure")]
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;
    public GameObject controlsPanel;

    [Header("UI Buttons (natural openings)")]
    public Button optionsButton; 
    public Button controlsButton;

    [Header("Pause Key")]
    public KeyCode pauseKey = KeyCode.Escape;

    [Header("Log File")]
    public string logFileName = "UIPanelLoadTimes.csv";

    private string _logFilePath;

    private void Awake()
    {
        _logFilePath = Path.Combine(Application.persistentDataPath, logFileName);
        Debug.Log($"[UIPanelTimer] Log file path: {_logFilePath}");

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath));
        }
        catch (Exception e)
        {
            Debug.LogError($"[UIPanelTimer] Couldn't create directory for log file: {e}");
        }

        if (!File.Exists(_logFilePath))
        {
            try
            {
                File.WriteAllText(_logFilePath,
                    "Timestamp,PanelName,LoadTimeMs" + Environment.NewLine
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"[UIPanelTimer] Couldn't create log file at {_logFilePath}: {e}");
            }
        }
    }
    private void Start()
    {
        if (optionsButton != null)
            optionsButton.onClick.AddListener(() =>
                StartCoroutine(MeasureShowTime(settingsPanel, "SettingsPanel"))
            );

        if (controlsButton != null)
            controlsButton.onClick.AddListener(() =>
                StartCoroutine(MeasureShowTime(controlsPanel, "ControlsPanel"))
            );
    }
    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if ((settingsPanel != null && settingsPanel.activeSelf) ||
                (controlsPanel != null && controlsPanel.activeSelf))
            {
                StartCoroutine(MeasureShowTime(pauseMenuPanel, "PauseMenuPanel"));
            }
            else if (pauseMenuPanel != null && !pauseMenuPanel.activeSelf)
            {
                StartCoroutine(MeasureShowTime(pauseMenuPanel, "PauseMenuPanel"));
            }
        }
    }
    private IEnumerator MeasureShowTime(GameObject panel, string panelName)
    {
        float startTime = Time.realtimeSinceStartup;

        yield return new WaitUntil(() => panel.activeSelf);
        yield return new WaitForEndOfFrame();

        float endTime = Time.realtimeSinceStartup;
        float deltaMs = (endTime - startTime) * 1000f;

        Debug.Log($"[UIPanelTimer] {panelName} load time: {deltaMs:F2} ms");
        LogToFile(panelName, deltaMs);
    }
    private void LogToFile(string panelName, float deltaMs)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string line = $"{timestamp},{panelName},{deltaMs:F2}{Environment.NewLine}";

        try
        {
            File.AppendAllText(_logFilePath, line);
        }
        catch (Exception e)
        {
            Debug.LogError($"[UIPanelTimer] Couldn't append to {_logFilePath}: {e}");
        }
    }
}
