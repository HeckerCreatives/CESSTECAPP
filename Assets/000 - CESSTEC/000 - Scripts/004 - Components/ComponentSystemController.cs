using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ComponentSystemController : MonoBehaviour
{
    public enum AirplaneType
    {
        NONE,
        CESSNA152,
        TECNAMP2002JF,
        CESSNA172,
        TECNAMP20006T
    }

    public enum Systems
    {
        System1,
        System2,
        System3,
        System4,
        System5,
        System6,
        System7,
        System8,
        System9,
        System10,
        System11,
        System12,
        System13,
        System14
    }

    public enum PictureType
    {
        SMALL,
        LARGE
    }

    //  =============================

    [SerializeField] private GameManager gameManager;
    [SerializeField] private AppStateManager appState;
    [SerializeField] private TopicController topicController;
    [SerializeField] private QuizController quizController;
    [SerializeField] private ErrorController errorController;
    [SerializeField] private ThreeDViewController threeDViewController;
    [SerializeField] private GameObject loadingNoBG;

    [Header("MENU")]
    [SerializeField] private GameObject cessna152Title;
    [SerializeField] private GameObject cessna172Title;
    [SerializeField] private GameObject tecnamP2002JFTitle;
    [SerializeField] private GameObject tecnamP20006TTitle;
    [SerializeField] private Image airplaneImg;
    [SerializeField] private List<string> cessna152Systems;
    [SerializeField] private List<string> tecnamp2002Systems;
    [SerializeField] private List<string> cessna172Systems;
    [SerializeField] private List<string> tecnamp2006Systems;
    [SerializeField] private List<TextMeshProUGUI> systemList;

    [Header("Sprites")]
    [SerializeField] private Sprite cessna152Sprite;
    [SerializeField] private Sprite technamp2002ifSprite;
    [SerializeField] private Sprite cessna172Sprite;
    [SerializeField] private Sprite technamp2006tSprite;

    [field: Header("DEBUGGER")]
    [field: ReadOnly] [field: SerializeField] public AirplaneType CurrentAirplaneType { get; set; }
    [field: ReadOnly] [field: SerializeField] public Systems CurrentSystems { get; set; }

    public void ChangeComponentsData(Action action)
    {
        cessna152Title.SetActive(false);
        cessna172Title.SetActive(false);
        tecnamP2002JFTitle.SetActive(false);
        tecnamP20006TTitle.SetActive(false);

        switch (CurrentAirplaneType)
        {
            case AirplaneType.CESSNA152:
                ChangeSystemNames(cessna152Systems);
                cessna152Title.SetActive(true);
                airplaneImg.sprite = cessna152Sprite;
                break;
            case AirplaneType.TECNAMP2002JF:
                ChangeSystemNames(tecnamp2002Systems);
                tecnamP2002JFTitle.SetActive(true);
                airplaneImg.sprite = technamp2002ifSprite;
                break;
            case AirplaneType.CESSNA172:
                ChangeSystemNames(cessna172Systems);
                cessna172Title.SetActive(true);
                airplaneImg.sprite = cessna172Sprite;
                break;
            case AirplaneType.TECNAMP20006T:
                ChangeSystemNames(tecnamp2006Systems);
                tecnamP20006TTitle.SetActive(true);
                airplaneImg.sprite = technamp2006tSprite;
                break;
        }

        action?.Invoke();
    }

    private void ChangeSystemNames(List<string> names)
    {
        for (int a = 0; a < systemList.Count; a++)
        {
            if (a >= names.Count)
                systemList[a].text = "NO SYSTEM";
            else
                systemList[a].text = names[a];
        }
    }

    #region BUTTON

    public void ResetExam()
    {
        if (!gameManager.CanUseButtons) return;

        gameManager.CanUseButtons = false;

        if (PlayerPrefs.GetString(((int)CurrentAirplaneType).ToString()) == "1")
        {
            errorController.ShowConfirmation("Are you sure you want to reset your quiz?", () =>
            {
                PlayerPrefs.SetString(((int)CurrentAirplaneType).ToString(), "0");
            }, () => gameManager.CanUseButtons = true);

            return;
        }

        errorController.ShowError("You still didn't take any quiz on this airplane yet", null);
    }

    public void To3DView()
    {
        if (!gameManager.CanUseButtons) return;

        gameManager.CanUseButtons = false;

        threeDViewController.CurrentViewState = ThreeDViewController.ViewState.EXTERIOR;
        appState.AddAppStateHistory(AppStateManager.AppState.VIEW3D);
    }

    public void TakeExam()
    {
        if (!gameManager.CanUseButtons) return;

        loadingNoBG.SetActive(true);

        gameManager.CanUseButtons = false;

        if (PlayerPrefs.GetString(((int)CurrentAirplaneType).ToString()) == "1")
        {
            errorController.ShowError("You have a score of " + PlayerPrefs.GetInt(((int)CurrentAirplaneType).ToString()+" Score"), null);
            gameManager.CanUseButtons = true;
            loadingNoBG.SetActive(false);
            return;
        }

        StartCoroutine(quizController.ShuffleQuestions(() => appState.AddAppStateHistory(AppStateManager.AppState.QUIZ)));
    }

    public void SystemsButton(int value)
    {
        if (!gameManager.CanUseButtons) return;

        gameManager.CanUseButtons = false;

        if (!topicController.AirplaneTopicDataList[CurrentAirplaneType].ContainsKey((Systems)value))
        {
            errorController.ShowError("No system available for this aircraft!", null);
            gameManager.CanUseButtons = true;
            return;
        }
        CurrentSystems = (Systems)value;
        appState.AddAppStateHistory(AppStateManager.AppState.TOPIC);
    }

    #endregion
}
