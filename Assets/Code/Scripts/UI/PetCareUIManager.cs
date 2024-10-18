using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Beamable.CloudSavingService;
using UnityEngine.SceneManagement;
using Beamable.InventoryService;
using Beamable.Server.Clients;
using TMPro;

public class PetCareUIManager : MonoBehaviour
{
    public static PetCareUIManager instance;

    [Header("Pet Care Data Reference")]
    public PetDataRef petDataRef;

    [Header("Extra Player Data Reference")]
    public ExtraPlayerDataListSO extraPlayerDataRef;

    [Space]
    public GameEventState petCareEvent;

    [Header("Loading UI")]
    public GameObject loadingCanvas;

    [Header("Pet Creation UI")]
    public GameObject petCreationPanelMain;
    public GameObject eggSelectionPanel;
    public GameObject welcomePanel;
    public GameObject setPetDetailsPanel;

    [Header("Tutorial UI")]
    public GameObject introTutorialPanel;
    public GameObject inventoryTutorialPanel;
    public GameObject showerTutorialPanel;
    public GameObject eatingTutorialPanel;

    [Header("Pet Welcome Panel UI")]
    public Image selectedEggPet;
    public List<Sprite> petImages;

    [Header("Set Pet Details Panel UI")]
    public InputField petInput;

    [Space]
    public Text abilityStatsTxt;
    public int maxAbilityStatLimit;

    [Space]
    public Slider runningSliderPetCreation;
    public Text runningPetCreationStatTxt;

    [Space]
    public Slider climbingSliderPetCreation;
    public Text climbingPetCreationStatTxt;

    [Space]
    public Slider flyingSliderPetCreation;
    public Text flyingPetCreationStatTxt;

    [Space]
    public Slider swimmingSliderPetCreation;
    public Text swimmingPetCreationStatTxt;

    [Space]
    public Text chanceStatsTxt;
    public int maxChanceStatLimit;

    [Space]
    public Slider intelligenceSliderPetCreation;
    public Text intelligencePetCreationStatTxt;

    [Space]
    public Slider luckSliderPetCreation;
    public Text luckPetCreationStatTxt;

    [Space]
    public Text noticeTxt;

    [Header("Wellbeing Button UI")]
    public Sprite wellbeingBtnsOn;
    public Sprite wellbeingBtnsOff;
    public Image wellbeingBtnImg;
    public GameObject careBtnHolder;

    [Header("Pet Care Screen UI")]
    public Button happyBtn;
    public Button eatBtn;
    public Button cleanBtn;
    public Button sleepBtn;

    [Space]
    public Slider happyFillSlider;
    public Slider hungerFillSlider;
    public Slider cleanFillSlider;
    public Slider energyFillSlider;

    [Header("Pet Stats Panel UI")]
    public GameObject petStatPanel;
    public Text petNameTxt;
    public Text petRankTxt;

    [Header("Wellbeing Stat Slider")]
    public Slider happyFillPanelSlider;
    public Slider hungerFillPanelSlider;
    public Slider cleanFillPanelSlider;
    public Slider energyFillPanelSlider;

    [Header("Athletics Stat Text")]
    public Text runningStatTxt;
    public Text climbingStatTxt;
    public Text flyingStatTxt;
    public Text swimmingStatTxt;

    [Header("Chance Stat Text")]
    public Text intelligenceStatTxt;
    public Text luckStatTxt;

    [Header("Pet Sick Label UI")]
    public GameObject sickLabel;
    public Text sickLabelTxt;

    [Header("Notification UI")]
    public GameObject notificationPopup;
    public Text notificationMsgTxt;

    [Header("Pet Death Screen UI")]
    public GameObject petDeathPanel;
    public Text petDeathReasonTxt;

    [Header("Rank Up UI")]
    public GameObject rankUpPanel;
    public Text rankUpMsgTxt;

    [Header("Welcome Back From Race UI")]
    public GameObject welcomeBackPanel;
    public Text earnedStatsTxt;
    public Text niceWorkTxt;

    [Header("Inventory UI")]
    public GameObject inventoryPanel;

    [Header("Sleep UI")]
    public GameObject sleepPanel;
    [Header("Coins UI")]
    public TextMeshProUGUI coinText;
    [Header("4th Pet")]
    public GameObject tempImage;

    [Header("Script Ref")]
    public PetCareStateManager petStateManager;
    public BeamableInventoryManager beamableInventoryManager;
    //public ExtraPlayerDataManager extraPlayerDataManager;
    public PetCareCameraViewManager cameraViewManager;

    private ExtraPlayerDataServiceClient _ExtraPlayerDataServiceClient = null;

    private int msgIndex;
    private List<string> msgList;
    private int petPrefabId;

    private bool isFourthPlayerAvailable = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(instance);
        }
    }

    private void OnEnable()
    {
        happyBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Happy));
        eatBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Eat));
        cleanBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Clean));
        sleepBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Energy));
    }

    private void Start()
    {
        msgList = new List<string>();

        _ExtraPlayerDataServiceClient = new ExtraPlayerDataServiceClient();

        //If show welcome is true then show UI
        if (PlayerPrefs.GetInt(_Strings.DatFromRaceScene) == 1)
        {
            Debug.Log("Showing wellcome back popup");
            PlayerPrefs.SetInt(_Strings.DatFromRaceScene, 0);
            ShowWelcomeBackFromRaceUI();
        }

        //SHOW Coins

    }

    #region SHOW COINS
    private void ShowCoins()
    {

    }
    #endregion

    #region BUTTON CLICK EVENTS

    public void ManagePetCareBtnsFromSoap(bool wantToOn)
    {
        wellbeingBtnImg.sprite = wellbeingBtnsOn;
        careBtnHolder.SetActive(false);
        //cameraViewManager.SetCameraTopView();
        //If sleeping then on the sleep ui
        if (!petDataRef.petData.sleepData.isSleeping)
        {
            sleepPanel.SetActive(false);
        }
    }

    public void ManagePetCareBtns(bool wantToOn)
    {
        //If pet is in the training we can't access pet
        if (petDataRef.petData.ongoingTrainingData.isTraining)
        {
            ShowNotificationUI("Sorry, you can't access this while your pet is training");
            return;
        }

        //Managing pet care buttons and wellbeing button image
        if (careBtnHolder.activeInHierarchy)
        {
            wellbeingBtnImg.sprite = wellbeingBtnsOn;
            careBtnHolder.SetActive(false);
            cameraViewManager.SetCameraTopView();
            //If sleeping then on the sleep ui
            if (!petDataRef.petData.sleepData.isSleeping)
            {
                sleepPanel.SetActive(false);
            }
        }
        else
        {
            if (wantToOn)
            {
                wellbeingBtnImg.sprite = wellbeingBtnsOff;
                careBtnHolder.SetActive(true);
            }
        }
    }

    //START RACE
    public void _StartRace()
    {
        BeamableCloudSaveManager.instance.SaveData(petDataRef.petData);
        SceneManager.LoadScene(_Strings.RaceScene);
    }

    //Pet Care button click event
    public void OnClickOfPetCareStateBtn(PetCareState selectedState)
    {
        petCareEvent.Raise(selectedState);
    }

    //Home button click event
    public void OnClickOfHomeBtn(string sceneName)
    {
        if (BeamableCloudSaveManager.instance._cloudSavingService != null)
        {
            BeamableCloudSaveManager.instance.SaveData(petDataRef.petData);
        }

        SceneManager.LoadScene(sceneName);
    }

    //Inventory button click event
    public void OnClickOfInventoryBtn()
    {
        if (!petDataRef.petData.ongoingTrainingData.isTraining)
        {
            inventoryPanel.SetActive(true);

        }
        else
        {
            ShowNotificationUI("Sorry, you can't access this while your pet is training");
        }

        PlayTutorial("inventoryTutorial");
    }
    #endregion

    #region LOADING UI
    public void ShowLoadingCanvas()
    {
        loadingCanvas.SetActive(true);
    }

    public void OffLoadingCanvas()
    {
        loadingCanvas.SetActive(false);
    }
    #endregion

    #region PETS PROFILE UI
    public void ShowPetStatPanel()
    {
        petStatPanel.SetActive(true);

        petNameTxt.text = petDataRef.petData.petname;
        petRankTxt.text = "Rank: " + petDataRef.petData.rank;

        happyFillPanelSlider.DOValue(petDataRef.petData.happiness, 0.5f);
        hungerFillPanelSlider.DOValue(petDataRef.petData.hunger, 0.5f);
        cleanFillPanelSlider.DOValue(petDataRef.petData.cleanliness, 0.5f);
        energyFillPanelSlider.DOValue(petDataRef.petData.energy, 0.5f);

        runningStatTxt.text = "Running: " + petDataRef.petData.running;
        climbingStatTxt.text = "Climbing: " + petDataRef.petData.climbing;
        flyingStatTxt.text = "Flying: " + petDataRef.petData.flying;
        swimmingStatTxt.text = "Swimming: " + petDataRef.petData.swimming;

        intelligenceStatTxt.text = "Intelligence: " + petDataRef.petData.intelligence;
        luckStatTxt.text = "Luck: " + petDataRef.petData.luck;
    }
    #endregion

    #region PET CREATION UI
    public void ShowPetCreationUI()
    {
        OffLoadingCanvas();
        petCreationPanelMain.SetActive(true);
        eggSelectionPanel.SetActive(true);
    }

    public void EggSelectionFourthPlayer(int index)
    {
        //isFourthPlayerAvailable = true;
        if (!isFourthPlayerAvailable)
        {
            //FOURTH PLAYER NOT AVAILABLE SHOW DESCRIPTION OR INSTRUCTION 

            return;
        }

        Debug.Log(index);

        //UNLOCK FOURTH PLAYER
        eggSelectionPanel.SetActive(false);
        welcomePanel.SetActive(true);

        selectedEggPet.sprite = petImages[index - 1];

        //Setting Pet prefab index for spawning pet
        petPrefabId = index;
    }

    public void EggSelectedAndShowPetWelcomePanel(int index)
    {
        eggSelectionPanel.SetActive(false);
        welcomePanel.SetActive(true);

        selectedEggPet.sprite = petImages[index - 1];

        //Setting Pet prefab index for spawning pet
        petPrefabId = index;
        /*petDataRef.petLocalData = new PetLocalData();
        petDataRef.petLocalData.petID = index.ToString();*/
        /*PlayerPrefs.SetInt(_Strings.PetID, index);
        Debug.Log("Petid - " + PlayerPrefs.GetInt(_Strings.PetID));*/
    }

    public void NextFromWelcomePanel()
    {
        welcomePanel.SetActive(false);
        setPetDetailsPanel.SetActive(true);
        UpdatePetDetailsSliderStatsText();
    }

    public void UpdatePetDetailsSliderStatsText()
    {
        abilityStatsTxt.text = "Total: (" + (runningSliderPetCreation.value + climbingSliderPetCreation.value
                                + flyingSliderPetCreation.value + swimmingSliderPetCreation.value) + "/" + maxAbilityStatLimit + ")";

        runningPetCreationStatTxt.text = "(" + runningSliderPetCreation.value + "/50)";
        climbingPetCreationStatTxt.text = "(" + climbingSliderPetCreation.value + "/50)";
        flyingPetCreationStatTxt.text = "(" + flyingSliderPetCreation.value + "/50)";
        swimmingPetCreationStatTxt.text = "(" + swimmingSliderPetCreation.value + "/50)";

        chanceStatsTxt.text = "Total: (" + (intelligenceSliderPetCreation.value + luckSliderPetCreation.value)
                                + "/" + maxChanceStatLimit + ")";

        intelligencePetCreationStatTxt.text = "(" + intelligenceSliderPetCreation.value + "/15)";
        luckPetCreationStatTxt.text = "(" + luckSliderPetCreation.value + "/15)";
    }

    public async void SubmitPetDetails()
    {
        //Checking for empty name and stats max limit
        if (petInput.text == "")
        {
            StartCoroutine(ShowNoticeTxt("Please enter Pet name!"));
        }
        else if ((runningSliderPetCreation.value + climbingSliderPetCreation.value
                + flyingSliderPetCreation.value + swimmingSliderPetCreation.value) > maxAbilityStatLimit)
        {
            StartCoroutine(ShowNoticeTxt("Please select Ability stats within Max limit!"));
        }
        else if ((intelligenceSliderPetCreation.value + luckSliderPetCreation.value) > maxChanceStatLimit)
        {
            StartCoroutine(ShowNoticeTxt("Please select Chance stats within Max limit!"));
        }
        else
        {
            await BeamableCloudSaveManager.instance.CreateNewPet(petInput.text, petPrefabId, (int)runningSliderPetCreation.value, (int)climbingSliderPetCreation.value, (int)flyingSliderPetCreation.value,
                                                            (int)swimmingSliderPetCreation.value, (int)intelligenceSliderPetCreation.value, (int)luckSliderPetCreation.value);
            setPetDetailsPanel.SetActive(false);
            petCreationPanelMain.SetActive(false);
            ClosePetSickLabel();
            PlayTutorial("introTutorial");
        }
    }

    IEnumerator ShowNoticeTxt(string msg)
    {
        noticeTxt.text = msg;

        yield return new WaitForSeconds(3f);
        noticeTxt.text = "";
    }
    #endregion

    #region PET SICK LABEL UI
    public void ShowPetSickLabel()
    {
        if (!sickLabel.activeInHierarchy)
        {
            sickLabelTxt.text = petDataRef.petData.petname + " is sick! \nThey need medicine.";
            sickLabel.SetActive(true);
        }
    }

    public void ClosePetSickLabel()
    {
        sickLabel.SetActive(false);
    }

    #endregion

    #region NOTIFICATION POPUP UI
    public void ShowNotificationUI(string msg)
    {
        notificationMsgTxt.text = msg;
        notificationPopup.SetActive(true);
    }

    public void ManageNotificationMsg(List<string> msgs)
    {
        msgList = msgs;
        msgIndex = 0;

        ShowNotificationUI(msgList[msgIndex]);
        msgIndex++;
    }

    public void CheckAndCloseNotificationUI()
    {
        if (msgList.Count != 0 && msgList.Count > msgIndex)
        {
            ShowNotificationUI(msgList[msgIndex]);
            msgIndex++;
        }
        else
        {
            notificationPopup.transform.DOScale(0.2f, 0.25f).OnComplete(() => notificationPopup.SetActive(false));
        }
    }
    #endregion

    #region PET DEATH UI
    public void ShowPetDeathUI(string deathReason)
    {
        petDeathReasonTxt.text = deathReason;
        petDeathPanel.SetActive(true);
    }
    #endregion

    #region RANK UP UI
    public void ShowRankUpUI()
    {
        rankUpMsgTxt.text = petDataRef.petData.petname + " just moved up to Rank " + petDataRef.petData.rank + "!\n" +
                            petDataRef.petData.petname + "'s Max Stamina is now " + petDataRef.petData.maxStamina + ".\n" +
                            "You've earned " + (petDataRef.petData.rank * 5) + " Coins.\n" +
                            petDataRef.petData.petname + " can now compete against Rank " + petDataRef.petData.rank + " pets in Races.";
        rankUpPanel.SetActive(true);
    }
    #endregion

    #region WELCOME BACK FROM RACE UI
    public void ShowWelcomeBackFromRaceUI()
    {

        int Coins = PlayerPrefs.GetInt(_Strings.CoinsToAdd);
        int XP = PlayerPrefs.GetInt(_Strings.XpToAdd);

        earnedStatsTxt.text = "� " + XP.ToString() + " XP\n"
                            + "� " + Coins.ToString() + " Coins";

        niceWorkTxt.text = "Nice work, " + petDataRef.petData.petname + "!";
        welcomeBackPanel.SetActive(true);


        PlayerPrefs.SetInt(_Strings.CoinsToAdd, 0);
        PlayerPrefs.SetInt(_Strings.XpToAdd, 0);
        PlayerPrefs.SetInt(_Strings.DatFromRaceScene, 0);


        Debug.Log("My XP incrimental is " + XP);
        Debug.Log("My Coins incrimental is " + Coins);

        SetRacingEarnedStats(Coins, XP);
    }

    public void SetRacingEarnedStats(int _coins_to_add, int xp_to_add)
    {
        //XP
        petStateManager.IncreaseXP(xp_to_add);

        //Add Coins
        beamableInventoryManager.AddCurrency(_coins_to_add);
    }
    #endregion

    #region TUTORIAL UI

    public void PlayTutorial(string tutorial)
    {
        switch (tutorial)
        {
            case "introTutorial":
                if (extraPlayerDataRef.extraPlayerData.introTutorial == 0)
                {
                    introTutorialPanel.SetActive(true);
                    extraPlayerDataRef.extraPlayerData.introTutorial = 1;
                }
                break;

            case "eatTutorial":
                if (extraPlayerDataRef.extraPlayerData.eatTutorial == 0)
                {
                    eatingTutorialPanel.SetActive(true);
                    extraPlayerDataRef.extraPlayerData.eatTutorial = 1;
                }
                break;

            case "inventoryTutorial":
                if (extraPlayerDataRef.extraPlayerData.inventoryTutorial == 0)
                {
                    inventoryTutorialPanel.SetActive(true);
                    extraPlayerDataRef.extraPlayerData.inventoryTutorial = 1;
                }
                break;

            case "showerTutorial":
                if (extraPlayerDataRef.extraPlayerData.showerTutorial == 0)
                {
                    showerTutorialPanel.SetActive(true);
                    extraPlayerDataRef.extraPlayerData.showerTutorial = 1;
                }
                break;
        }
    }

    #endregion

    #region 4th peth activation 
    public void ActivateFourthPet()
    {
        isFourthPlayerAvailable = true;
        tempImage.SetActive(false);
    }
    #endregion
}
