using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PetCareInputManager : MonoBehaviour
{
    public static PetCareInputManager instance;

    public SimpleGameEvent playerReachedEvent;

    [Space]
    public PetCareStateManager petCareStateManager;
    public ParticleEffectsManager particleEffectsManager;

    [Space]
    public Vector3 playerPosInSleep;
    public Transform playerPosOutSleep;

    [HideInInspector]
    public PetAnimation petAnim;

    NavMeshAgent agent;
    bool isCheckForPathCompletion = false;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {

        agent = GetComponent<NavMeshAgent>();
    }

    private void OnMouseDown()
    {
        switch (petCareStateManager.selectedPetCareState)
        {
            case PetCareState.Happy:

                if(petCareStateManager.petDataRef.petData.happiness < 100)
                {
                    //Play Happy Animation
                    particleEffectsManager.PlayHappyEffect();
                    petAnim._ChangeAnimationState(_AnimState.Happy);
                    petCareStateManager.ManageHappinessDataFiller(petCareStateManager.petStatData.happinessTickRate);  
                }
                break;
        }
    }

    private void Update()
    {
        // Check if we've reached the destination
        if (!agent.pathPending && isCheckForPathCompletion)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    // Done
                    if (petCareStateManager.petDataRef.petData.isSick)
                    {
                        petAnim._ChangeAnimationState(_AnimState.Sick);
                    }
                    else
                    {
                        petAnim._ChangeAnimationState(_AnimState.Idle);
                    }
                    transform.DORotateQuaternion(Quaternion.Euler(0f, 180f, 0f), 2f);
                    playerReachedEvent.Raise();

                    isCheckForPathCompletion = false;
                    //transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                }
            }
        }
    }

    public void SetDestinationPoint(Vector3 point)
    {
        isCheckForPathCompletion = true;
        petAnim._ChangeAnimationState(_AnimState.Run);
        agent.SetDestination(point);
    }

    public void SetPetToInsideHomeOnSleepStart()
    {
        petAnim.transform.gameObject.SetActive(false);
        transform.DOMove(playerPosInSleep, 1f);
    }

    public void SetPetToOutsideHomeOnSleepComplete()
    {
        transform.DOMove(playerPosOutSleep.position, 1f);
        petAnim.transform.gameObject.SetActive(true);
    }
}
