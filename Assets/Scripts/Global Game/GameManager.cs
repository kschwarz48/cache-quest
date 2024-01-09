using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private Vector3 nextPlayerPosition;
    private bool shouldSetPlayerPosition = false;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void LoadScene(string sceneName, string spawnPointName)
    {
        StartCoroutine(LoadSceneWithSpawnPoint(sceneName, spawnPointName));
    }

    private IEnumerator LoadSceneWithSpawnPoint(string sceneName, string spawnPointName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName);
        GameObject spawnPoint = GameObject.Find(spawnPointName);
        if (spawnPoint)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = spawnPoint.transform.position;
        }
        else
        {
            Debug.LogError("Spawn point not found: " + spawnPointName);
        }
    }



    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (shouldSetPlayerPosition)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Debug.Log("Setting player position to: " + nextPlayerPosition);
                player.transform.position = nextPlayerPosition;
            }
            shouldSetPlayerPosition = false;
        }
    }



}
