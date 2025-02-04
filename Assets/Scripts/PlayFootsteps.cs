using UnityEngine;

public class PlayFootsteps : MonoBehaviour
{
    [Header("Pitch Randomization Settings")]
    [Tooltip("Minimum pitch for the footstep sound.")]
    [SerializeField] private float minPitch = 0.8f;

    [Tooltip("Maximum pitch for the footstep sound.")]
    [SerializeField] private float maxPitch = 1.2f;

    [Tooltip("Volume for the footstep sound.")]
    [SerializeField] private float volume = 1f;

    public void PlaySound()
    {
        if (SoundManager.instance == null)
        {
            Debug.LogError("SoundManager instance not found!");
            return;
        }

        AudioClip[] clips = SoundManager.instance.GetSoundClips(SoundType.FOOTSTEP);
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning("No footstep clips found in SoundManager!");
            return;
        }

        AudioClip randomClip = clips[Random.Range(0, clips.Length)];

        float randomPitch = Random.Range(minPitch, maxPitch);

        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();

        tempSource.clip = randomClip;
        tempSource.volume = volume;
        tempSource.pitch = randomPitch;

        // Optionally, set other AudioSource properties if needed.
        // tempSource.spatialBlend = 1.0f;

        tempSource.Play();
        Destroy(tempAudio, randomClip.length / randomPitch);
    }
}
