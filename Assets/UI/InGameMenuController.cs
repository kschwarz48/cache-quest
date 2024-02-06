using UnityEngine;

public class InGameMenuController : MonoBehaviour
{
    public GameObject menuPanel; // Ensure this is linked to the panel in your prefab

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the menu is not visible when the game starts
        menuPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle menu on ESC key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        bool isMenuActive = menuPanel.activeSelf;

        // Toggle the visibility
        menuPanel.SetActive(!isMenuActive);

        // Pause or resume the game based on the menu state
        Time.timeScale = isMenuActive ? 1 : 0;
        PlayerController.isGamePaused = !isMenuActive;
    }

    // Add methods for each of your menu buttons here
    // For example, a method to quit the game could look like this:
    public void QuitGame()
    {
        Application.Quit();
    }
}
