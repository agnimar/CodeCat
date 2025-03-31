using UnityEngine;

public class EndSceneTrigger : MonoBehaviour
{
    public EndSceneManager endSceneManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            endSceneManager.StartEndSequence();
            endSceneManager.StartFade();

            GetComponent<Collider>().enabled = false;
        }
    }
}
