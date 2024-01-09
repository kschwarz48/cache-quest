using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public GameObject playerPrefab; // Assign the Player prefab here in the Inspector
    public Transform spawnPoint; // Assign a Transform to dictate where the player should spawn

    void Start()
    {
        if (PlayerController.Instance == null)
        {
            GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
