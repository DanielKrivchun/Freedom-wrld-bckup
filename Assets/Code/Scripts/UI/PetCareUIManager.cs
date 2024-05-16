using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PetCareUIManager : MonoBehaviour
{
    [Header("Pet Care Data Reference")]
    public PetDataRef petDataRef;

    [Space]
    public GameEventState petCareEvent;

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
    public GameObject petStatPanel;

    public Text petNameTxt;

    [Space]
    [Header("Wellbeing Stat Slider")]
    public Slider happyFillSliderP;
    public Slider hungerFillSliderP;
    public Slider cleanFillSliderP;
    public Slider energyFillSliderP;

    [Space]
    [Header("Athletics Stat Slider")]
    public Slider runningFillSlider;
    public Slider climbingFillSlider;
    public Slider flyingFillSlider;
    public Slider swimmingFillSlider;

    [Space]
    [Header("Chance Stat Slider")]
    public Slider intelligenceFillSlider;
    public Slider luckFillSlider;

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

        happyFillSliderP.DOValue(petDataRef.petData.happiness, 0.5f);
        hungerFillSliderP.DOValue(petDataRef.petData.hunger, 0.5f);
        cleanFillSliderP.DOValue(petDataRef.petData.cleanliness, 0.5f);
        energyFillSliderP.DOValue(petDataRef.petData.energy, 0.5f);

        runningFillSlider.DOValue(petDataRef.petData.running, 0.5f);
        climbingFillSlider.DOValue(petDataRef.petData.climbing, 0.5f);
        flyingFillSlider.DOValue(petDataRef.petData.flying, 0.5f);
        swimmingFillSlider.DOValue(petDataRef.petData.swimming, 0.5f);

        intelligenceFillSlider.DOValue(petDataRef.petData.intelligence, 0.5f);
        luckFillSlider.DOValue(petDataRef.petData.luck, 0.5f);
    }
}
