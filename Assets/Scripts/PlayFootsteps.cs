using UnityEngine;

public class PlayFootsteps : MonoBehaviour
{
    public void PlaySound()
    {
        SoundManager.PlaySound(SoundType.FOOTSTEP, 0.15f);
    }
}
