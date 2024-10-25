using Beamable.InventoryService;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PetTrainingManager : MonoBehaviour
{
    [Header("Pet Data Reference")]
    public PetDataRef petDataRef;

    [Header("Script Ref")]
    public PetCareStateManager petCareStateManager;
    public PetCareUIManager petCareUIManager;
    public PetCareInputManager petInputManager;
    public PetCareCameraViewManager cameraViewManager;
    public GetServerTime getServerTime;
    public BeamableInventoryManager beamableInventoryManager;

    [Space]
    public GameObject petCareBtnHolder;
    public Transform player;

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
    public GameObject puzzleModel;

    [Header("Training Points")]
    public Transform runPoint;
    public Transform swimPoint;
    public Transform flyPoint;
    public Transform climbPoint;
    public Transform intelligencePoint;

    [Space]
    public PetTraining selectedTraining;

    [HideInInspector]
    public bool startTimer;
    [HideInInspector]
    public float trainingTimer;

    private DateTime serverTimeNow;
    private PetTrainingData currentTrainingData;
    private int _hours;
    private int _minutes;
    private int _seconds;

    #region BUTTON LISTENERS
    private void Start()
    {
        runningTrainingSelectBtn.onClick.AddListener(() => OnClickOfTrainingSelectBtn(0, PetTraining.Running));
        climbingTrainingSelectBtn.onClick.AddListener(() => OnClickOfTrainingSelectBtn(1, PetTraining.Climbing));
        swimmingTrainingSelectBtn.onClick.AddListener(() => OnClickOfTrainingSelectBtn(2, PetTraining.Swimming));
        flyingTrainingSelectBtn.onClick.AddListener(() => OnClickOfTrainingSelectBtn(3, PetTraining.Flying));
        intelligenceTrainingSelectBtn.onClick.AddListener(() => OnClickOfTrainingSelectBtn(4, PetTraining.Intelligence));
    }
    #endregion

    #region TRAINING SELECTION UI
    public void ShowPetTrainPanel()
    {
        //If any training going on then can't open Pet Train Panel
        if (!ongoingTrainingPopup.activeInHierarchy || !petDataRef.petData.ongoingTrainingData.isTraining)
        {
            petTrainPanel.SetActive(true);
            petCareStateManager.ResetPetCareTakingState();
            cameraViewManager.SetCameraTopView();
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
            if (i == index)
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
    #endregion

    #region START TRAINING
    public void OnClickOfStartTraining()
    {
        //Show popup if energy is low
        if (IsLowerEnergyForPetTraining())
        {
            petCareUIManager.ShowNotificationUI("Pet's energy is low for training!");
            return;
        }

        //Start training
        switch (selectedTraining)
        {
            case PetTraining.Running:
                petInputManager.SetDestinationPointForCareTakingOrTraining(runPoint.position, true);

                currentTrainingData = runningTrainingData;
                petDataRef.petData.ongoingTrainingData.ongoingTrainingName = "Running";
                break;

            case PetTraining.Climbing:
                petInputManager.SetDestinationPointForCareTakingOrTraining(climbPoint.position, true);

                currentTrainingData = climbingTrainingData;
                petDataRef.petData.ongoingTrainingData.ongoingTrainingName = "Climbing";
                break;

            case PetTraining.Swimming:
                petInputManager.SetDestinationPointForCareTakingOrTraining(swimPoint.position, true);

                currentTrainingData = swimmingTrainingData;
                petDataRef.petData.ongoingTrainingData.ongoingTrainingName = "Swimming";
                break;

            case PetTraining.Flying:
                petInputManager.SetDestinationPointForCareTakingOrTraining(flyPoint.position, true);

                currentTrainingData = flyingTrainingData;
                petDataRef.petData.ongoingTrainingData.ongoingTrainingName = "Flying";
                break;

            case PetTraining.Intelligence:
                petInputManager.SetDestinationPointForCareTakingOrTraining(intelligencePoint.position, true);

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

    //Checking for lower energy
    public bool IsLowerEnergyForPetTraining()
    {
        switch (selectedTraining)
        {
            case PetTraining.Running:
                return petDataRef.petData.energy <= Mathf.Abs(2 * runningTrainingData.energyStatValue);

            case PetTraining.Climbing:
                return petDataRef.petData.energy <= Mathf.Abs(2 * climbingTrainingData.energyStatValue);

            case PetTraining.Swimming:
                return petDataRef.petData.energy <= Mathf.Abs(2 * swimmingTrainingData.energyStatValue);

            case PetTraining.Flying:
                return petDataRef.petData.energy <= Mathf.Abs(2 * flyingTrainingData.energyStatValue);

            case PetTraining.Intelligence:
                return petDataRef.petData.energy <= Mathf.Abs(2 * inteligenceTrainingData.energyStatValue);

            default:
                return false;
        }
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

                SetPetBackToTrainingPoint();
                ShowTrainingCompletedUI();
            }
        }
    }

    void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        _hours = TimeSpan.FromSeconds(currentTime).Hours;
        _minutes = TimeSpan.FromSeconds(currentTime).Minutes;
        _seconds = TimeSpan.FromSeconds(currentTime).Seconds;


        ongoingTrainingTimerTxt.text = "Time Remaining:\n<b>" + string.Format("{0:0}:{1:00}:{2:00}", _hours, _minutes, _seconds) + "</b>";
    }
    #endregion

    #region MANAGE PET REACHED TO DESTINATION POINT
    public void PetReachedSelectedObjectDestination()
    {
        if (selectedTraining == PetTraining.Intelligence)
        {
            puzzleModel.SetActive(true);
        }

        petInputManager.NavigatePlayerAroundTrainingPath(selectedTraining);
    }
    #endregion

    #region STOP PARTICLE EFFECTS AND STOP PET DOING TRAINING
    void SetPetBackToTrainingPoint()
    {
        StartCoroutine(petInputManager.StopNavigating());

        switch (selectedTraining)
        {
            case PetTraining.Running:
                petInputManager.particleEffectsManager.StopRunningDirtEffect();
                break;

            case PetTraining.Swimming:
                petInputManager.particleEffectsManager.StopSwimmingWaterSplashEffect();
                break;

            case PetTraining.Flying:
                petInputManager.particleEffectsManager.StopFlyingWindEffect();
                break;

            case PetTraining.Intelligence:
                petInputManager.particleEffectsManager.StopPuzzleEffect();
                break;
        }
    }
    #endregion

    #region TRAINING COMPLETION UI AND EARNED STATS
    void ShowTrainingCompletedUI()
    {
        puzzleModel.SetActive(false);
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

        //Add Coins
        beamableInventoryManager.AddCurrency(currentTrainingData.coins);

        //Training Stats
        if (currentTrainingData == runningTrainingData)
        {
            petDataRef.petData.running += currentTrainingData.trainingStatValue;
        }
        else if (currentTrainingData == climbingTrainingData)
        {
            petDataRef.petData.climbing += currentTrainingData.trainingStatValue;
        }
        else if (currentTrainingData == swimmingTrainingData)
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
    public async Task CheckForAnyOngoingTraining()
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
                puzzleModel.SetActive(true);
                break;
        }

        float timeDiff = await CheckTimeDiffWithCurrentTimeInSec(petDataRef.petData.ongoingTrainingData.trainingStartTime);

        //Training completed
        if (timeDiff > currentTrainingData.trainingTime)
        {
            ShowTrainingCompletedUI();
        }
        //Training time is not over yet
        else
        {
            ShowOngoingTrainingUIWithTime();
            trainingTimer = currentTrainingData.trainingTime - await CheckTimeDiffWithCurrentTimeInSec(petDataRef.petData.ongoingTrainingData.trainingStartTime);
            startTimer = true;

            petInputManager.StartNavigatingPlayerAroundTrainingPath(selectedTraining);
        }
    }

    async Task<float> CheckTimeDiffWithCurrentTimeInSec(string lastTime)
    {
        serverTimeNow = await getServerTime.GetCurrentTimeTask();
        return (float)(serverTimeNow - DateTime.Parse(lastTime)).TotalSeconds;
    }
    #endregion
}
