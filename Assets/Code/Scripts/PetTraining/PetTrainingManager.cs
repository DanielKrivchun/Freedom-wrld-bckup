using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetTrainingManager : MonoBehaviour
{
    [Header("Pet Data Reference")]
    public PetDataRef petDataRef;

    [Header("Pet Train Panel UI")]
    public GameObject petTrainPanel;
    [Space]
    public Button runningTrainingSelectBtn;
    public Button climbingTrainingSelectBtn;
    public Button swimmingTrainingSelectBtn;
    public Button flyingTrainingSelectBtn;
    public Button intelligenceTrainingSelectBtn;

    [Space]
    public List<Image> trainingSelectBtnImg;
    public List<Text> trainingSelectBtnTxt;
    public List<GameObject> trainingStatsMain;
    public GameObject notSelectedTrainingMain;
    public GameObject startTrainingBtn;

    [Header("Ongoing Training Popup UI")]
    public GameObject ongoingTrainingPopup;
    public Text ongoingTrainingMsgTxt;
    public Text ongoingTrainingTimerTxt;

    [Header("Completed Training Panel UI")]
    public GameObject completedTrainingPanel;
    public Text completedTrainingMsgTxt;
    public Text earnedStatsTxt;
    public Text niceWorkTxt;

    [Header("Training Data")]
    [SerializeField] private PetTrainingData runningTrainingData;
    [SerializeField] private PetTrainingData climbingTrainingData;
    [SerializeField] private PetTrainingData swimmingTrainingData;
    [SerializeField] private PetTrainingData flyingTrainingData;
    [SerializeField] private PetTrainingData inteligenceTrainingData;

    private PetTrainingData currentTrainingData;

    [Space]
    public PetTraining selectedTraining;

    public bool startTimer;
    public float trainingTimer;

    // Total seconds of 1 hour for hours calculation
    static float oneHourSeconds = 3600f;
    // Total seconds of 1 minute for minutes calculation
    static float oneMinuteSeconds = 60f;


    private void Start()
    {
        runningTrainingSelectBtn.onClick.AddListener(() => OnClickOfTrainingSelectBtn(0, PetTraining.Running));
        climbingTrainingSelectBtn.onClick.AddListener(() => OnClickOfTrainingSelectBtn(1, PetTraining.Climbing));
        swimmingTrainingSelectBtn.onClick.AddListener(() => OnClickOfTrainingSelectBtn(2, PetTraining.Swimming));
        flyingTrainingSelectBtn.onClick.AddListener(() => OnClickOfTrainingSelectBtn(3, PetTraining.Flying));
        intelligenceTrainingSelectBtn.onClick.AddListener(() => OnClickOfTrainingSelectBtn(4, PetTraining.Intelligence));
    }

    public void ShowPetTrainPanel()
    {
        //If any training going on then can't open Pet Train Panel
        if(!ongoingTrainingPopup.activeInHierarchy)
        {
            petTrainPanel.SetActive(true);
        }
    }

    public void OnClickOfTrainingSelectBtn(int index, PetTraining petTraining)
    {
        for (int i = 0; i < trainingStatsMain.Count; i++)
        {
            if(i == index)
            {
                trainingSelectBtnImg[i].color = Color.green;
                trainingSelectBtnTxt[i].text = "Selected";
                trainingStatsMain[i].SetActive(true);
            }
            else
            {
                trainingSelectBtnImg[i].color = Color.yellow;
                trainingSelectBtnTxt[i].text = "Select";
                trainingStatsMain[i].SetActive(false);
            }
        }

        selectedTraining = petTraining;
        notSelectedTrainingMain.SetActive(false);
        startTrainingBtn.SetActive(true);
    }

    public void OnClickOfStartTraining()
    {
        SetSelectedTrainingData();

        ongoingTrainingMsgTxt.text = "<b>" + petDataRef.petData.petname + "\n" + selectedTraining.ToString() + " Training</b> \nIn Session";
        petTrainPanel.SetActive(false);
        ongoingTrainingPopup.SetActive(true);
    }

    private void SetSelectedTrainingData()
    {
        switch(selectedTraining)
        {
            case PetTraining.Running:
                currentTrainingData = runningTrainingData;
                break;

                case PetTraining.Climbing:
                currentTrainingData = climbingTrainingData;
                break;

                case PetTraining.Swimming:
                currentTrainingData = swimmingTrainingData;
                break;

                case PetTraining.Flying:
                currentTrainingData = flyingTrainingData;
                break;

                case PetTraining.Intelligence:
                currentTrainingData = inteligenceTrainingData;
                break;
        }

        trainingTimer = currentTrainingData.trainingTime;
        startTimer = true;
    }

    private void Update()
    {
        //Sleep
        if (startTimer)
        {
            StartTrainingTimer();
        }
    }

    private void StartTrainingTimer()
    {
        if (startTimer)
        {
            if (trainingTimer > 0)
            {
                trainingTimer -= Time.deltaTime;
                UpdateTimer(trainingTimer);
            }
            else
            {
                Debug.Log("Trainig Time is over!");
                startTimer = false;

                ShowTrainingCompletedUI();
            }
        }
    }

    void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float hours = Mathf.FloorToInt(currentTime / 3600);
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        ongoingTrainingTimerTxt.text = "Time Remaining:\n<b>" + string.Format("{0:0}:{1:00}:{2:00}", hours, minutes, seconds) + "</b>";
    }

    void ShowTrainingCompletedUI()
    {
        ongoingTrainingPopup.SetActive(false);
        completedTrainingPanel.SetActive(true);

        float hours = Mathf.FloorToInt(currentTrainingData.trainingTime / 3600);
        float minutes = Mathf.FloorToInt(currentTrainingData.trainingTime / 60);
        float seconds = Mathf.FloorToInt(currentTrainingData.trainingTime % 60);

        /*string trainingTime = "";
        //If time is in hours
        if (currentTrainingData.trainingTime >= oneHourSeconds)
        {
            trainingTime = string.Format("{0:0.00}", currentTrainingData.trainingTime / oneHourSeconds) + " hours";
        }
        //Or time is in minutes
        else
        {
            //trainingTime = string.Format("{0:0.00}", currentTrainingData.trainingTime / oneMinuteSeconds) + " minutes";
            trainingTime = (currentTrainingData.trainingTime / oneMinuteSeconds).ToString() + " minutes";
        }*/

        completedTrainingMsgTxt.text = petDataRef.petData.petname + " spent " + hours + " hours " + minutes + " minutes " + seconds + " seconds " 
                                        + "\ntraining their " + selectedTraining.ToString() + " skills!";

        earnedStatsTxt.text = "• " + currentTrainingData.xP.ToString() + " XP\n"
                            + "• " + currentTrainingData.coins.ToString() + " Coins\n"
                            + "• " + currentTrainingData.trainingStatValue.ToString() + " " + selectedTraining.ToString() + "\n"
                            + "• " + currentTrainingData.happinessStatValue.ToString() + " Happiness\n"
                            + "• " + currentTrainingData.cleanlinessStatValue.ToString() + " Cleanliness\n"
                            + "• " + currentTrainingData.hungerStatValue.ToString() + " Hunger\n"
                            + "• " + currentTrainingData.energyStatValue.ToString() + " Energy\n";

        niceWorkTxt.text = "Nice work, " + petDataRef.petData.petname + "!";

        //NEED TO SET DATA

        currentTrainingData = null;
    }


}
