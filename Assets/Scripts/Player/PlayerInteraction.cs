using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private GameObject currentInteractableObject = null;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentInteractableObject != null)
        {
            var interactable = currentInteractableObject.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }

    public void SetCurrentInteractable(GameObject interactable)
    {
        currentInteractableObject = interactable;
    }

    public void ClearCurrentInteractable(GameObject interactable)
    {
        if (currentInteractableObject == interactable)
        {
            currentInteractableObject = null;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<IInteractable>() != null)
        {
            currentInteractableObject = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == currentInteractableObject)
        {
            currentInteractableObject = null;
        }
    }
}
