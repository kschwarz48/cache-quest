using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public GameObject playerPrefab; // Assign the Player prefab here in the Inspector
    public Transform playerSpawnPoint; // Assign a Transform to dictate where the player should spawn

    public GameObject enemyPrefab; // Assign the Enemy prefab here in the Inspector
    public Transform enemySpawnPoint; // Assign a Transform for the enemy spawn point

    void Start()
    {
        if (PlayerController.Instance == null)
        {
            Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        }

        // Instantiate an enemy at the enemy spawn point
        Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);
    }
}
