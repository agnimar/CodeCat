using UnityEngine;

public class CrystalActivation : MonoBehaviour
{
    [Header("Crystal Effects")]
    [SerializeField] private Light crystalLight;                 
    [SerializeField] private ParticleSystem activationParticles;    
    [SerializeField] private AudioSource activationSound;

    private bool isActivated = false;
    public bool IsActivated { get { return isActivated; } }

    private void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        if (crystalLight != null)
        {
            crystalLight.enabled = true;
        }
        if (activationParticles != null)
        {
            activationParticles.Play();
        }
        if (activationSound != null)
        {
            activationSound.Play();
        }
        isActivated = true;
    }

    public void Deactivate()
    {
        if (crystalLight != null)
        {
            crystalLight.enabled = false;
        }
        if (activationParticles != null)
        {
            activationParticles.Stop();
        }
        isActivated = false;
    }
}
