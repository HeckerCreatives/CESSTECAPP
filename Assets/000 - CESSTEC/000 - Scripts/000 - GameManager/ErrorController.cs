using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [Header("CANVAS GROUP")]
    public CanvasGroup errorPanelObj;
    public CanvasGroup confirmationPanelObj;

    [Header("TMP")]
    [SerializeField] private TextMeshProUGUI errorTMP;
    [SerializeField] private TextMeshProUGUI confirmationTMP;

    public Action currentAction, closeAction;

    #region CONFIRMATION

    public void ShowConfirmation(string statusText, Action currentConfirmationAction, Action closeConfirmationAction)
    {
        confirmationTMP.text = statusText;

        confirmationPanelObj.alpha = 0f;
        confirmationPanelObj.gameObject.SetActive(true);

        LeanTween.alphaCanvas(confirmationPanelObj, 1f, 0.25f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() => 
        {
            gameManager.CanUseButtons = true;
        });

        currentAction = currentConfirmationAction;
        closeAction = closeConfirmationAction;
    }

    public void ConfirmedAction()
    {
        //GameManager.Instance.Sound.SetSFXAudio(acceptClip);


        LeanTween.alphaCanvas(confirmationPanelObj, 0f, 0.25f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
        {
            confirmationPanelObj.gameObject.SetActive(false);
            confirmationTMP.text = "";

            if (currentAction != null)
            {
                currentAction();
                currentAction = null;
            }
        });
    }

    public void CloseConfirmationAction()
    {
        //GameManager.Instance.Sound.SetSFXAudio(backClip);

        LeanTween.alphaCanvas(confirmationPanelObj, 0f, 0.25f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
        {
            confirmationPanelObj.gameObject.SetActive(false);
            confirmationTMP.text = "";

            if (closeAction != null)
            {
                closeAction();
                closeAction = null;

            }
        });
    }

    #endregion

    #region ERROR

    public void ShowError(string statusText, Action closeConfirmationAction)
    {
        errorTMP.text = statusText;

        errorPanelObj.alpha = 0f;
        errorPanelObj.gameObject.SetActive(true);

        LeanTween.alphaCanvas(errorPanelObj, 1f, 0.25f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
        {
            gameManager.CanUseButtons = true;
        });

        closeAction = closeConfirmationAction;
    }

    public void CloseErrorAction()
    {
        //GameManager.Instance.Sound.SetSFXAudio(backClip);

        LeanTween.alphaCanvas(errorPanelObj, 0f, 0.25f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
        {
            gameManager.CanUseButtons = true;
            errorPanelObj.gameObject.SetActive(false);
            errorTMP.text = "";
            if (closeAction != null)
            {
                closeAction();
                closeAction = null;
            }
        });
    }


    #endregion
}
