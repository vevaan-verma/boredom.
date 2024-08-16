using UnityEngine;

[CreateAssetMenu(menuName = "Tasks/Sandwich")]
public class Sandwich : Task {
    // Start is called before the first frame update
    public override void OnTaskComplete() {

        Debug.Log("Made a sandwich");

    }
}
