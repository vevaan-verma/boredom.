using UnityEngine;

public abstract class Interactable : MonoBehaviour {

    [Header("References")]
    protected TaskManager taskManager;
    protected bool isInteractable;

    protected void Awake() {

        taskManager = FindObjectOfType<TaskManager>();
        isInteractable = true;

    }

    public abstract void Interact();

    public void SetInteractable(bool isInteractable) { this.isInteractable = isInteractable; }

    public bool IsInteractable() { return isInteractable; }

}
