using UnityEngine;
using System.Collections;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource musicSource; // Assign this in the Inspector
    public AudioSource sfxSource;   // Assign this in the Inspector

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ChangeMusic(AudioClip newMusic)
    {
        Debug.Log("Changing music to: " + newMusic.name);
        if (musicSource.clip != newMusic)
        {
            musicSource.Stop();
            musicSource.clip = newMusic;
            musicSource.Play();
        }
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        Debug.Log("Attempting to play sound effect: " + clip.name);
        sfxSource.PlayOneShot(clip);
        StartCoroutine(CheckIfSoundPlayed(clip.length));
    }


    private IEnumerator CheckIfSoundPlayed(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (!sfxSource.isPlaying)
        {
            Debug.Log("Sound effect ended or was stopped");
        }
        else
        {
            Debug.Log("Sound effect still playing");
        }
    }


    // Method to stop the music if needed
    public void StopMusic()
    {
        musicSource.Stop();
    }
}
