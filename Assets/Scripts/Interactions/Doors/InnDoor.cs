using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractable
{
    public string sceneToLoad = "Inn";

    public void Interact()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
