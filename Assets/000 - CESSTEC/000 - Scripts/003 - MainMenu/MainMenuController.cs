using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AppStateManager appStateManager;
    [SerializeField] private ComponentSystemController componentSystem;

    public void ToComponentSystem(int index)
    {
        componentSystem.CurrentAirplaneType = (ComponentSystemController.AirplaneType)index;
        componentSystem.ChangeComponentsData(() =>
        {
            appStateManager.AddAppStateHistory(AppStateManager.AppState.COMPONENTS);
        });
    }

    public void ToAboutUs()
    {
        if (!gameManager.CanUseButtons) return;

        gameManager.CanUseButtons = true;

        appStateManager.AddAppStateHistory(AppStateManager.AppState.ABOUTUS);
    }
}
