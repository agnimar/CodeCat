using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Audio Mixer (Settings)")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Sliders (Settings)")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Graphics Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Controls Settings")]
    [SerializeField] private Slider mouseSensitivitySlider;
    private const string MOUSE_SENSITIVITY_PREF = "MouseSensitivity";

    private const float MOUSE_SLIDER_MIN = 100f;
    private const float MOUSE_SLIDER_MAX = 250f;
    private const float MOUSE_MULTIPLIER_MIN = 0.1f;
    private const float MOUSE_MULTIPLIER_MAX = 2.0f;
    private const float MOUSE_SLIDER_DEFAULT = 100f;

    private Resolution[] allResolutions; 
    private List<string> uniqueResolutionOptions; 

    private const string MASTER_VOL_PARAM = "MasterVolume";
    private const string MUSIC_VOL_PARAM = "MusicVolume";
    private const string SFX_VOL_PARAM = "SFXVolume";

    private const string MASTER_VOL_PREF = "MasterVol";
    private const string MUSIC_VOL_PREF = "MusicVol";
    private const string SFX_VOL_PREF = "SFXVol";
    private const string RESOLUTION_PREF = "ResolutionIndex";
    private const string FULLSCREEN_PREF = "Fullscreen";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple MainMenuUIManager instances detected! Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        if (controlsPanel != null)
            controlsPanel.SetActive(false);
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        SetupSettings();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SetupSettings()
    {
        if (settingsPanel == null) return;
        SetupAudio();
        SetupGraphics();
        SetupMouseSensitivity();
    }

    private void SetupMouseSensitivity()
    {
        if (mouseSensitivitySlider == null) return;

        mouseSensitivitySlider.minValue = MOUSE_SLIDER_MIN;
        mouseSensitivitySlider.maxValue = MOUSE_SLIDER_MAX;
        mouseSensitivitySlider.wholeNumbers = false;

        float savedSliderValue = PlayerPrefs.GetFloat(MOUSE_SENSITIVITY_PREF, MOUSE_SLIDER_DEFAULT);
        mouseSensitivitySlider.value = savedSliderValue;

        mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivityFromSlider);
    }

    private void SetMouseSensitivityFromSlider(float sliderValue)
    {
        PlayerPrefs.SetFloat(MOUSE_SENSITIVITY_PREF, sliderValue);
    }

    private void ApplyMouseSensitivity(float value)
    {
    }

    private void SetupAudio()
    {
        if (masterSlider == null || musicSlider == null || sfxSlider == null || audioMixer == null) return;
        float savedMasterVol = PlayerPrefs.GetFloat(MASTER_VOL_PREF, 0.8f); 
        float savedMusicVol = PlayerPrefs.GetFloat(MUSIC_VOL_PREF, 0.6f); 
        float savedSfxVol = PlayerPrefs.GetFloat(SFX_VOL_PREF, 0.8f); 

        masterSlider.value = savedMasterVol;
        musicSlider.value = savedMusicVol;
        sfxSlider.value = savedSfxVol;

        SetMasterVolume(savedMasterVol);
        SetMusicVolume(savedMusicVol);
        SetSFXVolume(savedSfxVol);

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void SetupGraphics()
    {
        if (resolutionDropdown == null || fullscreenToggle == null) return;

        allResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        uniqueResolutionOptions = allResolutions
            .Select(res => res.width + "x" + res.height)
            .Distinct()
            .ToList();

        resolutionDropdown.AddOptions(uniqueResolutionOptions);

        int savedIndex = PlayerPrefs.GetInt(RESOLUTION_PREF, -1);
        string currentScreenResString = Screen.currentResolution.width + "x" + Screen.currentResolution.height;
        int currentScreenIndexInOptions = uniqueResolutionOptions.FindIndex(option => option == currentScreenResString);

        int targetIndex = 0; 

        if (savedIndex != -1 && savedIndex < uniqueResolutionOptions.Count)
        {
            string savedResString = uniqueResolutionOptions[savedIndex];
            if (savedResString == currentScreenResString)
            {
                targetIndex = savedIndex;
            }
            else if (currentScreenIndexInOptions != -1)
            {
                targetIndex = currentScreenIndexInOptions;
                PlayerPrefs.SetInt(RESOLUTION_PREF, targetIndex);
            }
            else
            {
                targetIndex = savedIndex;
            }

        }
        else if (currentScreenIndexInOptions != -1)
        {
            targetIndex = currentScreenIndexInOptions;
            PlayerPrefs.SetInt(RESOLUTION_PREF, targetIndex); 
        }
        else
        {
            Debug.LogWarning($"Current resolution {currentScreenResString} not found in available options. Defaulting.");
            targetIndex = 0;
            PlayerPrefs.SetInt(RESOLUTION_PREF, targetIndex); 
        }


        resolutionDropdown.value = targetIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        fullscreenToggle.isOn = PlayerPrefs.GetInt(FULLSCREEN_PREF, 1) == 1;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        SetFullscreen(fullscreenToggle.isOn);
    }

    public void SetMasterVolume(float value)
    {
        if (audioMixer == null) return;
        float dB = (value <= 0.0001f) ? -80f : Mathf.Log10(value) * 20f;
        audioMixer.SetFloat(MASTER_VOL_PARAM, dB);
        PlayerPrefs.SetFloat(MASTER_VOL_PREF, value);
    }

    public void SetMusicVolume(float value)
    {
        if (audioMixer == null) return;
        float dB = (value <= 0.0001f) ? -80f : Mathf.Log10(value) * 20f;
        audioMixer.SetFloat(MUSIC_VOL_PARAM, dB);
        PlayerPrefs.SetFloat(MUSIC_VOL_PREF, value);
    }

    public void SetSFXVolume(float value)
    {
        if (audioMixer == null) return;
        float dB = (value <= 0.0001f) ? -80f : Mathf.Log10(value) * 20f;
        audioMixer.SetFloat(SFX_VOL_PARAM, dB);
        PlayerPrefs.SetFloat(SFX_VOL_PREF, value);
    }

    public void SetResolution(int index)
    {
        if (allResolutions == null || uniqueResolutionOptions == null || index < 0 || index >= uniqueResolutionOptions.Count)
        {
            Debug.LogError($"Invalid resolution index: {index}");
            return;
        }

        string resolutionText = uniqueResolutionOptions[index];
        string[] split = resolutionText.Split('x');

        if (split.Length == 2 && int.TryParse(split[0], out int width) && int.TryParse(split[1], out int height))
        {
            Resolution targetResolution = allResolutions
                                            .Where(r => r.width == width && r.height == height)
                                            .OrderByDescending(r => r.refreshRateRatio.numerator / (double)r.refreshRateRatio.denominator)
                                            .FirstOrDefault();

            if (targetResolution.width > 0) 
            {
                Debug.Log($"Setting resolution to: {targetResolution.width}x{targetResolution.height} @ {targetResolution.refreshRateRatio}");
                Screen.SetResolution(targetResolution.width, targetResolution.height, Screen.fullScreenMode, targetResolution.refreshRateRatio);
                PlayerPrefs.SetInt(RESOLUTION_PREF, index); 
            }
            else
            {
                Debug.LogError($"Could not find a matching Resolution struct for {width}x{height}. Applying without refresh rate.");
                Screen.SetResolution(width, height, Screen.fullScreenMode);
                PlayerPrefs.SetInt(RESOLUTION_PREF, index);
            }
        }
        else
        {
            Debug.LogError($"Failed to parse resolution string: {resolutionText}");
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        PlayerPrefs.SetInt(FULLSCREEN_PREF, isFullscreen ? 1 : 0);
    }

    public void NewGame()
    {
        Debug.Log("New Game button clicked");
        SceneManager.LoadScene("Chapter_1"); 
    }

    public void ShowControls()
    {
        Debug.Log("Controls button clicked");
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(true);
        else Debug.LogWarning("Controls panel is not assigned in the Inspector!");
    }

    public void ShowSettings()
    {
        Debug.Log("Settings button clicked");
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
        else Debug.LogWarning("Settings panel is not assigned in the Inspector!");
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void BackToMainMenuFromControls()
    {
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void BackToMainMenuFromSettings()
    {
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }
}