using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene transitions

public class ChapterTransitionTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName; // The name of the next scene to load

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger. Loading next chapter...");
            LoadNextChapter();
        }
    }

    private void LoadNextChapter()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName); // Load the specified scene
        }
        else
        {
            Debug.LogError("Next scene name is not set in the Inspector!");
        }
    }
}
