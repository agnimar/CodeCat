using System;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundType
{
    FOOTSTEP,
    INTERACT,
    MOEW,
    PICK_UP_OBJECT,
    PICK_UP_BOOK,
    DROP,
    PLACE_ON_PILLAR,
    PUZZLE_SOLVED,
    OPEN_UI,
    BACKGROUND_MUSIC, 
    SUCCESS
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [Header("Sound List")]
    [SerializeField] private SoundList[] soundList;

    [Header("Audio Mixer Groups")]
    [SerializeField] private AudioMixerGroup sfxAudioMixerGroup;
    [SerializeField] private AudioMixerGroup bgmAudioMixerGroup;

    public static SoundManager instance { get; private set; }
    private AudioSource sfxSource;
    [SerializeField] private AudioSource bgmSource;

    private void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        sfxSource = GetComponent<AudioSource>();

        if (sfxAudioMixerGroup != null)
        {
            sfxSource.outputAudioMixerGroup = sfxAudioMixerGroup;
        }

        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true; 
        }
        if (bgmAudioMixerGroup != null)
        {
            bgmSource.outputAudioMixerGroup = bgmAudioMixerGroup;
        }
    }
    public static void PlaySound(SoundType sound, float volume = 1f)
    {
        SoundList list = instance.soundList[(int)sound];
        AudioClip[] clips = list.Sounds;

        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        float pitch = list.pitch <= 0f ? 1f : list.pitch;

        GameObject tempGO = new GameObject("TempAudio");
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();

        tempSource.clip = clip;
        tempSource.pitch = pitch;
        tempSource.volume = volume;
        tempSource.outputAudioMixerGroup = instance.sfxAudioMixerGroup;
        tempSource.Play();

        UnityEngine.Object.Destroy(tempGO, clip.length / pitch);

    }


    public static void PlayBackgroundMusic(SoundType sound, float volume = 1)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[0];

        instance.bgmSource.clip = clip;
        instance.bgmSource.volume = volume;
        instance.bgmSource.loop = true;
        instance.bgmSource.Play();
    }
    public static void StopBackgroundMusic()
    {
        instance.bgmSource.Stop();
    }
#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }
#endif
    public AudioClip[] GetSoundClips(SoundType sound)
    {
        if (soundList == null || soundList.Length <= (int)sound)
        {
            Debug.LogError("Sound list not properly configured for " + sound);
            return null;
        }
        return soundList[(int)sound].Sounds;
    }
    public AudioMixerGroup GetSFXMixerGroup()
    {
        return sfxAudioMixerGroup;
    }
    public static float GetSpeedMultiplier(SoundType sound)
    {
        if (instance == null || instance.soundList == null || instance.soundList.Length <= (int)sound)
            return 1f;

        float speed = instance.soundList[(int)sound].speedMultiplier;
        return speed <= 0f ? 1f : speed;
    }

}

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds {get => sounds;}
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
    public float pitch;
    public float speedMultiplier;
}