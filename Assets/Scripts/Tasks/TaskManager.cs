using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class TaskManager : MonoBehaviour {

    [Header("References")]
    [SerializeField] private List<TaskInteractable> destinations = new List<TaskInteractable>();
    [SerializeField] private Level level;
    private PlayerController playerController;
    private UIController uiController;
    private AudioManager audioMangager;
    private GameManager gameManager;

    [Header("Tasks")]
    private int completedTasks;
    private bool taskStarted;
    private bool gameComplete;

    [Header("Cleanup")]
    [SerializeField] private GameObject trosh;
    [SerializeField] private int trashToSpawn;
    [SerializeField] private Vector2 topLeftTrashSpawnBound;
    [SerializeField] private Vector2 bottomRightTrashSpawnBound;
    private int trashRemaining;

    [Header("Mopping")]
    [SerializeField] private GameObject puddlePrefab;
    private Task currTask;

    [Header("Sandwich")]
    [SerializeField] private GameObject[] ingredients;
    private bool[] ingredientStatuses;
    private int currIngredientIdx;

    [Header("Pausing")]
    private bool isPaused;

    private void Start() {

        audioMangager = FindObjectOfType<AudioManager>();
        playerController = FindObjectOfType<PlayerController>();
        uiController = FindObjectOfType<UIController>();
        gameManager = FindObjectOfType<GameManager>();
        AssignDestination();

        ingredientStatuses = new bool[ingredients.Length];
        ResetIngredients();

        uiController.InitializeTimer(level.GetTimeLimit()); // begin timer

        Time.timeScale = 1f; // reset time scale

    }

    private void OnDestroy() {

        DOTween.KillAll();

    }

    public void TogglePause() {

        if (gameComplete) // game already ended
            return;

        if (isPaused)
            UnpauseGame();
        else
            PauseGame();

    }

    private void PauseGame() {

        Time.timeScale = 0f;
        isPaused = true;
        playerController.PauseBoredomTick();
        playerController.SetMechanicStatus(MechanicType.Movement, false);
        playerController.StopMeterFlash();
        uiController.PauseTimer();
        uiController.OpenPauseMenu();

    }

    private void UnpauseGame() {

        Time.timeScale = 1f;
        isPaused = false;
        playerController.StartBoredomTick();
        playerController.SetMechanicStatus(MechanicType.Movement, true);
        uiController.ResumeTimer();
        uiController.ClosePauseMenu();
        // meter flash will start automatically if needed

    }

    private void ResetIngredients() {
        //i know this is sus but DO I CARE NAH
        int i = 0;
        currIngredientIdx = 0;
        foreach (GameObject g in ingredients) {
            g.SetActive(false);
            ingredientStatuses[i] = false;
            i++;
        }
    }

    public void OnTaskComplete() {

        completedTasks++;
        FindObjectOfType<AudioManager>().PlaySound(AudioManager.GameSoundEffectType.TaskComplete);

        if (completedTasks >= level.GetTasksToComplete())
            OnGameVictory();

    }

    public int GetTotalTasks() => level.GetTasksToComplete();

    public int GetCompletedTasks() { return completedTasks; }

    public void OnGameVictory() {

        audioMangager.PlaySound(AudioManager.GameSoundEffectType.Win);
        gameComplete = true;
        playerController.PauseBoredomTick();
        playerController.SetMechanicStatus(MechanicType.Movement, false);
        playerController.StopMeterFlash();
        uiController.PauseTimer();
        uiController.ShowVictoryScreen();
        gameManager.SetLevelCompleted(level);
        gameManager.SaveLevelData();

    }

    public void OnGameLoss() {

        audioMangager.PlaySound(AudioManager.GameSoundEffectType.Lose);
        gameComplete = true;
        playerController.PauseBoredomTick();
        playerController.SetMechanicStatus(MechanicType.Movement, false);
        playerController.StopMeterFlash();
        uiController.PauseTimer();
        uiController.ShowLossScreen();
        gameManager.SaveLevelData();

    }

    public bool StartTask() {

        if (currTask == null || taskStarted) // no assigned task or already started a task
            return false;

        taskStarted = true;

        playerController.SetArrowVisible(false);

        if (currTask is CleanupTask)
            SpawnTrash();

        if (currTask is MoppingTask)
            SpawnPuddle();

        if (currTask is HomeworkTask)
            uiController.OpenHomework();

        if (currTask is TVRepairTask)
            StartCoroutine(uiController.OpenTVRepair());

        if (currTask is Sandwich)
            BeginSandwich();

        return true;

    }

    public void AssignDestination() {

        if (currTask != null || gameComplete) // already has a task or game already ended
            return;

        playerController.SetArrowVisible(true);
        TaskInteractable dest = destinations[Random.Range(0, destinations.Count)];

        UpdateTaskInteractables(dest);
        playerController.PointArrow(dest.gameObject.transform.position);

    }

    private void UpdateTaskInteractables(TaskInteractable destination) {

        for (int i = 0; i < destinations.Count; i++) // set all destinations to not interactable
            destinations[i].SetInteractable(false);

        destination.SetInteractable(true);
        currTask = destination.GetRandomTask();
        uiController.SetTaskInfo(completedTasks + 1, currTask.GetName(), currTask.GetDescription());

    }

    private void SpawnTrash() {

        for (int i = 0; i < trashToSpawn; i++)
            Instantiate(trosh, new Vector3(Random.Range(topLeftTrashSpawnBound.x, bottomRightTrashSpawnBound.x), 0, Random.Range(topLeftTrashSpawnBound.y, bottomRightTrashSpawnBound.y)), Quaternion.Euler(0, 0, Random.Range(0, 360)));

        trashRemaining = trashToSpawn;

    }

    private void SpawnPuddle() {

        Instantiate(puddlePrefab, new Vector3(Random.Range(topLeftTrashSpawnBound.x, bottomRightTrashSpawnBound.x), 0.1f, Random.Range(topLeftTrashSpawnBound.y, bottomRightTrashSpawnBound.y)), Quaternion.Euler(90f, 0f, 0f));

    }

    private void BeginSandwich() {
        ResetIngredients();
        StartCoroutine(SpawnIngredient(currIngredientIdx));
    }

    private IEnumerator SpawnIngredient(int idx) {
        if (idx >= ingredients.Length) {
            ResetIngredients();
            if (currTask is Sandwich)
                CompleteCurrentTask();
            yield break;
        } else {
            ingredients[idx].SetActive(true);
            playerController.SetArrowVisible(true);
            playerController.PointArrow(ingredients[idx].transform.position);
            while (!ingredientStatuses[idx])
                yield return null;
            ingredients[idx].SetActive(false);
            currIngredientIdx++;
            StartCoroutine(SpawnIngredient(currIngredientIdx));
        }
    }

    public float GetBoredomDecayRate() => level.GetBoredomDecayRate();

    public void OnIngredientPickup() {
        ingredientStatuses[currIngredientIdx] = true;

        //this means it has been selected
    }

    public void OnTrashPickup() {

        trashRemaining--;

        if (currTask is CleanupTask && trashRemaining == 0) // cleanup task finished
            CompleteCurrentTask();

    }


    public bool HasCurrentTask() { return currTask != null; }

    public void CompleteCurrentTask() {

        currTask = null;
        OnTaskComplete();
        AssignDestination();
        taskStarted = false;

    }

    public void FailCurrentTask() {

        currTask = null;
        AssignDestination();
        taskStarted = false;

    }

    public bool IsTaskStarted() { return taskStarted; }

    public bool IsGameComplete() { return gameComplete; }

}