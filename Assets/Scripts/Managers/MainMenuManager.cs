using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;

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

    private Resolution[] resolutions;
    private const string MASTER_VOL_PARAM = "MasterVolume";
    private const string MUSIC_VOL_PARAM = "MusicVolume";
    private const string SFX_VOL_PARAM = "SFXVolume";

    private const string MASTER_VOL_PREF = "MasterVol";
    private const string MUSIC_VOL_PREF = "MusicVol";
    private const string SFX_VOL_PREF = "SFXVol";
    private const string RESOLUTION_PREF = "ResolutionIndex";
    private const string FULLSCREEN_PREF = "Fullscreen";
    private const string VSYNC_PREF = "VSync";

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
        float savedSensitivity = PlayerPrefs.GetFloat(MOUSE_SENSITIVITY_PREF, 100f);
        mouseSensitivitySlider.value = savedSensitivity;
        mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
        ApplyMouseSensitivity(savedSensitivity);
    }

    private void SetMouseSensitivity(float value)
    {
        PlayerPrefs.SetFloat(MOUSE_SENSITIVITY_PREF, value);
        ApplyMouseSensitivity(value);
    }

    private void ApplyMouseSensitivity(float value)
    {
        if (Camera.main != null && Camera.main.TryGetComponent(out CameraController camCtrl))
        {
            camCtrl.SetMouseSensitivity(value * 100);
        }
    }

    private void SetupAudio()
    {
        if (masterSlider == null || musicSlider == null || sfxSlider == null) return;
        float savedMasterVol = PlayerPrefs.GetFloat(MASTER_VOL_PREF, 1f);
        float savedMusicVol = PlayerPrefs.GetFloat(MUSIC_VOL_PREF, 1f);
        float savedSfxVol = PlayerPrefs.GetFloat(SFX_VOL_PREF, 1f);

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
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_PREF, 0);

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        fullscreenToggle.isOn = PlayerPrefs.GetInt(FULLSCREEN_PREF, 1) == 1;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
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
        if (resolutions == null) return;
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt(RESOLUTION_PREF, index);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(FULLSCREEN_PREF, isFullscreen ? 1 : 0);
    }

    public void SetVSync(bool isEnabled)
    {
        QualitySettings.vSyncCount = isEnabled ? 1 : 0;
        PlayerPrefs.SetInt(VSYNC_PREF, isEnabled ? 1 : 0);
    }

    public void NewGame()
    {
        Debug.Log("New Game button clicked");
        SceneManager.LoadScene("Chapter_1");
    }

    public void ShowControls()
    {
        Debug.Log("Controls button clicked");
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Controls panel is not assigned in the Inspector!");
        }
    }

    public void ShowSettings()
    {
        Debug.Log("Settings button clicked");
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Settings panel is not assigned in the Inspector!");
        }
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked");
        Application.Quit();
    }

    public void BackToMainMenuFromControls()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(false);
    }

    public void BackToMainMenuFromSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
}