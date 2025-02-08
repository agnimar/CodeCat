using UnityEngine;

public class CaveBarrierSound : MonoBehaviour
{
    [SerializeField] private AudioClip barrierSound;
    [SerializeField] private float maxVolume = 1.0f;
    [SerializeField] private float maxDistance = 10.0f;
    private AudioSource audioSource;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = barrierSound;
        audioSource.loop = true;
        audioSource.spatialBlend = 1.0f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = maxDistance;
        audioSource.playOnAwake = true;
        audioSource.outputAudioMixerGroup = SoundManager.instance.GetSFXMixerGroup();
        audioSource.Play();
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        float volumeFactor = Mathf.Clamp01(1 - (distance / maxDistance));
        audioSource.volume = volumeFactor * maxVolume;
    }
}
