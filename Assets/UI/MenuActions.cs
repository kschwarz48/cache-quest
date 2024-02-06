using UnityEngine;
using System.Collections;

public class MenuActions : MonoBehaviour
{
    public string sceneToLoad;
    public AudioClip buttonPressSound;
    public SceneSettings settings; // Assuming you have a class that manages scene-specific settings

    public string spawnPointNameInNewScene;
    public AudioSource audioSource; // Assign in the inspector

    // Method to start the game or load a specific scene
    public void StartGame()
    {
        // Play button press sound if available
        if (buttonPressSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonPressSound);
            // Wait for the sound to finish before continuing
            StartCoroutine(DelayedSceneLoad(buttonPressSound.length));
        }
        else
        {
            // If no sound, load immediately
            LoadSceneDirectly();
        }
    }

    private IEnumerator DelayedSceneLoad(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadSceneDirectly();
    }

    private void LoadSceneDirectly()
    {
        if (SceneTransitionManager.Instance != null)
        {
            // Apply any scene settings if needed
            ApplySceneSettings();
            // Use the SceneTransitionManager to load the scene
            SceneTransitionManager.Instance.LoadScene(sceneToLoad, spawnPointNameInNewScene);
        }
        else
        {
            Debug.LogError("SceneTransitionManager instance not found.");
        }
    }

    private void ApplySceneSettings()
    {
        if (settings != null)
        {
            // Apply settings such as music, UI configurations, etc.
            // This might involve calling methods on the AudioManager or other managers
            AudioManager.Instance.ChangeMusic(settings.sceneMusic);
        }
    }

    // Optionally, add more methods here for other menu actions like quitting the game
    public void QuitGame()
    {
        // Perform any necessary cleanup
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
