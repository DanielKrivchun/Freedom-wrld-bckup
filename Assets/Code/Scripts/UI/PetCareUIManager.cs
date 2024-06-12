using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Beamable.CloudSavingService;
using System;
using UnityEngine.SceneManagement;
using Beamable.InventoryService;

public class PetCareUIManager : MonoBehaviour
{
    public static PetCareUIManager instance;

    [Header("Pet Care Data Reference")]
    public PetDataRef petDataRef;

    [Space]
    public GameEventState petCareEvent;

    [Header("Loading UI")]
    public GameObject loadingCanvas;

    [Header("Pet Creation UI")]
    public GameObject petCreationPanelMain;
    public GameObject eggSelectionPanel;
    public GameObject welcomePanel;
    public GameObject setPetDetailsPanel;

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

    [Header("Script Ref")]
    public PetCareStateManager petStateManager;
    public BeamableInventoryManager beamableInventoryManager;

    private int msgIndex;
    private List<string> msgList;

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
    }

    #region BUTTON CLICK EVENTS
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

    //Pet Care button click event
    public void OnClickOfPetCareStateBtn(PetCareState selectedState)
    {
        petCareEvent.Raise(selectedState);
    }

    //Home button click event
    public void OnClickOfHomeBtn(string sceneName)
    {
        BeamableCloudSaveManager.instance.SaveData(petDataRef.petData);
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

    public void EggSelectedAndShowPetWelcomePanel(int index)
    {
        eggSelectionPanel.SetActive(false);
        welcomePanel.SetActive(true);

        selectedEggPet.sprite = petImages[index - 1];

        //Setting Pet prefab index for spawning pet
        petDataRef.petLocalData.petID = index.ToString();
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
            await BeamableCloudSaveManager.instance.CreateNewPet(petInput.text, (int)runningSliderPetCreation.value, (int)climbingSliderPetCreation.value, (int)flyingSliderPetCreation.value,
                                                            (int)swimmingSliderPetCreation.value, (int)intelligenceSliderPetCreation.value, (int)luckSliderPetCreation.value);
            setPetDetailsPanel.SetActive(false);
            petCreationPanelMain.SetActive(false);
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
            notificationPopup.SetActive(false);
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
        /*earnedStatsTxt.text = "• " + currentTrainingData.xP.ToString() + " XP\n"
                            + "• " + currentTrainingData.coins.ToString() + " Coins";*/

        niceWorkTxt.text = "Nice work, " + petDataRef.petData.petname + "!";

        SetRacingEarnedStats();
    }

    public void SetRacingEarnedStats()
    {
        //XP
        //petStateManager.IncreaseXP(XP);

        //Add Coins
        //beamableInventoryManager.AddCurrency(coins); 
    }
    #endregion
}
