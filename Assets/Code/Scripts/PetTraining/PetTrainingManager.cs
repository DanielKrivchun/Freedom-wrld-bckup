using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetTrainingManager : MonoBehaviour
{
    [Header("Pet Data Reference")]
    public PetDataRef petDataRef;

    [Space]
    public PetCareStateManager petCareStateManager;
    public PetCareUIManager petCareUIManager;
    public GetServerTime getServerTime;

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


    [Space]
    public PetTraining selectedTraining;

    public bool startTimer;
    public float trainingTimer;

    private DateTime serverTimeNow;
    private PetTrainingData currentTrainingData;

    private void Start()
    {
        runningTrainingSelectBtn.onClick.AddListener(() => OnClickOfTrainingSelectBtn(0, PetTraining.Running));
        climbingTrainingSelectBtn.onClick.AddListener(() => OnClickOfTrainingSelectBtn(1, PetTraining.Climbing));
        swimmingTrainingSelectBtn.onClick.AddListener(() => OnClickOfTrainingSelectBtn(2, PetTraining.Swimming));
        flyingTrainingSelectBtn.onClick.AddListener(() => OnClickOfTrainingSelectBtn(3, PetTraining.Flying));
        intelligenceTrainingSelectBtn.onClick.AddListener(() => OnClickOfTrainingSelectBtn(4, PetTraining.Intelligence));
    }

    #region TRAINING SELECTION UI
    public void ShowPetTrainPanel()
    {
        //If any training going on then can't open Pet Train Panel
        if(!ongoingTrainingPopup.activeInHierarchy || !petDataRef.petData.ongoingTrainingData.isTraining)
        {
            petTrainPanel.SetActive(true);
        }
        else
        {
            petCareUIManager.ShowNotificationUI("Sorry, you can't access this while your pet is training");
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
        switch (selectedTraining)
        {
            case PetTraining.Running:
                currentTrainingData = runningTrainingData;
                petDataRef.petData.ongoingTrainingData.ongoingTrainingName = "Running";
                break;

            case PetTraining.Climbing:
                currentTrainingData = climbingTrainingData;
                petDataRef.petData.ongoingTrainingData.ongoingTrainingName = "Climbing";
                break;

            case PetTraining.Swimming:
                currentTrainingData = swimmingTrainingData;
                petDataRef.petData.ongoingTrainingData.ongoingTrainingName = "Swimming";
                break;

            case PetTraining.Flying:
                currentTrainingData = flyingTrainingData;
                petDataRef.petData.ongoingTrainingData.ongoingTrainingName = "Flying";
                break;

            case PetTraining.Intelligence:
                currentTrainingData = inteligenceTrainingData;
                petDataRef.petData.ongoingTrainingData.ongoingTrainingName = "Intelligence";
                break;
        }

        petDataRef.petData.ongoingTrainingData.isTraining = true;
        petDataRef.petData.ongoingTrainingData.ongoingTraining = selectedTraining;
        getServerTime.GetCurrentTime(timeNow => { petDataRef.petData.ongoingTrainingData.trainingStartTime = timeNow.ToString(); });

        ShowOngoingTrainingUIWithTime();
        SetTrainingTimer(currentTrainingData.trainingTime);
    }
    #endregion

    #region ONGOING TRAINING UI
    void ShowOngoingTrainingUIWithTime()
    {
        ongoingTrainingMsgTxt.text = "<b>" + petDataRef.petData.petname + "\n" + selectedTraining.ToString() + " Training</b> \nIn Session";
        petTrainPanel.SetActive(false);
        ongoingTrainingPopup.SetActive(true);
    }

    void SetTrainingTimer(float totalTime)
    {
        trainingTimer = totalTime;
        startTimer = true;
    }
    #endregion

    #region TRAINING TIMER
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
    #endregion

    #region TRAINING COMPLETION UI AND EARNED STATS
    void ShowTrainingCompletedUI()
    {
        ongoingTrainingPopup.SetActive(false);
        completedTrainingPanel.SetActive(true);

        float hours = Mathf.FloorToInt(currentTrainingData.trainingTime / 3600);
        float minutes = Mathf.FloorToInt(currentTrainingData.trainingTime / 60);
        float seconds = Mathf.FloorToInt(currentTrainingData.trainingTime % 60);

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

        SetTrainigEarnedStats();
    }

    public void SetTrainigEarnedStats()
    {
        //XP
        petCareStateManager.IncreaseXP(currentTrainingData.xP);

        //Coins
        //Needs add coins of Current Training Data

        //Training Stats
        if(currentTrainingData == runningTrainingData)
        {
            petDataRef.petData.running += currentTrainingData.trainingStatValue;
        }
        else if(currentTrainingData == climbingTrainingData)
        {
            petDataRef.petData.climbing += currentTrainingData.trainingStatValue;
        }
        else if(currentTrainingData == swimmingTrainingData)
        {
            petDataRef.petData.swimming += currentTrainingData.trainingStatValue;
        }
        else if (currentTrainingData == flyingTrainingData)
        {
            petDataRef.petData.flying += currentTrainingData.trainingStatValue;
        }
        else if (currentTrainingData == inteligenceTrainingData)
        {
            petDataRef.petData.intelligence += currentTrainingData.trainingStatValue;
        }

        //Wellbeing Stats
        petCareStateManager.ManageHappinessDataFiller(currentTrainingData.happinessStatValue);
        petCareStateManager.ManageCleanlinessDataFiller(currentTrainingData.cleanlinessStatValue);
        petCareStateManager.ManageHungerDataFiller(currentTrainingData.hungerStatValue);
        petCareStateManager.ManageEnergyDataFiller(currentTrainingData.energyStatValue);

        //Reset all data
        petDataRef.petData.ongoingTrainingData.isTraining = false;
        petDataRef.petData.ongoingTrainingData.ongoingTraining = PetTraining.NotSelected;
        petDataRef.petData.ongoingTrainingData.ongoingTrainingName = "None";
        currentTrainingData = null;
    }
    #endregion

    #region CHECK FOR ONGOING TRAINING
    public void CheckForAnyOngoingTraining()
    {
        switch (petDataRef.petData.ongoingTrainingData.ongoingTraining)
        {
            case PetTraining.Running:
                selectedTraining = PetTraining.Running;
                currentTrainingData = runningTrainingData;
                break;

                case PetTraining.Climbing:
                selectedTraining = PetTraining.Climbing;
                currentTrainingData = climbingTrainingData;
                break;

                case PetTraining.Swimming:
                selectedTraining = PetTraining.Swimming;
                currentTrainingData = swimmingTrainingData;
                break;

            case PetTraining.Flying:
                selectedTraining = PetTraining.Flying;
                currentTrainingData = flyingTrainingData;
                break;

            case PetTraining.Intelligence:
                selectedTraining = PetTraining.Intelligence;
                currentTrainingData = inteligenceTrainingData;
                break;
        }

        if (CheckTimeDiffWithCurrentTimeInSec(petDataRef.petData.ongoingTrainingData.trainingStartTime) > currentTrainingData.trainingTime)
        {
            Debug.Log("Training completed!");
            ShowTrainingCompletedUI();
        }
        else
        {
            Debug.Log("Training Time is Not over yet!");
            ShowOngoingTrainingUIWithTime();
            trainingTimer = currentTrainingData.trainingTime - CheckTimeDiffWithCurrentTimeInSec(petDataRef.petData.ongoingTrainingData.trainingStartTime);
            startTimer = true;
        }
    }

    public float CheckTimeDiffWithCurrentTimeInSec(string lastTime)
    {
        getServerTime.GetCurrentTime(timeNow => { serverTimeNow = timeNow; });
        return (float)(serverTimeNow - DateTime.Parse(lastTime)).TotalSeconds;
    }
    #endregion
}
