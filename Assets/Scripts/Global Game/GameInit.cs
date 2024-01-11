using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;

    public GameObject[] enemyPrefabs; // Array of different enemy prefabs
    public Transform[] enemySpawnPoints; // Array of enemy spawn points

    void Start()
    {
        if (PlayerController.Instance == null)
        {
            Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        }

        for (int i = 0; i < enemySpawnPoints.Length; i++)
        {
            int prefabIndex = Random.Range(0, enemyPrefabs.Length); // Randomly select an enemy prefab
            Transform spawnPoint = enemySpawnPoints[i];
            Instantiate(enemyPrefabs[prefabIndex], spawnPoint.position, spawnPoint.rotation);
        }
    }
}
