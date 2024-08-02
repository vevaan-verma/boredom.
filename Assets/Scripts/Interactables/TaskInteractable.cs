using System.Collections.Generic;
using UnityEngine;

public abstract class TaskInteractable : Interactable {

    [Header("Tasks")]
    [SerializeField] protected List<Task> tasks;

    public override void Interact() {

        taskManager.StartTask();

    }

    public Task GetRandomTask() {

        return tasks[Random.Range(0, tasks.Count)];

    }
}