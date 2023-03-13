using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AboutUsController : MonoBehaviour
{

    [SerializeField] private GameManager gameManager;
    [SerializeField] private AppStateManager appState;
    [SerializeField] private TextMeshProUGUI developerNamesTMP;
    [SerializeField] private Image devImg;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Developers developers;

    [Header("DEBUGGER")]
    [ReadOnly] [SerializeField] private int currentIndex;

    private void Awake()
    {
        appState.OnAppStateChange += AppStateChange;
    }

    private void OnDisable()
    {
        appState.OnAppStateChange -= AppStateChange;
    }

    private void AppStateChange(object sender, EventArgs e)
    {
        ShowDevelopers();
    }

    private void ShowDevelopers()
    {
        if (appState.GetCurrentAppState != AppStateManager.AppState.ABOUTUS) return;

        //currentIndex = 0;

        CheckButtons();

        developerNamesTMP.text = developers[currentIndex].devName;
        devImg.sprite = developers[currentIndex].devSprites;
    }

    private void CheckButtons()
    {
        if (developers.Count <= 1)
        {
            previousButton.interactable = false;
            nextButton.interactable = false;
        }
        else if (currentIndex == 0)
        {
            previousButton.interactable = false;
            nextButton.interactable = true;
        }
        else if (currentIndex > 0 && currentIndex < developers.Count - 1)
        {
            previousButton.interactable = true;
            nextButton.interactable = true;
        }
        else if (currentIndex >= developers.Count - 1)
        {
            previousButton.interactable = true;
            nextButton.interactable = false;
        }

        gameManager.CanUseButtons = true;
    }

    #region BUTTON

    public void NextPreviousButton(bool value)
    {
        if (!gameManager.CanUseButtons) return;

        gameManager.CanUseButtons = false;

        //NEXT
        if (value)
            currentIndex++;
        else
            currentIndex--;

        ShowDevelopers();
    }
    public void BackButton()
    {
        if (!gameManager.CanUseButtons) return;

        gameManager.CanUseButtons = false;

        appState.Back = true;

        currentIndex = 0;

        appState.RemoveAppStateHistory();
    }

    #endregion
}

[Serializable]
public class Developers: SerializableDictionary<int, DeveloperData> { }

[Serializable]
public class DeveloperData
{
    public Sprite devSprites;
    public string devName;
}
