using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizController : MonoBehaviour
{
    private event EventHandler ChangeAnswer;
    public event EventHandler OnChangeAnswer
    {
        add
        {
            if (ChangeAnswer == null || !ChangeAnswer.GetInvocationList().Contains(value))
                ChangeAnswer += value;
        }
        remove { ChangeAnswer -= value; }
    }

    //  =====================================

    [SerializeField] private GameManager gameManager;
    [SerializeField] private AppStateManager appState;
    [SerializeField] private ComponentSystemController componentSystem;
    [SerializeField] private ErrorController errorController;
    [SerializeField] private GameObject loadingNoBG;

    [Header("EXAM")]
    [SerializeField] private TextMeshProUGUI pageInformationTMP;
    [SerializeField] private TextMeshProUGUI questionTMP;
    [SerializeField] private Image questionImage;
    [SerializeField] private Button previousBtn;
    [SerializeField] private Button nextBtn;
    [SerializeField] private List<Image> answerBtnImg;
    [SerializeField] private List<TextMeshProUGUI> answersTMP;
    [SerializeField] private GameObject checkquizBtnObj;

    [Header("QUIZ DATA")]
    [SerializeField] private QuizData quizData;

    [Header("DEBUGGER")]
    [SerializeField] private int currentIndex;
    [ReadOnly] [SerializeField] private QuizData quizDataTemp;

    //  ======================================

    Dictionary<int, QuizTempContent> quizContent = new Dictionary<int, QuizTempContent>();
    Dictionary<int, string> answerContent = new Dictionary<int, string>();
    Dictionary<int, int> answerIndexContent = new Dictionary<int, int>();

    //  ======================================

    private void Start()
    {
        ChangeAnswer += AnswerChange;
    }

    private void OnDisable()
    {
        ChangeAnswer -= AnswerChange;
    }

    private void Update()
    {
        UseKeyboardNextPrevious();
    }

    private void AnswerChange(object sender, EventArgs e)
    {
        ResetColorAnswers();
    }

    private void UseKeyboardNextPrevious()
    {
        if (appState.GetCurrentAppState != AppStateManager.AppState.QUIZ) return;

        if (Input.GetKeyUp(KeyCode.LeftArrow) && currentIndex > 0) NextPreviousButton(false);
        else if (Input.GetKeyUp(KeyCode.RightArrow) && currentIndex < quizContent.Count - 1) NextPreviousButton(true);
    }

    private void ResetSettings()
    {
        if (appState.GetCurrentAppState != AppStateManager.AppState.QUIZ) return;

        currentIndex = 0;
        questionTMP.text = "";
        questionImage.sprite = null;
        quizContent.Clear();
        answerContent.Clear();
        answerIndexContent.Clear();
    }

    public IEnumerator ShuffleQuestions(Action action = null)
    {
        Dictionary<int, QuizTempContent> tempQuizContent = new Dictionary<int, QuizTempContent>();
        Dictionary<int, QuizTempContent> shuffleTempQuizContent = new Dictionary<int, QuizTempContent>();

        //  ADDING TEMP
        for (int a = 0; a < quizData[componentSystem.CurrentAirplaneType].Count; a++)
        {
            tempQuizContent.Add(a + 1, new QuizTempContent()
            {
                quizAnswers = new List<string>()
                {
                    quizData[componentSystem.CurrentAirplaneType][a + 1].choiceOne,
                    quizData[componentSystem.CurrentAirplaneType][a + 1].choiceTwo,
                    quizData[componentSystem.CurrentAirplaneType][a + 1].choiceThree,
                    quizData[componentSystem.CurrentAirplaneType][a + 1].answer,
                },
                answer = quizData[componentSystem.CurrentAirplaneType][a + 1].answer,
                question = quizData[componentSystem.CurrentAirplaneType][a + 1].question,
                questionSprite = quizData[componentSystem.CurrentAirplaneType][a + 1].questionSprite
            });;

            tempQuizContent[a + 1].quizAnswers.Shuffle();

            yield return null;
        }

        shuffleTempQuizContent = tempQuizContent.Shuffle();

        for (int a = 0; a < tempQuizContent.Count; a++)
        {
            quizContent.Add(a + 1, shuffleTempQuizContent.ElementAt(a).Value);

            yield return null;
        }

        loadingNoBG.SetActive(false);
        action?.Invoke();
        CheckContent();
    }

    private void CheckContent()
    {
        if (appState.GetCurrentAppState != AppStateManager.AppState.QUIZ) return;

        pageInformationTMP.text = "PAGE " + (currentIndex + 1).ToString() + " OUT OF " + quizContent.Count;

        questionTMP.text = quizContent[currentIndex + 1].question.Replace("\r","");

        if (quizContent[currentIndex + 1].questionSprite != null)
        {
            questionImage.sprite = quizContent[currentIndex + 1].questionSprite;
            questionImage.gameObject.SetActive(true);
        }
        else
            questionImage.gameObject.SetActive(false);

        for(int a = 0; a < answersTMP.Count; a++)
        {
            answerBtnImg[a].color = new Color(1, 1, 1, 1);
            answersTMP[a].text = quizContent[currentIndex + 1].quizAnswers[a];
        }

        ResetColorAnswers();

        CheckNextPreviousButton();
    }

    private void CheckNextPreviousButton()
    {
        if (quizContent.Count <= 1)
        {
            previousBtn.interactable = false;
            nextBtn.interactable = false;
            checkquizBtnObj.SetActive(true);
        }
        else if (currentIndex == 0)
        {
            previousBtn.interactable = false;
            nextBtn.interactable = true;
            checkquizBtnObj.SetActive(false);
        }
        else if (currentIndex > 0 && currentIndex < quizContent.Count - 1)
        {
            previousBtn.interactable = true;
            nextBtn.interactable = true;
            checkquizBtnObj.SetActive(false);
        }
        else if (currentIndex >= quizContent.Count - 1)
        {
            previousBtn.interactable = true;
            nextBtn.interactable = false;
            checkquizBtnObj.SetActive(true);
        }

        gameManager.CanUseButtons = true;
    }

    private void ResetColorAnswers()
    {
        for (int a = 0; a < answerBtnImg.Count; a++)
            answerBtnImg[a].color = new Color(1f, 1f, 1f, 1f);

        if (answerIndexContent.ContainsKey(currentIndex + 1))
            answerBtnImg[answerIndexContent[currentIndex + 1]].color = new Color(0.1951762f, 0.9622642f, 0.2903914f, 1);
    }

    private IEnumerator CountAnswers()
    {
        int tempScore = 0;

        if (answerContent.Count > 0)
        {
            for (int a = 0; a < quizContent.Count; a++)
            {
                if (answerContent.ContainsKey(a + 1))
                {
                    if (answerContent[a + 1] == quizContent[a + 1].answer)
                        tempScore++;
                }

                yield return null;
            }
        }

        PlayerPrefs.SetString(((int)componentSystem.CurrentAirplaneType).ToString(), "1");
        PlayerPrefs.SetInt(((int)componentSystem.CurrentAirplaneType).ToString(), tempScore);

        loadingNoBG.SetActive(false);

        errorController.ShowError("You have a score of " + tempScore + " out of " + quizContent.Count, () => 
        {
            ResetSettings();
            appState.Back = true;
            appState.RemoveAppStateHistory();
        });
    }

    #region BUTTONS

    public void BackButton()
    {
        if (!gameManager.CanUseButtons) return;

        gameManager.CanUseButtons = false;

        errorController.ShowConfirmation("Are you sure you want to go back?", () => 
        {
            ResetSettings();
            appState.Back = true;
            appState.RemoveAppStateHistory();
        }, null);
    }

    public void CheckAnswer()
    {
        if (currentIndex + 1 >= quizContent.Count)
        {
            errorController.ShowConfirmation("Are you sure to submit your answers?", () => 
            {
                loadingNoBG.SetActive(true);
                gameManager.CanUseButtons = false;
                StartCoroutine(CountAnswers());
            }, null);
        }
        else
        {
            errorController.ShowConfirmation("You still didn't answer all the questions! Are you sure you want to submit it?", () =>
            {
                loadingNoBG.SetActive(true);
                gameManager.CanUseButtons = false;
                StartCoroutine(CountAnswers());
            }, null);
        }
    }

    public void NextPreviousButton(bool value)
    {
        if (!gameManager.CanUseButtons) return;

        gameManager.CanUseButtons = false;

        if (value)
            currentIndex++;
        else
            currentIndex--;

        CheckContent();
    }

    public void ChooseAnswer(int index)
    {
        if (answerContent.ContainsKey(currentIndex + 1))
            answerContent[currentIndex + 1] = answersTMP[index].text;
        else
            answerContent.Add(currentIndex + 1, answersTMP[index].text);

        if (answerIndexContent.ContainsKey(currentIndex + 1))
            answerIndexContent[currentIndex + 1] = index;
        else
            answerIndexContent.Add(currentIndex + 1, index);

        ChangeAnswer?.Invoke(this, EventArgs.Empty);
    }

    #endregion
}

[Serializable]
public class QuizData : SerializableDictionary<ComponentSystemController.AirplaneType, QuizNumber> { }

[Serializable]
public class QuizNumber : SerializableDictionary<int, QuizContent> { }

[Serializable]
public class QuizContent
{
    [TextArea] public string question;
    public Sprite questionSprite;
    public string choiceOne;
    public string choiceTwo;
    public string choiceThree;
    public string answer;
}

[Serializable]
public class QuizTempContent
{
    public List<string> quizAnswers;
    public string answer;
    public string question;
    public Sprite questionSprite;
}

public static class DictionaryExtensions
{
    public static Dictionary<TKey, TValue> Shuffle<TKey, TValue>(
       this Dictionary<TKey, TValue> source)
    {
        System.Random r = new System.Random();
        return source.OrderBy(x => r.Next())
           .ToDictionary(item => item.Key, item => item.Value);
    }
}