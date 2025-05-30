using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundOptionsUI : MonoBehaviour
{
    [Header("Audio Mixer")]
    [Tooltip("Assign your AudioMixer here.")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sliders")]
    [Tooltip("Slider that controls Master Volume (0 - 1).")]
    [SerializeField] private Slider masterSlider;

    [Tooltip("Slider that controls Music Volume (0 - 1).")]
    [SerializeField] private Slider musicSlider;

    [Tooltip("Slider that controls SFX Volume (0 - 1).")]
    [SerializeField] private Slider sfxSlider;

    private const string MASTER_VOL_PARAM = "MasterVolume";
    private const string MUSIC_VOL_PARAM = "MusicVolume";
    private const string SFX_VOL_PARAM = "SFXVolume";

    private const string MASTER_VOL_PREF = "MasterVol";
    private const string MUSIC_VOL_PREF = "MusicVol";
    private const string SFX_VOL_PREF = "SFXVol";

    private void Start()
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
}
