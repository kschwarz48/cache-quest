using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;

    public GameObject[] enemyPrefabs;
    public Transform[] enemySpawnPoints;

    public GameObject playerHUDPrefab;

    void Start()
    {
        if (PlayerController.Instance == null)
        {
            Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        }

        // Instantiate PlayerHUD and make it persistent across scenes
        if (GameObject.FindWithTag("PlayerHUD") == null)
        {
            var playerHUD = Instantiate(playerHUDPrefab);
            DontDestroyOnLoad(playerHUD);
        }

        // Instantiate enemies
        for (int i = 0; i < enemySpawnPoints.Length; i++)
        {
            int prefabIndex = Random.Range(0, enemyPrefabs.Length);
            Transform spawnPoint = enemySpawnPoints[i];
            Instantiate(enemyPrefabs[prefabIndex], spawnPoint.position, spawnPoint.rotation);
        }
    }
}
