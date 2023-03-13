using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopicController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AppStateManager appState;
    [SerializeField] private ComponentSystemController componentSystem;
    [SerializeField] private SoundManager soundManager;

    [field: Header("CONTENTS")]
    [field: SerializeField] public TopicData AirplaneTopicDataList { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI contentTMP;

    [Header("PICTURE CONTENT")]
    [SerializeField] private GameObject miniLoaderPicture;
    [SerializeField] private GameObject pictureContentParentObj;
    [SerializeField] private Transform pictureContentTF;
    [SerializeField] private Scrollbar pictureContentScrollbar;

    [Header("BUTTONS")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    [Header("DEBUGGER")]
    [ReadOnly] [SerializeField] private int pageCounter;
    [ReadOnly] [SerializeField] private int pictureCounter;

    //  ==============================

    Coroutine pictureContentCoroutine;

    //  ==============================

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
        ResetTopic();
        TopicChecker();
    }

    private void ResetTopic()
    {
        if (appState.GetCurrentAppState != AppStateManager.AppState.TOPIC) return;

        pageCounter = 0;
    }

    private void TopicChecker()
    {
        if (appState.GetCurrentAppState != AppStateManager.AppState.TOPIC) return;

        if (pictureContentCoroutine != null)
        {
            miniLoaderPicture.SetActive(true);
            pictureContentParentObj.SetActive(false);
            StopCoroutine(pictureContentCoroutine);
        }

        ShowContent(AirplaneTopicDataList);
        pictureContentCoroutine = StartCoroutine(ShowPictureContent());
    }

    IEnumerator ShowPictureContent()
    {
        pictureContentScrollbar.value = 1f;

        for (var i = pictureContentTF.childCount - 1; i >= 0; i--)
        {
            Destroy(pictureContentTF.GetChild(i).gameObject);

            yield return null;
        }

        if (AirplaneTopicDataList[componentSystem.CurrentAirplaneType][componentSystem.CurrentSystems][pageCounter].topicSprites.Count <= 0)
        {
            miniLoaderPicture.SetActive(false);
            yield break;
        }

        for (int a = 0; a < AirplaneTopicDataList[componentSystem.CurrentAirplaneType][componentSystem.CurrentSystems][pageCounter].topicSprites.Count; a++)
        {

            Sprite contentSprite = AirplaneTopicDataList[componentSystem.CurrentAirplaneType][componentSystem.CurrentSystems][pageCounter].topicSprites[a];

            GameObject newObject = new GameObject("PictureContent" + a);
            newObject.transform.parent = pictureContentTF;

            RectTransform objRT = newObject.AddComponent<RectTransform>();
            Image objImg = newObject.AddComponent<Image>();

            objRT.localPosition = new Vector3(0f, 0f, 0f);
            objRT.sizeDelta = new Vector2(contentSprite.rect.width, contentSprite.rect.height);
            objRT.localScale = new Vector2(1f, 1f);
            objRT.localRotation = Quaternion.identity;
            objImg.sprite = contentSprite;

            yield return null;
        }

        miniLoaderPicture.SetActive(false);
        pictureContentParentObj.SetActive(true);
    }

    private void ShowContent(TopicData data)
    {
        if (appState.GetCurrentAppState != AppStateManager.AppState.TOPIC) return;

        contentTMP.text = data[componentSystem.CurrentAirplaneType][componentSystem.CurrentSystems][pageCounter].content;

        if (data[componentSystem.CurrentAirplaneType][componentSystem.CurrentSystems][pageCounter].narration != null)
            soundManager.PlayVoiceNarration(data[componentSystem.CurrentAirplaneType][componentSystem.CurrentSystems][pageCounter].narration);

        CheckButtons(data);
    }

    private void CheckButtons(TopicData data)
    {
        if (data[componentSystem.CurrentAirplaneType][componentSystem.CurrentSystems].Values.Count <= 1)
        {
            previousButton.interactable = false;
            nextButton.interactable = false;
        }
        else if (pageCounter == 0)
        {
            previousButton.interactable = false;
            nextButton.interactable = true;
        }
        else if (pageCounter > 0 && pageCounter < data[componentSystem.CurrentAirplaneType][componentSystem.CurrentSystems].Values.Count -1 )
        {
            previousButton.interactable = true;
            nextButton.interactable = true;
        }
        else if (pageCounter >= data[componentSystem.CurrentAirplaneType][componentSystem.CurrentSystems].Values.Count - 1)
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

        soundManager.StopVoiceNarration();

        //NEXT
        if (value) 
            pageCounter++;
        else
            pageCounter--;

        TopicChecker();
    }

    public void BackButton()
    {
        if (!gameManager.CanUseButtons) return;

        gameManager.CanUseButtons = false;

        soundManager.StopVoiceNarration();

        appState.RemoveAppStateHistory();
    }

    public void MuteUnmute()
    {
        if (!gameManager.CanUseButtons) return;

        gameManager.CanUseButtons = false;

        if (soundManager.CurrentVolume == 1) soundManager.CurrentVolume = 0;
        else soundManager.CurrentVolume = 1;

        gameManager.CanUseButtons = true;
    }

    #endregion
}


//  ================================

[Serializable]
public class TopicData : SerializableDictionary<ComponentSystemController.AirplaneType, AirplaneSystems> { };

[Serializable]
public class AirplaneSystems : SerializableDictionary<ComponentSystemController.Systems, AirplaneTopicContent> { }

[Serializable]
public class AirplaneTopicContent : SerializableDictionary<int, AirplaneTopic> { }

[Serializable]
public class AirplaneTopic
{
    [TextArea] public string content;
    public AudioClip narration;
    public List<Sprite> topicSprites;
}
