using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour
{
    [SerializeField] private AppStateManager appStateManager;

    public void ToMainMenu()
    {
        appStateManager.AddAppStateHistory(AppStateManager.AppState.MAINMENU);
    }
}
