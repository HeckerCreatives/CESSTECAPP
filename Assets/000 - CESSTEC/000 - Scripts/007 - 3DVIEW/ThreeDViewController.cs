using Cinemachine;
using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThreeDViewController : MonoBehaviour
{
    public enum ViewState
    {
        NONE,
        INTERIOR,
        EXTERIOR
    }
    private event EventHandler ChangeView;
    public event EventHandler OnChangeView
    {
        add
        {
            if (ChangeView == null || !ChangeView.GetInvocationList().Contains(value))
                ChangeView += value;
        }
        remove { ChangeView -= value; }
    }
    public ViewState CurrentViewState
    {
        get => currentViewState;
        set
        {
            currentViewState = value;
            ChangeView?.Invoke(this, EventArgs.Empty);
        }
    }

    //  =========================================

    [SerializeField] private GameManager gameManager;
    [SerializeField] private AppStateManager appState;
    [SerializeField] private ComponentSystemController componentSystem;
    [SerializeField] private ErrorController errorController;
    [SerializeField] private GameObject loadingNoBG;

    [Header("AIRPLANE")]
    [SerializeField] private Airplane airplane;

    [Header("DEBUGGER")]
    [ReadOnly] [SerializeField] private ViewState currentViewState;

    private void Awake()
    {
        appState.OnAppStateChange += AppStateChange;
        ChangeView += ViewChange;
    }


    private void OnDisable()
    {
        appState.OnAppStateChange -= AppStateChange;
        ChangeView -= ViewChange;
    }

    private void AppStateChange(object sender, EventArgs e)
    {
        CheckAirplane();
    }

    private void ViewChange(object sender, EventArgs e)
    {
        CheckView();
    }

    private void CheckAirplane()
    {
        if (appState.GetCurrentAppState != AppStateManager.AppState.VIEW3D) return;

        airplane[componentSystem.CurrentAirplaneType].airplaneObj.SetActive(true);
    }

    private void CheckView()
    {
        if (appState.GetCurrentAppState != AppStateManager.AppState.VIEW3D) return;

        if (CurrentViewState == ViewState.INTERIOR)
        {
            airplane[componentSystem.CurrentAirplaneType].interiorBtnObj.SetActive(true);
            airplane[componentSystem.CurrentAirplaneType].exteriorBtnObj.SetActive(false);

            airplane[componentSystem.CurrentAirplaneType].exteriorVcam.m_Priority = 9;
            airplane[componentSystem.CurrentAirplaneType].interiorVCam.m_Priority = 10;
        }
        else if (CurrentViewState == ViewState.EXTERIOR)
        {
            airplane[componentSystem.CurrentAirplaneType].exteriorBtnObj.SetActive(true);
            airplane[componentSystem.CurrentAirplaneType].interiorBtnObj.SetActive(false);


            airplane[componentSystem.CurrentAirplaneType].interiorVCam.m_Priority = 9;
            airplane[componentSystem.CurrentAirplaneType].exteriorVcam.m_Priority = 10;
        }
    }

    #region BUTTON

    public void ChangeCurrentView(int index)
    {
        CurrentViewState = (ViewState)index;
    }

    public void BackButton()
    {
        if (!gameManager.CanUseButtons) return;

        gameManager.CanUseButtons = true;

        appState.Back = true;

        airplane[componentSystem.CurrentAirplaneType].airplaneObj.SetActive(false);
        appState.RemoveAppStateHistory();
    }

    #endregion
}

[Serializable]
public class Airplane : SerializableDictionary<ComponentSystemController.AirplaneType, AirplaneData> { }

[Serializable]
public class AirplaneData
{
    public GameObject airplaneObj;
    public GameObject interiorBtnObj;
    public GameObject exteriorBtnObj;
    public CinemachineVirtualCamera interiorVCam;
    public CinemachineFreeLook exteriorVcam;
}
