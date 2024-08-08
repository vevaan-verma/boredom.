
using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    [Header("References")]
    private GameManager gameManager;
    private Animator animator;

    [Header("Main Menu")]
    [SerializeField] private CanvasGroup mainMenu;
    [SerializeField] private float mainMenuFadeDuration;
    [SerializeField] private Transform mainContent;
    [SerializeField] private Button playButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button creditsButton;

    [Header("Level Select")]
    [SerializeField] private CanvasGroup levelMenu;
    [SerializeField] private float levelMenuFadeDuration;
    [SerializeField] private Level[] levels;
    [SerializeField] private Transform levelButtonsParent;
    [SerializeField] private LevelButton levelButtonPrefab;
    [SerializeField] private Button levelsCloseButton;
    [SerializeField] private int levelIndexOffset;

    [Header("Tutorial")]
    [SerializeField] private CanvasGroup tutorialMenu;
    [SerializeField] private float tutorialMenuFadeDuration;
    [SerializeField] private Button tutorialCloseButton;

    [Header("Credits")]
    [SerializeField] private CanvasGroup creditsMenu;
    [SerializeField] private float creditsMenuFadeDuration;
    [SerializeField] private Button creditsCloseButton;

    [Header("Loading Screen")]
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private float loadingFadeDuration;
    private AsyncOperation sceneLoad;

    [Header("Animations")]
    [SerializeField] private Transform ground;
    [SerializeField] private Vector2 groundStart;
    [SerializeField] private Vector2 groundTarget;
    [SerializeField] private float groundDuration;

    private void Start() {

        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();

        OpenMainMenu();
        CloseTutorialMenu();
        CloseLevelMenu();
        CloseCreditsMenu();

        HideLoadingScreen();

        playButton.onClick.AddListener(OpenLevelMenu);
        quitButton.onClick.AddListener(() => Application.Quit());
        tutorialButton.onClick.AddListener(OpenTutorialMenu);
        creditsButton.onClick.AddListener(OpenCreditsMenu);

        tutorialCloseButton.onClick.AddListener(CloseTutorialMenu);
        levelsCloseButton.onClick.AddListener(CloseLevelMenu);
        creditsCloseButton.onClick.AddListener(CloseCreditsMenu);

        LevelButton button = Instantiate(levelButtonPrefab, levelButtonsParent);
        button.Initialize(1, levels[0]);
        button.onClick.AddListener(() => LoadLevel(levelIndexOffset));
        button.interactable = true; // level 1 is always unlocked

        for (int i = 1; i < levels.Length; i++) {

            button = Instantiate(levelButtonPrefab, levelButtonsParent);
            button.Initialize(i + 1, levels[i]);
            int levelIndex = i + levelIndexOffset;
            button.onClick.AddListener(() => LoadLevel(levelIndex));
            button.interactable = gameManager.IsLevelCompleted(levels[i - 1]);

        }

        /* INTRO ANIMATIONS */
        animator.SetTrigger("mainMenuIntro");

        ground.position = groundStart;
        ground.DOMove(Vector3.zero, groundDuration);

        Time.timeScale = 1f; // reset time scale

    }

    private void OnDestroy() {

        DOTween.KillAll();

    }

    #region Main Menu

    private void OpenMainMenu() {

        animator.SetTrigger("mainMenuIntro");
        mainMenu.gameObject.SetActive(true);
        mainMenu.DOFade(1f, mainMenuFadeDuration);

    }

    private void CloseMainMenu() {

        mainMenu.alpha = 0f;
        mainMenu.gameObject.SetActive(false);

    }

    #endregion

    #region Tutorial Menu

    private void OpenTutorialMenu() {

        animator.SetTrigger("tutorialIntro");
        CloseMainMenu();
        tutorialMenu.gameObject.SetActive(true);
        tutorialMenu.DOFade(1f, tutorialMenuFadeDuration);

    }

    private void CloseTutorialMenu() {

        tutorialMenu.alpha = 0f;
        tutorialMenu.gameObject.SetActive(false);
        OpenMainMenu();

    }

    #endregion

    #region Level Menu

    private void OpenLevelMenu() {

        animator.SetTrigger("levelsIntro");
        CloseMainMenu();
        levelMenu.gameObject.SetActive(true);
        levelMenu.DOFade(1f, levelMenuFadeDuration);
        StartCoroutine(RebuildLayout(mainContent, levelButtonsParent)); // rebuild layout EACH time the level menu is opened, rebuild AFTER making it visible

    }

    private void CloseLevelMenu() {

        levelMenu.alpha = 0f;
        levelMenu.gameObject.SetActive(false);
        OpenMainMenu();

    }

    private void LoadLevel(int buildIndex) {

        sceneLoad = SceneManager.LoadSceneAsync(buildIndex);
        sceneLoad.allowSceneActivation = false;
        ShowLoadingScreen();

    }

    #endregion

    #region Credits Menu

    private void OpenCreditsMenu() {

        animator.SetTrigger("creditsIntro");
        CloseMainMenu();
        creditsMenu.gameObject.SetActive(true);
        creditsMenu.DOFade(1f, creditsMenuFadeDuration);

    }

    private void CloseCreditsMenu() {

        creditsMenu.alpha = 0f;
        creditsMenu.gameObject.SetActive(false);
        OpenMainMenu();

    }

    #endregion

    #region Loading Screen

    private void ShowLoadingScreen() {

        loadingScreen.alpha = 0f;
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(1f, loadingFadeDuration).OnComplete(() => {

            if (sceneLoad != null) sceneLoad.allowSceneActivation = true;

        });
    }

    private void HideLoadingScreen() {

        loadingScreen.alpha = 1f;
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(0f, loadingFadeDuration).OnComplete(() => loadingScreen.gameObject.SetActive(false));

    }

    #endregion

    #region Utility

    private void RefreshLayout(Transform layout) {

        LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());

    }

    private IEnumerator RebuildLayout(Transform layout1, Transform layout2) { // make sure to maintain order

        RefreshLayout(layout1);
        yield return new WaitForEndOfFrame();
        RefreshLayout(layout2);

    }

    #endregion
}

