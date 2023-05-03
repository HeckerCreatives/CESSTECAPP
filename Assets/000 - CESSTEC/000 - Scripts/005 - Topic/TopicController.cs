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
    [field: SerializeField] public AirplaneSystemStats SytemAirplaneStats { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI contentTMP;

    [Header("PICTURE CONTENT")]
    [SerializeField] private GameObject miniLoaderPicture;
    [SerializeField] private GameObject pictureContentParentObj;
    [SerializeField] private Transform pictureContentTF;

    [Header("BUTTONS")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextPictureBtn;
    [SerializeField] private Button previousPictureBtn;

    [Header("DEBUGGER")]
    [ReadOnly] [SerializeField] private int pageCounter;
    [ReadOnly] [SerializeField] private int pictureCounter;
    [ReadOnly][SerializeField] private int totalPictures;
    [ReadOnly][SerializeField] private int currentPictureIndex;
    [ReadOnly][SerializeField] private bool canNextPrevious;
 
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

    private void Update()
    {
        MoveIndexUsingKeyboard();
    }

    private void AppStateChange(object sender, EventArgs e)
    {
        ResetTopic();
        TopicChecker();
    }

    private void MoveIndexUsingKeyboard()
    {
        if (appState.GetCurrentAppState != AppStateManager.AppState.TOPIC) return;

        if (Input.GetKeyUp(KeyCode.LeftArrow) && pageCounter > 0)
            NextPreviousButton(false);
        else if (Input.GetKeyUp(KeyCode.RightArrow) && pageCounter < AirplaneTopicDataList[componentSystem.CurrentAirplaneType][componentSystem.CurrentSystems].Values.Count - 1)
            NextPreviousButton(true);
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
        if (pageCounter >= AirplaneTopicDataList[componentSystem.CurrentAirplaneType][componentSystem.CurrentSystems].Count - 1)
            SytemAirplaneStats[componentSystem.CurrentAirplaneType][componentSystem.CurrentSystems].isDone = true;

        ShowContent(AirplaneTopicDataList);
        pictureContentCoroutine = StartCoroutine(ShowPictureContent());
    }

    IEnumerator ShowPictureContent()
    {
        canNextPrevious = false;
        nextPictureBtn.gameObject.SetActive(false);
        previousPictureBtn.gameObject.SetActive(false);

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

            objImg.gameObject.SetActive(false);

            yield return null;
        }

        pictureContentTF.GetChild(0).gameObject.SetActive(true);
        totalPictures = AirplaneTopicDataList[componentSystem.CurrentAirplaneType][componentSystem.CurrentSystems][pageCounter].topicSprites.Count;
        currentPictureIndex = 0;
        CheckPictureButtons();

        miniLoaderPicture.SetActive(false);
        pictureContentParentObj.SetActive(true);
    }

    private void ShowContent(TopicData data)
    {
        if (appState.GetCurrentAppState != AppStateManager.AppState.TOPIC) return;

        contentTMP.text = data[componentSystem.CurrentAirplaneType][componentSystem.CurrentSystems][pageCounter].content.Replace("\r","");

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

    private void ShowPictures(bool value)
    {
        if (value)
        {
            pictureContentTF.GetChild(currentPictureIndex - 1).gameObject.SetActive(false);
            pictureContentTF.GetChild(currentPictureIndex).gameObject.SetActive(true);
        }
        else
        {
            pictureContentTF.GetChild(currentPictureIndex + 1).gameObject.SetActive(false);
            pictureContentTF.GetChild(currentPictureIndex).gameObject.SetActive(true);
        }
        CheckPictureButtons();
    }

    private void CheckPictureButtons()
    {
        if (totalPictures == 0 || totalPictures == 1)
        {
            nextPictureBtn.gameObject.SetActive(false);
            previousPictureBtn.gameObject.SetActive(false);
        }
        else
        {
            if (currentPictureIndex == 0)
            {
                nextPictureBtn.gameObject.SetActive(true);
                previousPictureBtn.gameObject.SetActive(false);
            }
            else if (currentPictureIndex > 0 && currentPictureIndex < totalPictures - 1)
            {
                nextPictureBtn.gameObject.SetActive(true);
                previousPictureBtn.gameObject.SetActive(true);
            }
            else if (currentPictureIndex >= totalPictures - 1)
            {
                nextPictureBtn.gameObject.SetActive(false);
                previousPictureBtn.gameObject.SetActive(true);
            }
        }

        canNextPrevious = true;
    }

    #region BUTTON

    public void NextPreviousPictureBtn(bool value)
    {
        if (!canNextPrevious) return;

        canNextPrevious = false;

        if (value)
            currentPictureIndex++;
        else
            currentPictureIndex--;

        ShowPictures(value);
    }

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
public class AirplaneSystemStats : SerializableDictionary<ComponentSystemController.AirplaneType, SystemStats> { }

[Serializable]
public class SystemStats : SerializableDictionary<ComponentSystemController.Systems, AirplaneSystemStatus> { }

[Serializable]
public class AirplaneSystemStatus
{
    [ReadOnly] public bool isDone;
}

[Serializable]
public class AirplaneTopic
{
    [TextArea] public string content;
    public AudioClip narration;
    public List<Sprite> topicSprites;
    public List<string> topicCitations;
}
