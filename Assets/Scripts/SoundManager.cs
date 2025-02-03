using UnityEngine;

public enum SoundType
{
    FOOTSTEP,
    INTERACT,
    MOEW,
    PICK_UP,
    PLACE_ON_PILLAR
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;
    public static SoundManager instance { get; private set; }
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public static void PlaySound(SoundType sound, float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.audioClips[(int)sound]);

    }
}

