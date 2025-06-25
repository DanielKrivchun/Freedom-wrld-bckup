using Beamable.Server.Clients;
using DG.Tweening;
using UnityEngine;

public class PetCareObjectManager : MonoBehaviour
{
    [Header("Script Ref")]
    public PetCareStateManager petStateManager;
    public PetCareInputManager petInputManager;
    public PetCareCameraViewManager cameraViewManager;
    public ExtraPlayerDataManager extraPlayerDataManager;
    public PetCareUIManager petCareUiManager;

    [Header("Extra Player Data Reference")]
    public ExtraPlayerDataListSO extraPlayerDataRef;

    [Header("Happy")]
    [SerializeField] CharacterController characterController;

    [Header("Eat")]
    public GameObject eatObjectHolder;
    public Transform eatPoint;

    [Header("Clean")]
    public GameObject bathObjectHolder;
    public Transform bathPoint;
    public BathObject bathObject;

    [Header("Sleep")]
    public GameObject sleepCanvas;
    public Transform sleepPoint;

    [Header("Tutorial UI")]
    public GameObject eatTutorialPanel;

    private ExtraPlayerDataServiceClient _ExtraPlayerDataServiceClient = null;

    #region MANAGE PET CARE OBJECTS
    public void ManagePetCareObjects(PetCareState state)
    {
        //Stop pet from navigating
        petInputManager.SetPetToIdle();

        //Set camera front for Happy
        if (state == PetCareState.Happy)
        {
            
            cameraViewManager.SetCameraFrontView();
        }
        else
        {
            cameraViewManager.SetCameraTopView();
        }

        switch (state)
        {
            case PetCareState.Happy:
                petInputManager.transform.DORotateQuaternion(Quaternion.Euler(0f, 180f, 0f), 1f);

                characterController.enabled = true;
                bathObjectHolder.SetActive(false);
                eatObjectHolder.SetActive(false);
                sleepCanvas.SetActive(false);
                break;

            case PetCareState.Eat:
                petInputManager.SetDestinationPointForCareTakingOrTraining(eatPoint.position, false);
                petStateManager.SetAvailabeFoodItemOnTable();

                characterController.enabled = false;
                bathObjectHolder.SetActive(false);
                sleepCanvas.SetActive(false);

                petCareUiManager.PlayTutorial("eatTutorial");
                break;

            case PetCareState.Clean:
                petInputManager.SetDestinationPointForCareTakingOrTraining(bathPoint.position, false);

                characterController.enabled = false;
                eatObjectHolder.SetActive(false);
                sleepCanvas.SetActive(false);

                petCareUiManager.PlayTutorial("showerTutorial");
                break;

            case PetCareState.Energy:
                petInputManager.SetDestinationPointForCareTakingOrTraining(sleepPoint.position, false);

                characterController.enabled = false;
                bathObjectHolder.SetActive(false);
                eatObjectHolder.SetActive(false);
                break;

            default:
                characterController.enabled = false;
                bathObjectHolder.SetActive(false);
                eatObjectHolder.SetActive(false);
                sleepCanvas.SetActive(false);
                break;
        }

        //Start Idle Timer for Pet Care State
        if(state != PetCareState.None)
        {
            petStateManager.StartIdleTimer();
        }
    }
    #endregion

    #region ON PET REACHED TO DESTINATION
    public void PetReachedSelectedObjectDestination()
    {
        //Set camera front for Eat & Clean state after pet reached to destination point
        if (petStateManager.selectedPetCareState == PetCareState.Eat ||
            petStateManager.selectedPetCareState == PetCareState.Clean)
        {
            cameraViewManager.SetCameraFrontView();
        }

        switch (petStateManager.selectedPetCareState)
        {
            case PetCareState.Eat:
                eatObjectHolder.SetActive(true);
                break;

            case PetCareState.Clean:
                bathObjectHolder.SetActive(true);
                bathObject.MakePetReadyForBath();
                break;

            case PetCareState.Energy:
                sleepCanvas.SetActive(true);
                break;
        }
    }
    #endregion
}
