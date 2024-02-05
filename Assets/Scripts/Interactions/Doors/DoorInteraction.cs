using UnityEngine;
using System.Collections;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    public string sceneToLoad;
    public AudioClip doorSound;
    public SceneSettings settings;

    public string spawnPointNameInNewScene;
    public Texture2D cursorTexture;
    public bool isAutomaticExit = false; // Set this to true for exits in the Inspector

    private bool isPlayerClose = false;
    private bool isPlayerInSafeZone = false;

    private bool hasInteracted = false;
    private float interactionCooldown = 0.5f;
    private float lastInteractionTime = -1f;

    void OnMouseEnter()
    {
        if (cursorTexture != null)
        {
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void Update()
    {
        if (isPlayerClose && Time.time >= lastInteractionTime + interactionCooldown)
        {
            if ((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(1)) && !hasInteracted)
            {
                Interact();
                hasInteracted = true; // Prevent further interaction until reset
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerClose = true;
            if (isAutomaticExit && !isPlayerInSafeZone && !hasInteracted)
            {
                PlayDoorSound();
                AudioManager.Instance.ChangeMusic(settings.sceneMusic); 
                StartCoroutine(DelayedSceneLoad(doorSound.length));
                hasInteracted = true; // Prevent further automatic interaction
            }
            else
            {
                other.GetComponent<PlayerInteraction>().SetCurrentInteractable(gameObject);
            }
        }
    }
    public void Interact()
    {
        Debug.Log("Interact called for door leading to " + sceneToLoad);
        PlayDoorSound();
        AudioManager.Instance.ChangeMusic(settings.sceneMusic);
        // Start loading the scene after the door sound has had time to play
        StartCoroutine(DelayedSceneLoad(doorSound != null ? doorSound.length : 0));
    }


    private IEnumerator DelayedSceneLoad(float delay)
    {
        yield return new WaitForSeconds(delay); // Delay to let the sound play before loading the scene
        SceneTransitionManager.Instance.LoadScene(sceneToLoad, spawnPointNameInNewScene);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerClose = false;
            hasInteracted = false; // Reset on exit to allow for re-entry interactions
            if (!isAutomaticExit)
            {
                other.GetComponent<PlayerInteraction>().ClearCurrentInteractable(gameObject);
            }
        }
    }

    private void PlayDoorSound()
    {
        if (doorSound != null)
        {
            GameObject soundObject = new GameObject("TempAudio");
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.clip = doorSound;
            audioSource.Play();

            DontDestroyOnLoad(soundObject);
            Destroy(soundObject, doorSound.length); // Destroy the object after the clip finishes playing
        }
        else
        {
            Debug.LogWarning("Door sound is null for the door leading to " + sceneToLoad);
        }
    }



    // Method to be called by the SafeZone script
    public void SetSafeZoneStatus(bool status)
    {
        isPlayerInSafeZone = status;
    }
}
