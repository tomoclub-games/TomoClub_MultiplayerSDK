using UnityEngine.UI;
using UnityEngine;
using TomoClub.Util;

public class TutorialManager : MonoBehaviour
{

    [Header("Tutorial Settings")]
    [Tooltip("Time after which the tutorial automatically shifts")]
    [SerializeField] int transitionTime;

    [Header("Tutorial UI")]
    [SerializeField] Image Tutorial_BG;
    [SerializeField] Image Image_Tutorial;
    [SerializeField] Sprite[] Sprites_Tutorial;
    [SerializeField] GameObject[] GO_TutorialPageNavig;

    private TimerUp tutorialTimer;
    private int currentTutorialPage = 0;

    private void Awake()
    {
        //Player side initializations 
        Tutorial_Init();
    }

    private void Start()
    {
        tutorialTimer.StartTimer();
    }

    private void OnEnable()
    {
        tutorialTimer.TimerCompleted += Tutorial_NextButton;
    }

    private void OnDisable()
    {
        tutorialTimer.TimerCompleted -= Tutorial_NextButton;
    }


    private void Update()
    {
        tutorialTimer.UpdateTimer();
    }


    private void Tutorial_Init()
    {
        Image_Tutorial.sprite = Sprites_Tutorial[0];

        //Init setup for the tutorial menu
        for (int i = 0; i < GO_TutorialPageNavig.Length; i++)
        {
            if (i < Sprites_Tutorial.Length) GO_TutorialPageNavig[i].SetActive(true);
            else GO_TutorialPageNavig[i].SetActive(false);
        }

        // A new timer up counter
        tutorialTimer = new TimerUp(transitionTime);
    }

    public void SetTutorialBG(float alpha)
    {
        Color color = Tutorial_BG.color;
        color.a = alpha;
    }


    #region TUTORIAL UI

    //Update tutorial to previous
    public void Tutorial_PrevButton()
    {
        GO_TutorialPageNavig[currentTutorialPage].transform.GetChild(0).gameObject.SetActive(false);

        currentTutorialPage--;
        if (currentTutorialPage == -1) currentTutorialPage = Sprites_Tutorial.Length - 1;

        Image_Tutorial.sprite = Sprites_Tutorial[currentTutorialPage];
        GO_TutorialPageNavig[currentTutorialPage].transform.GetChild(0).gameObject.SetActive(true);

        tutorialTimer.RestartTimer();
    }

    //Update tutorial to next
    public void Tutorial_NextButton()
    {
        GO_TutorialPageNavig[currentTutorialPage].transform.GetChild(0).gameObject.SetActive(false);
        currentTutorialPage++;
        if (currentTutorialPage == Sprites_Tutorial.Length) currentTutorialPage = 0;

        Image_Tutorial.sprite = Sprites_Tutorial[currentTutorialPage];
        GO_TutorialPageNavig[currentTutorialPage].transform.GetChild(0).gameObject.SetActive(true);

        tutorialTimer.RestartTimer();
    }

    #endregion
}
