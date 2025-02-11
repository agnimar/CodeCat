using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterTransitionTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    private void OnTriggerEnter(Collider other)
    {
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
            SceneManager.LoadScene(nextSceneName); 
        }
        else
        {
            Debug.LogError("Next scene name is not set in the Inspector!");
        }
    }
}
