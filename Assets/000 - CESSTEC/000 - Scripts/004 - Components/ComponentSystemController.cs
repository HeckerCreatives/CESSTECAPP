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
        System12,
        System11,
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
                cessna152Title.SetActive(true);
                airplaneImg.sprite = cessna152Sprite;
                break;
            case AirplaneType.TECNAMP2002JF:
                tecnamP2002JFTitle.SetActive(true);
                airplaneImg.sprite = technamp2002ifSprite;
                break;
            case AirplaneType.CESSNA172:
                cessna172Title.SetActive(true);
                airplaneImg.sprite = cessna172Sprite;
                break;
            case AirplaneType.TECNAMP20006T:
                tecnamP20006TTitle.SetActive(true);
                airplaneImg.sprite = technamp2006tSprite;
                break;
        }

        action?.Invoke();
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
