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

    [Header("Set Pet Details UI")]
    public InputField petInput;
    public Slider runningSliderPetCreation;
    public Slider climbingSliderPetCreation;
    public Slider flyingSliderPetCreation;
    public Slider swimmingSliderPetCreation;
    public Slider intelligenceSliderPetCreation;
    public Slider luckSliderPetCreation;
    public Text noticeTxt;

    public int maxAbilityStatLimit;
    public int maxChanceStatLimit;

    [Space]
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
    [Header("Pet Stats UI")]
    public GameObject petStatPanel;
    public Text petNameTxt;

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

    [Space]
    public Button medicineBtn;
    public Text medicineCntTxt;
    public int medicineCnt = 1;

    private void Start()
    {
        happyBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Happy));
        eatBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Feed));
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

    public void UpdatePetSickToHealthy()
    {
        if (petDataRef.petData.isSick)
        {
            medicineBtn.interactable = false;
            medicineCntTxt.text = (medicineCnt - 1).ToString();
            petDataRef.petData.isSick = false;
        }
    }

    public void ShowPetCreationUI()
    {
        petCreationPanelMain.SetActive(true);
        eggSelectionPanel.SetActive(true);
    }

    public void EggSelectedAndShowPetWelcomePanel(int index)
    {
        Debug.Log("Selected Egg - " +  index);
        eggSelectionPanel.SetActive(false);
        welcomePanel.SetActive(true);
    }

    public void NextFromWelcomePanel()
    {
        welcomePanel.SetActive(false);
        setPetDetailsPanel.SetActive(true);
    }

    public void SubmitPetDetails()
    {
        if(petInput.text == "")
        {
            StartCoroutine(ShowNoticeTxt("Please enter valid Pet name!"));
        }
        else if((runningSliderPetCreation.value + climbingSliderPetCreation.value 
                + flyingFillSlider.value + swimmingFillSlider.value) > maxAbilityStatLimit)
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
}
