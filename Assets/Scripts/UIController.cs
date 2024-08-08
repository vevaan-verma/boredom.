using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [Header("References")]
    private PlayerController playerController;
    private TaskManager taskManager;
    private GameData gameData;
    private Animator animator;

    [Header("Tasks")]
    [SerializeField] private CanvasGroup mainHUD;
    [SerializeField] private float mainHUDFadeDuration;
    [SerializeField] private Transform taskInfo;
    [SerializeField] private TMP_Text taskHeaderText;
    [SerializeField] private TMP_Text taskNameText;
    [SerializeField] private TMP_Text taskDescriptionText;

    [Header("Timer")]
    [SerializeField] private TMP_Text timerText; // timer text is part of HUD
    [SerializeField] private TMP_Text taskTimerText; // task timer text is part of HUD (but it only shows up for certain tasks)
    [SerializeField] private float flashThreshold;
    [SerializeField] private float flashWaitDuration;
    [SerializeField] private Color flashColor;
    private Coroutine flashTimerCoroutine;
    private Coroutine timerCoroutine;
    private int timerSeconds;

    [Header("Quiz")]
    [SerializeField] private GameObject homework;
    [SerializeField] private float homeworkFadeDuration;
    [SerializeField] private Transform homeworkContent;
    [SerializeField] private HomeworkQuestionUI homeworkQuestionPrefab;
    [SerializeField] private Button submitButtonPrefab;
    [SerializeField] private float homeworkCompleteWaitDuration;
    private List<HomeworkQuestionUI> questionPrefabs;
    private bool homeworkOpen;

    [Header("TV Repair")]
    [SerializeField] private CanvasGroup tvRepair;
    [SerializeField] private float tvRepairFadeDuration;
    [SerializeField] private float tvRepairCompleteWaitDuration;
    [SerializeField] private List<TVRepairQuestionUI> tvRepairQuestionObjs;
    [SerializeField] private List<TVRepairAnswerUI> tvRepairAnswerObjs;
    [SerializeField] private GameObject checkmark;
    private TVRepairQuestion[] tvRepairQuestions;
    private List<string> orderedAnswers;
    private bool tvRepairOpen;

    [Header("Victory Screen")]
    [SerializeField] private CanvasGroup victoryScreen;
    [SerializeField] private float victoryFadeDuration;
    [SerializeField] private Button victoryMenuButton;
    [SerializeField] private Button victoryRestartButton;
    [SerializeField] private Button victoryQuitButton;

    [Header("Loss Screen")]
    [SerializeField] private CanvasGroup lossScreen;
    [SerializeField] private float lossFadeDuration;
    [SerializeField] private Button lossMenuButton;
    [SerializeField] private Button lossRestartButton;
    [SerializeField] private Button lossQuitButton;

    [Header("Pause Menu")]
    [SerializeField] private CanvasGroup pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Loading Screen")]
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private float loadingFadeDuration;
    private AsyncOperation sceneLoad;

    private void Start() {

        playerController = FindObjectOfType<PlayerController>();
        taskManager = FindObjectOfType<TaskManager>();
        gameData = FindObjectOfType<GameData>();
        animator = GetComponent<Animator>();

        victoryScreen.gameObject.SetActive(false);
        lossScreen.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        HideLoadingScreen();

        homework.gameObject.SetActive(false);
        questionPrefabs = new List<HomeworkQuestionUI>();
        //quizCloseButton.onClick.AddListener(() => StartCoroutine(CloseQuiz(false)));

        tvRepair.gameObject.SetActive(false);
        checkmark.SetActive(false);

        victoryMenuButton.onClick.AddListener(LoadMainMenu);
        victoryRestartButton.onClick.AddListener(ReloadLevel);
        victoryQuitButton.onClick.AddListener(() => Application.Quit());

        lossMenuButton.onClick.AddListener(LoadMainMenu);
        lossRestartButton.onClick.AddListener(ReloadLevel);
        lossQuitButton.onClick.AddListener(() => Application.Quit());

        /* order tv repair answers by index */
        List<TVRepairAnswerUI> ordered = new List<TVRepairAnswerUI>(new TVRepairAnswerUI[tvRepairAnswerObjs.Count]);

        for (int i = 0; i < tvRepairAnswerObjs.Count; i++) {

            tvRepairAnswerObjs[i].Initialize();
            ordered[tvRepairAnswerObjs[i].GetIndex()] = tvRepairAnswerObjs[i];

        }

        tvRepairAnswerObjs = ordered;

        taskTimerText.gameObject.SetActive(false);

        StartCoroutine(RebuildLayout(taskInfo));

        resumeButton.onClick.AddListener(taskManager.TogglePause);
        restartButton.onClick.AddListener(ReloadLevel);
        mainMenuButton.onClick.AddListener(LoadMainMenu);

        ResetTaskInfo();

    }

    //private void Update() {

    //    /* QUIZ */
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //        StartCoroutine(CloseQuiz(false)); // close quiz

    //}

    #region Timer

    public void InitializeTimer(int seconds) {

        timerCoroutine = StartCoroutine(HandleTimer(seconds));

    }

    public void ResumeTimer() {

        timerCoroutine = StartCoroutine(HandleTimer(timerSeconds));

    }

    public void PauseTimer() {

        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

    }

    private IEnumerator HandleTimer(int seconds) {

        this.timerSeconds = seconds;

        while (timerSeconds > 0) {

            if (timerSeconds <= flashThreshold) {

                if (flashTimerCoroutine == null) // make sure timer isn't already flashing
                    flashTimerCoroutine = StartCoroutine(FlashTimer()); // start flashing

            } else if (flashTimerCoroutine != null) { // timer is already flashing but time is above threshold

                StopCoroutine(flashTimerCoroutine); // stop flashing

            }

            taskTimerText.text = timerText.text = string.Format("{0:0}:{1:00}", timerSeconds / 60, timerSeconds % 60);
            yield return new WaitForSeconds(1f);
            timerSeconds--;

        }

        if (flashTimerCoroutine != null)
            StopCoroutine(flashTimerCoroutine); // stop flashing

        taskTimerText.text = timerText.text = "0:00";
        taskManager.OnGameLoss();

    }

    private IEnumerator FlashTimer() {

        while (true) {

            taskTimerText.color = timerText.color = Color.clear;
            yield return new WaitForSeconds(flashWaitDuration);
            taskTimerText.color = timerText.color = flashColor;
            yield return new WaitForSeconds(flashWaitDuration);

        }
    }

    #endregion

    #region Tasks

    private void ShowMainHUD() {

        mainHUD.gameObject.SetActive(true);
        mainHUD.DOFade(1f, mainHUDFadeDuration).SetUpdate(true);

    }

    private void HideMainHUD() {

        mainHUD.DOFade(0f, mainHUDFadeDuration).SetUpdate(true).OnComplete(() => mainHUD.gameObject.SetActive(false));

    }

    public void SetTaskInfo(int taskNum, string taskName, string taskDescription) {

        if (taskNum == 0)
            taskHeaderText.text = "No Task";
        else
            taskHeaderText.text = "Task " + taskNum + "/" + taskManager.GetTotalTasks() + ":";

        taskNameText.text = taskName;
        taskDescriptionText.text = taskDescription;
        StartCoroutine(RebuildLayout(taskInfo));

    }

    public void ResetTaskInfo() {

        SetTaskInfo(0, "", "");

    }

    #endregion

    #region Homework

    public void OpenHomework() {

        if (homeworkOpen) return; // homework already open

        taskTimerText.gameObject.SetActive(true);
        playerController.PauseBoredomTick(); // stop ticking boredom
        playerController.SetMechanicStatus(MechanicType.Movement, false);

        foreach (Transform child in homeworkContent)
            Destroy(child.gameObject);

        Homework homework = gameData.GetQuiz();
        HomeworkQuestion[] homeworkQuestions = homework.GetRandomQuestions();

        foreach (HomeworkQuestion question in homeworkQuestions) {

            HomeworkQuestionUI questionObject = Instantiate(homeworkQuestionPrefab, homeworkContent);
            questionObject.SetQuestionText(question.GetQuestionText());
            questionObject.SetOptionTexts(question.GetOptions());
            questionPrefabs.Add(questionObject); // track for validation purposes (added in same order as homework questions)

        }

        Button submitButton = Instantiate(submitButtonPrefab, homeworkContent);
        submitButton.onClick.AddListener(() => {

            submitButton.interactable = false; // prevent multiple submissions
            StartCoroutine(OnHomeworkComplete(homework.ValidateAnswers(questionPrefabs)));

        });

        this.homework.gameObject.SetActive(true);
        StartCoroutine(RebuildLayout(homeworkContent, this.homework.transform)); // rebuild layout AFTER making it visible

        animator.SetTrigger("openHomework");
        homeworkOpen = true;

    }

    private IEnumerator OnHomeworkComplete(bool pass) {

        yield return new WaitForSeconds(homeworkCompleteWaitDuration);
        yield return StartCoroutine(CloseHomework(pass)); // wait for homework to close

    }

    private IEnumerator CloseHomework(bool pass) {

        if (!homeworkOpen) yield break; // homework already closed

        taskTimerText.gameObject.SetActive(false);
        animator.SetTrigger("closeHomework");
        yield return new WaitForEndOfFrame(); // wait for animation to start playing

        if (pass)
            taskManager.CompleteCurrentTask(); // complete task
        else
            taskManager.FailCurrentTask(); // fail task

        // clear all homework data
        questionPrefabs.Clear();

        playerController.SetMechanicStatus(MechanicType.Movement, true); // allow movement before homework closes fully

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // wait for animation to finish playing

        homework.gameObject.SetActive(false);
        homeworkOpen = false;

        if (!taskManager.IsGameComplete())
            playerController.StartBoredomTick(); // start ticking boredom again only if game is still going

    }

    #endregion

    #region TV Repair

    public IEnumerator OpenTVRepair() {

        if (tvRepairOpen) yield break; // tv repair already open

        taskTimerText.gameObject.SetActive(true);
        this.tvRepair.blocksRaycasts = true; // allow interactions

        playerController.PauseBoredomTick(); // stop ticking boredom
        playerController.SetMechanicStatus(MechanicType.Movement, false);
        checkmark.SetActive(false);

        TVRepair tvRepair = gameData.GetTVRepair();
        tvRepairQuestions = tvRepair.GetRandomQuestions();
        List<string> availableAnswers = new List<string>();
        orderedAnswers = new List<string>(new string[tvRepairQuestions.Length]);

        // set questions
        for (int i = 0; i < tvRepairQuestions.Length; i++) {

            tvRepairQuestionObjs[i].SetQuestionText(tvRepairQuestions[i]);
            availableAnswers.Add(tvRepairQuestions[i].GetAnswerText());

        }

        // randomize answers
        int guaranteedRandom = Random.Range(0, tvRepairQuestions.Length); // INDEX OF ANSWER THAT WILL BE INSERTED (FROM AVAILABLE ANSWERS)
        int randIndex; // INDEX OF WHICH POSITION ANSWER IS INSERTED INTO

        if (guaranteedRandom == 0) {

            randIndex = Random.Range(1, tvRepairQuestions.Length); // random index from 1 to length - 1

        } else if (guaranteedRandom == tvRepairQuestions.Length - 1) {

            randIndex = Random.Range(0, tvRepairQuestions.Length - 1); // random index from 0 to length - 2

        } else {

            int[] ranges = { Random.Range(0, guaranteedRandom), Random.Range(guaranteedRandom + 1, tvRepairQuestions.Length) }; // random ranges from 0 to guaranteedRandom - 1 and guaranteedRandom + 1 to length - 1
            randIndex = ranges[Random.Range(0, ranges.Length)]; // random index from ranges

        }

        orderedAnswers[randIndex] = availableAnswers[guaranteedRandom]; // insert guaranteed random answer at random index
        tvRepairAnswerObjs[randIndex].SetAnswerText(availableAnswers[guaranteedRandom]); // set answer text (same index as insert index of ordered answers)
        availableAnswers.RemoveAt(guaranteedRandom); // remove guaranteed random answer

        for (int i = 0; i < tvRepairQuestions.Length; i++) { // I IS THE INDEX OF THE ANSWER THAT WILL BE INSERTED (FROM AVAILABLE ANSWERS)

            if (availableAnswers.Count == 0) break; // no more answers to randomize

            do {

                yield return null;
                randIndex = Random.Range(0, tvRepairQuestions.Length); // random index from ranges

            } while (orderedAnswers[randIndex] != null);

            if (i == guaranteedRandom) continue; // skip guaranteed random answer

            orderedAnswers[randIndex] = availableAnswers[availableAnswers.Count - 1]; // insert random answer at random index
            tvRepairAnswerObjs[randIndex].SetAnswerText(availableAnswers[availableAnswers.Count - 1]); // set answer text (same index as insert index of ordered answers)
            availableAnswers.RemoveAt(availableAnswers.Count - 1); // remove random answer

        }

        this.tvRepair.gameObject.SetActive(true);

        animator.SetTrigger("openTVRepair");
        tvRepairOpen = true;

    }

    private IEnumerator OnTVRepairComplete(bool pass) {

        checkmark.SetActive(true);
        animator.SetTrigger("completeTVRepair");
        tvRepair.blocksRaycasts = false; // stop interactions
        yield return new WaitForSeconds(tvRepairCompleteWaitDuration);
        yield return StartCoroutine(CloseTVRepair(pass)); // wait for tv repair to close

    }

    private IEnumerator CloseTVRepair(bool pass) {

        if (!tvRepairOpen) yield break; // tv repair already closed

        taskTimerText.gameObject.SetActive(false);
        animator.SetTrigger("closeTVRepair");
        yield return new WaitForEndOfFrame(); // wait for animation to start playing

        if (pass)
            taskManager.CompleteCurrentTask(); // complete task
        else
            taskManager.FailCurrentTask(); // fail task

        playerController.SetMechanicStatus(MechanicType.Movement, true); // allow movement before tv repair closes fully

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // wait for animation to finish playing

        tvRepair.gameObject.SetActive(false);
        tvRepairOpen = false;

        if (!taskManager.IsGameComplete())
            playerController.StartBoredomTick(); // start ticking boredom again only if game is still going

    }

    public void OnTVRepairDrop() {

        /* order tv repair questions by index */
        List<TVRepairQuestionUI> ordered = new List<TVRepairQuestionUI>(new TVRepairQuestionUI[tvRepairQuestionObjs.Count]);

        for (int i = 0; i < tvRepairQuestionObjs.Count; i++)
            ordered[tvRepairQuestionObjs[i].GetIndex()] = tvRepairQuestionObjs[i];

        if (gameData.GetTVRepair().ValidateAnswers(ordered, orderedAnswers)) StartCoroutine(OnTVRepairComplete(true));

    }

    #endregion

    #region Scene Loading

    private void LoadMainMenu() {

        sceneLoad = SceneManager.LoadSceneAsync(0);
        sceneLoad.allowSceneActivation = false;
        ShowLoadingScreen();

    }

    private void ReloadLevel() {

        sceneLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        sceneLoad.allowSceneActivation = false;
        ShowLoadingScreen();

    }

    #endregion

    #region Pausing

    public void OpenPauseMenu() {

        HideMainHUD();
        pauseMenu.gameObject.SetActive(true);
        pauseMenu.DOFade(1f, mainHUDFadeDuration).SetUpdate(true);
        animator.SetTrigger("openPauseMenu");

    }

    public void ClosePauseMenu() {

        ShowMainHUD();
        animator.SetTrigger("closePauseMenu");
        pauseMenu.DOFade(0f, lossFadeDuration).SetUpdate(true);
        StartCoroutine(HidePauseMenu());

    }

    private IEnumerator HidePauseMenu() { // helper method, actually closes the menu

        yield return new WaitForEndOfFrame(); // wait for animation to start playing
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // wait for animation to finish playing
        pauseMenu.gameObject.SetActive(false); // hide pause menu after animation ends

    }

    #endregion

    #region Victory/Loss

    public void ShowVictoryScreen() {

        StartCoroutine(CloseHomework(false));
        StartCoroutine(CloseTVRepair(false));

        victoryScreen.alpha = 0f;
        victoryScreen.gameObject.SetActive(true);
        mainHUD.DOFade(0f, victoryFadeDuration).SetEase(Ease.InCubic).OnComplete(() => mainHUD.gameObject.SetActive(false));
        victoryScreen.DOFade(1f, victoryFadeDuration).SetEase(Ease.InCubic);

    }

    public void ShowLossScreen() {

        StartCoroutine(CloseHomework(false));
        StartCoroutine(CloseTVRepair(false));

        lossScreen.alpha = 0f;
        lossScreen.gameObject.SetActive(true);
        mainHUD.DOFade(0f, lossFadeDuration).SetEase(Ease.InCubic).OnComplete(() => mainHUD.gameObject.SetActive(false));
        lossScreen.DOFade(1f, lossFadeDuration).SetEase(Ease.InCubic);

    }

    #endregion

    #region Loading Screen

    private void ShowLoadingScreen() {

        loadingScreen.alpha = 0f;
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(1f, loadingFadeDuration).SetUpdate(true).SetEase(Ease.InCubic).OnComplete(() => {

            if (sceneLoad != null) sceneLoad.allowSceneActivation = true;

        });
    }

    private void HideLoadingScreen() {

        loadingScreen.alpha = 1f;
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(0f, loadingFadeDuration).SetEase(Ease.InCubic).OnComplete(() => loadingScreen.gameObject.SetActive(false));

    }

    #endregion

    #region Utility

    private void RefreshLayout(Transform layout) {

        LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());

    }

    private IEnumerator RebuildLayout(Transform layout) { // make sure to maintain order

        layout.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        layout.gameObject.SetActive(true);
        RefreshLayout(layout);

    }

    private IEnumerator RebuildLayout(Transform layout1, Transform layout2) { // make sure to maintain order

        RefreshLayout(layout1);
        yield return new WaitForEndOfFrame();
        RefreshLayout(layout2);

    }

    #endregion

}