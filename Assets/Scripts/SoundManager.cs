using System;
using UnityEngine;

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
    BACKGROUND_MUSIC
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;
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
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true; 
        }
    }
    public static void PlaySound(SoundType sound, float volume = 1)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.sfxSource.PlayOneShot(randomClip, volume);
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
}

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds {get => sounds;}
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}