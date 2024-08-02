public class SandwichIngredient : Interactable {
    private TaskManager tm;
    // Start is called before the first frame update
    void Start() {
        tm = FindObjectOfType<TaskManager>().GetComponent<TaskManager>();
    }

    public override void Interact() {
        tm.OnIngredientPickup();
    }
}
