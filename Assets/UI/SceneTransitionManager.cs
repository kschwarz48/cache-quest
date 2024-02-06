using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public Animator animator; // Make sure this is the Animator for your fadeImage
    private bool isTransitioning = false;
    private string nextSpawnPoint; // To keep track of the next spawn point name
    public static SceneTransitionManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName, string spawnPointName)
    {
        if (!isTransitioning)
        {
            nextSpawnPoint = spawnPointName; // Store the next spawn point name
            StartCoroutine(FadeAndLoadScene(sceneName));
            isTransitioning = true;
        }
    }

    IEnumerator FadeAndLoadScene(string sceneName)
    {
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1); // Wait for the fade out animation to finish
        SceneManager.LoadScene(sceneName);
        // The FadeIn and setting of the spawn point will occur in the OnSceneLoaded method
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Set the player's position to the next spawn point
        SetPlayerPosition(nextSpawnPoint);

        // Fade in, triggered after a new scene is loaded
        animator.SetTrigger("FadeIn");
        isTransitioning = false; // Transition is complete, ready for the next one
    }

    private void SetPlayerPosition(string spawnPointName)
    {
        // Wait one frame for the scene to load completely
        GameObject spawnPoint = GameObject.Find(spawnPointName);
        if (spawnPoint != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = spawnPoint.transform.position;
            }
        }
    }

    // Helper methods to fade in and out
    public void FadeOut()
    {
        animator.SetTrigger("FadeOut");
    }

    public void FadeIn()
    {
        animator.SetTrigger("FadeIn");
    }
}
