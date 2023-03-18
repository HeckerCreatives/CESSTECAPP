using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AppStateManager : MonoBehaviour
{
    public enum AppState
    {
        NONE,
        START,
        MAINMENU,
        COMPONENTS,
        TOPIC,
        QUIZ,
        VIEW3D,
        ABOUTUS
    }
    private event EventHandler AppStateChange;
    public event EventHandler OnAppStateChange
    {
        add
        {
            if (AppStateChange == null || !AppStateChange.GetInvocationList().Contains(value)) AppStateChange += value;
        }
        remove { AppStateChange -= value; }
    }
    public void AddAppStateHistory(AppState value)
    {
        if (appStateHistory.Count != 0)
            lastAppState = appStateHistory[appStateHistory.Count - 1];

        appStateHistory.Add(value);
        AppStateChange?.Invoke(this, EventArgs.Empty);
    }
    public List<AppState> GetAppStateHistory
    {
        get => appStateHistory;
    }
    public AppState GetCurrentAppState
    {
        get => appStateHistory[appStateHistory.Count - 1];
    }
    public void RemoveAppStateHistory()
    {
        lastAppState = appStateHistory[appStateHistory.Count - 1];
        appStateHistory.RemoveAt(appStateHistory.Count - 1);
        AppStateChange?.Invoke(this, EventArgs.Empty);
    }
    public void ResetAppStateHistory()
    {
        lastAppState = AppState.START;
        appStateHistory.Clear();
        appStateHistory.Add(AppState.START);
        AppStateChange?.Invoke(this, EventArgs.Empty);
    }
    public AppState LastAppState
    {
        get => lastAppState;
    }

    //  ===============================

    [SerializeField] private GameManager gameManager;
    [SerializeField] private QuizController quizController;
    [SerializeField] private ErrorController errorController;
    [SerializeField] private TopicController topicController;
    [SerializeField] private ThreeDViewController threeDViewController;

    [Header("CANVAS GROUP")]
    [SerializeField] private CanvasGroup startCG;
    [SerializeField] private CanvasGroup mainMenuCG;
    [SerializeField] private CanvasGroup topicCG;
    [SerializeField] private CanvasGroup quizCG;
    [SerializeField] private CanvasGroup componentsCG;
    [SerializeField] private CanvasGroup view3DCG;
    [SerializeField] private CanvasGroup aboutUsCG;

    [Header("DEBUGGER")]
    [ReadOnly] [SerializeField] private List<AppState> appStateHistory;
    [ReadOnly] [SerializeField] private AppState lastAppState;
    [field: ReadOnly] [field: SerializeField] public bool Back { get; set; }

    private void Awake()
    {
        gameManager.CanUseButtons = true;
        Back = false;
        ResetAppStateHistory();
        OnAppStateChange += StateChange;
    }

    private void OnDisable()
    {
        OnAppStateChange -= StateChange;
    }

    private void StateChange(object sender, EventArgs e)
    {
        Animation();
    }

    public void Update()
    {
        if (gameManager.CanUseButtons && Input.GetKeyUp(KeyCode.Escape))
        {
            if (errorController.confirmationPanelObj.gameObject.activeSelf)
            {
                errorController.CloseConfirmationAction();
            }
            else if (errorController.errorPanelObj.gameObject.activeSelf)
            {
                errorController.CloseErrorAction();
            }
            else
            {
                if (GetAppStateHistory.Count > 1)
                {
                    Back = true;

                    if (GetCurrentAppState == AppState.QUIZ)
                        quizController.BackButton();
                    else if (GetCurrentAppState == AppState.TOPIC)
                        topicController.BackButton();
                    else if (GetCurrentAppState == AppState.VIEW3D)
                        threeDViewController.BackButton();
                    else
                        RemoveAppStateHistory();
                }
                else
                {
                    errorController.ShowConfirmation("Are you sure you want to quit?", () => { Application.Quit(); }, null);
                }
            }
        }
    }

    public void BackButton() => Back = true;

    private void Animation()
    {
        switch (GetCurrentAppState)
        {
            case AppState.START:

                if (Back)
                {
                    if (LastAppState == AppState.MAINMENU)
                        mainMenuCG.gameObject.SetActive(false);
                }
                else
                    mainMenuCG.gameObject.SetActive(false);
                startCG.gameObject.SetActive(true);

                break;
            case AppState.MAINMENU:

                if (Back)
                {
                    if (LastAppState == AppState.COMPONENTS)
                        componentsCG.gameObject.SetActive(false);
                    else if (LastAppState == AppState.ABOUTUS)
                        aboutUsCG.gameObject.SetActive(false);
                }
                else
                {
                    startCG.gameObject.SetActive(false);
                }
                mainMenuCG.gameObject.SetActive(true);

                break;
            case AppState.ABOUTUS:
                aboutUsCG.gameObject.SetActive(true);
                break;
            case AppState.COMPONENTS:

                if (Back)
                {
                    if (lastAppState == AppState.VIEW3D)
                        view3DCG.gameObject.SetActive(false);
                    else if (lastAppState == AppState.QUIZ)
                        quizCG.gameObject.SetActive(false);
                    else if (lastAppState == AppState.TOPIC)
                        topicCG.gameObject.SetActive(false);
                }
                else
                    mainMenuCG.gameObject.SetActive(false);

                componentsCG.gameObject.SetActive(true);

                break;
            case AppState.TOPIC:

                if (Back)
                {
                    if (lastAppState == AppState.QUIZ)
                        quizCG.gameObject.SetActive(false);
                }
                else
                    componentsCG.gameObject.SetActive(false);
                topicCG.gameObject.SetActive(true);

                break;
            case AppState.QUIZ:
                topicCG.gameObject.SetActive(false);
                quizCG.gameObject.SetActive(true);
                break;
            case AppState.VIEW3D:
                componentsCG.gameObject.SetActive(false);
                view3DCG.gameObject.SetActive(true);
                break;
        }

        Back = false;
        gameManager.CanUseButtons = true;
    }
}
