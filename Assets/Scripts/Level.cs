using System;
using UnityEngine;

[CreateAssetMenu]
public class Level : ScriptableObject {

    [Tooltip("Level 1 begins at index 1.")] public int levelNum;
    public int tasksToComplete;
    [Tooltip("In seconds")] public int timeLimit;
    public float boredomDecayRate;

    public int GetTasksToComplete() => tasksToComplete;

    public int GetTimeLimit() => timeLimit;

    public float GetBoredomDecayRate() => boredomDecayRate;

}
