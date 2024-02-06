using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;

    public GameObject[] enemyPrefabs;
    public Transform[] enemySpawnPoints;

    public GameObject playerHUDPrefab;
    public GameObject inGameMenuPrefab; // Add a public field for the InGameMenu prefab
    private GameObject inGameMenuInstance; 

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

        // Instantiate the InGameMenu and make it persistent across scenes
        if (inGameMenuInstance == null)
        {
            inGameMenuInstance = Instantiate(inGameMenuPrefab);
            DontDestroyOnLoad(inGameMenuInstance);
            inGameMenuInstance.SetActive(true); // Start inactive
        }

        // Instantiate enemies
        for (int i = 0; i < enemySpawnPoints.Length; i++)
        {
            int prefabIndex = Random.Range(0, enemyPrefabs.Length);
            Transform spawnPoint = enemySpawnPoints[i];
            Instantiate(enemyPrefabs[prefabIndex], spawnPoint.position, spawnPoint.rotation);
        }
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Escape))
    //     {
    //         if (inGameMenuInstance != null)
    //         {
    //             bool isActive = inGameMenuInstance.activeSelf;
    //             Debug.Log("ESC key pressed");
    //             inGameMenuInstance.SetActive(!isActive);
    //             Time.timeScale = !isActive ? 0 : 1; // Pause game if menu is activated
    //         }
    //     }
    // }
}
