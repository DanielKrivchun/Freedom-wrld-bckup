using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Beamable.CloudSavingService;

public class PetCareUIManager : MonoBehaviour
{
    [Header("Pet Care Data Reference")]
    public PetDataRef petDataRef;

    [Space]
    public GameEventState petCareEvent;

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
    public Text runningStatTxt;

    [Space]
    public Slider climbingSliderPetCreation;
    public Text climbingStatTxt;

    [Space]
    public Slider flyingSliderPetCreation;
    public Text flyingStatTxt;

    [Space]
    public Slider swimmingSliderPetCreation;
    public Text swimmingStatTxt;

    [Space]
    public Text chanceStatsTxt;
    public int maxChanceStatLimit;

    [Space]
    public Slider intelligenceSliderPetCreation;
    public Text intelligenceStatTxt;

    [Space]
    public Slider luckSliderPetCreation;
    public Text luckStatTxt;

    [Space]
    public Text noticeTxt;

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

    [Space]
    [Header("Pet Stats Panel UI")]
    public GameObject petStatPanel;
    public Text petNameTxt;
    public Text petRankTxt;

    [Header("Wellbeing Stat Slider")]
    public Slider happyFillPanelSlider;
    public Slider hungerFillPanelSlider;
    public Slider cleanFillPanelSlider;
    public Slider energyFillPanelSlider;

    [Header("Athletics Stat Slider")]
    public Slider runningFillSlider;
    public Slider climbingFillSlider;
    public Slider flyingFillSlider;
    public Slider swimmingFillSlider;

    [Header("Chance Stat Slider")]
    public Slider intelligenceFillSlider;
    public Slider luckFillSlider;

    [Header("Pet Death Screen UI")]
    public GameObject petDeathPanel;
    public Text petDeathReasonTxt;

    [Header("Rank Up UI")]
    public GameObject rankUpPanel;
    public Text rankUpMsgTxt;

    private void Start()
    {
        happyBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Happy));
        eatBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Eat));
        cleanBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Clean));
        sleepBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Energy));
    }

    public void OnClickOfPetCareStateBtn(PetCareState selectedState)
    {
        petCareEvent.Raise(selectedState);
    }

    public void ShowPetStatPanel()
    {
        petStatPanel.SetActive(true);

        petNameTxt.text = petDataRef.petData.petname;
        petRankTxt.text = "Rank: " + petDataRef.petData.rank;

        happyFillPanelSlider.DOValue(petDataRef.petData.happiness, 0.5f);
        hungerFillPanelSlider.DOValue(petDataRef.petData.hunger, 0.5f);
        cleanFillPanelSlider.DOValue(petDataRef.petData.cleanliness, 0.5f);
        energyFillPanelSlider.DOValue(petDataRef.petData.energy, 0.5f);

        runningFillSlider.DOValue(petDataRef.petData.running, 0.5f);
        climbingFillSlider.DOValue(petDataRef.petData.climbing, 0.5f);
        flyingFillSlider.DOValue(petDataRef.petData.flying, 0.5f);
        swimmingFillSlider.DOValue(petDataRef.petData.swimming, 0.5f);

        intelligenceFillSlider.DOValue(petDataRef.petData.intelligence, 0.5f);
        luckFillSlider.DOValue(petDataRef.petData.luck, 0.5f);
    }

    public void ShowPetCreationUI()
    {
        petCreationPanelMain.SetActive(true);
        eggSelectionPanel.SetActive(true);
    }

    public void EggSelectedAndShowPetWelcomePanel(int index)
    {
        eggSelectionPanel.SetActive(false);
        welcomePanel.SetActive(true);

        selectedEggPet.sprite = petImages[index];
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

        runningStatTxt.text = "(" + runningSliderPetCreation.value + "/50)";
        climbingStatTxt.text = "(" + climbingSliderPetCreation.value + "/50)";
        flyingStatTxt.text = "(" + flyingSliderPetCreation.value + "/50)";
        swimmingStatTxt.text = "(" + swimmingSliderPetCreation.value + "/50)";

        chanceStatsTxt.text = "Total: (" + (intelligenceSliderPetCreation.value + luckSliderPetCreation.value)
                                + "/" + maxChanceStatLimit + ")";

        intelligenceStatTxt.text = "(" + intelligenceSliderPetCreation.value + "/15)";
        luckStatTxt.text = "(" + luckSliderPetCreation.value + "/15)";
    }

    public void SubmitPetDetails()
    {
        if(petInput.text == "")
        {
            StartCoroutine(ShowNoticeTxt("Please enter valid Pet name!"));
        }
        else if((runningSliderPetCreation.value + climbingSliderPetCreation.value 
                + flyingSliderPetCreation.value + swimmingSliderPetCreation.value) > maxAbilityStatLimit)
        {
            StartCoroutine(ShowNoticeTxt("Please select Ability stats within Max limit!"));
        }
        else if((intelligenceSliderPetCreation.value + luckSliderPetCreation.value) > maxChanceStatLimit)
        {
            StartCoroutine(ShowNoticeTxt("Please select Chance stats within Max limit!"));
        }
        else
        {
            BeamableCloudSaveManager.instance.CreateNewPet(petInput.text, (int)runningSliderPetCreation.value, (int)climbingSliderPetCreation.value, (int)flyingSliderPetCreation.value, 
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

    public void ShowPetDeathUI(string deathReason)
    {
        petDeathReasonTxt.text = deathReason;
        petDeathPanel.SetActive(true);
    }

    public void ShowRankUpUI()
    {
        rankUpMsgTxt.text = petDataRef.petData.petname + " just moved up to Rank " + petDataRef.petData.rank +"!\n" +
                            petDataRef.petData.petname + "'s Max Stamina is now " + petDataRef.petData.maxStamina + ".\n" +
                            "You've earned X Coins.\n" +
                            petDataRef.petData.petname + " can now compete against Rank " + petDataRef.petData.rank + " pets in Races.";
        rankUpPanel.SetActive(true);
    }
}
