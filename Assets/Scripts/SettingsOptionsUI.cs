using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SettingsOptionsUI : MonoBehaviour
{
    public static SettingsOptionsUI Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Graphics Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Settings Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject controlsPanel;

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    private bool isSettingsOpen = false;
    private bool isPaused = false;

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

    public bool IsAnyMenuOpen => settingsPanel.activeSelf || pauseMenuPanel.activeSelf || controlsPanel.activeSelf;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        SetupAudio();
        SetupGraphics();
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);

        resumeButton.onClick.AddListener(ResumeGame);
        controlsButton.onClick.AddListener(OpenControls);
        optionsButton.onClick.AddListener(OpenSettings);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        quitButton.onClick.AddListener(QuitGame);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel.activeSelf || controlsPanel.activeSelf)
            {
                ClosePanels();
            }
            else
            {
                TogglePauseMenu();
            }
        }
    }
    private void SetupAudio()
    {
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
        float dB = (value <= 0.0001f) ? -80f : Mathf.Log10(value) * 20f;
        audioMixer.SetFloat(MASTER_VOL_PARAM, dB);
        PlayerPrefs.SetFloat(MASTER_VOL_PREF, value);
    }

    public void SetMusicVolume(float value)
    {
        float dB = (value <= 0.0001f) ? -80f : Mathf.Log10(value) * 20f;
        audioMixer.SetFloat(MUSIC_VOL_PARAM, dB);
        PlayerPrefs.SetFloat(MUSIC_VOL_PREF, value);
    }

    public void SetSFXVolume(float value)
    {
        float dB = (value <= 0.0001f) ? -80f : Mathf.Log10(value) * 20f;
        audioMixer.SetFloat(SFX_VOL_PARAM, dB);
        PlayerPrefs.SetFloat(SFX_VOL_PREF, value);
    }

    public void SetResolution(int index)
    {
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
    public bool IsPaused => isPaused;
    private void ToggleSettingsMenu()
    {
        isSettingsOpen = !isSettingsOpen;
        settingsPanel.SetActive(isSettingsOpen);
        Cursor.lockState = isSettingsOpen ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = isSettingsOpen;
    }
    private void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pauseMenuPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
        Cursor.lockState = isPaused ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }
    private void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OpenSettings()
    {
        pauseMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ClosePanels()
    {
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }
    private void OpenControls()
    {
        settingsPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }
    private void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    private void QuitGame()
    {
        Application.Quit();
    }

}
