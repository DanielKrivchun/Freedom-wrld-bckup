using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PetCareInputManager : MonoBehaviour
{
    public static PetCareInputManager instance;

    public SimpleGameEvent petCareReachedPointEvent;
    public SimpleGameEvent petTrainingReachedPointEvent;

    [Space]
    public PetCareStateManager petCareStateManager;
    public ParticleEffectsManager particleEffectsManager;

    [Space]
    public float navmeshSpawnOffset;

    [Space]
    public List<Transform> swimmingPoints;

    public float moveSpeed, rotationSpeed;

    [HideInInspector]
    public PetAnimation petAnim;

    NavMeshAgent agent;
    bool isCheckForPathCompletion = false, isTrainingPoint;
    private bool isMoving;
    private Transform targetPoint;
    private List<Transform> trainingPoints;

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

        transform.position = GetRandomPointOnNavMesh(transform.position, navmeshSpawnOffset);
    }

    //Set player on Random spawn point
    Vector3 GetRandomPointOnNavMesh(Vector3 center, float range)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, range, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return Vector3.zero; // Return zero if no valid NavMesh point is found
    }

    //Happy state
    private void OnMouseDown()
    {
        switch (petCareStateManager.selectedPetCareState)
        {
            case PetCareState.Happy:

                if (petCareStateManager.petDataRef.petData.happiness < 100)
                {
                    //Play Happy Animation
                    particleEffectsManager.PlayHappyEffect();
                    petAnim._ChangeAnimationState(_AnimState.Happy);
                    petCareStateManager.ManageHappinessDataFiller(petCareStateManager.petStatData.happinessTickRate);
                }
                break;
        }
    }

    //Setting and moving player to destination
    public void SetDestinationPoint(Vector3 point, bool isTrainingPoint)
    {
        isCheckForPathCompletion = true;
        petAnim._ChangeAnimationState(_AnimState.Run);
        agent.SetDestination(point);

        this.isTrainingPoint = isTrainingPoint;
    }

    //Checking for path completion
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
                    if (!isTrainingPoint)
                    {
                        ReachedToPetCareDestinationPoint();
                    }
                    else
                    {
                        petTrainingReachedPointEvent.Raise();
                    }

                    isCheckForPathCompletion = false;
                }
            }
        }

        if (isMoving)
        {
            // Move the player towards the target point
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

            // Rotate the player to face the target point
            Vector3 direction = (targetPoint.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);


            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                MoveToRandomPoints();
            }
        }
    }

    //Reached to Pet Care destination point
    void ReachedToPetCareDestinationPoint()
    {
        SetIdleOrSickAnim();
        transform.DORotateQuaternion(Quaternion.Euler(0f, 180f, 0f), 1f);
        petCareReachedPointEvent.Raise();
    }

    public void NavigatePlayerAroundTrainingPath(PetTraining currentTraining)
    {
        switch (currentTraining)
        {
            case PetTraining.Swimming:
                agent.enabled = false;
                petAnim._ChangeAnimationState(_AnimState.Swimming);
                trainingPoints = swimmingPoints;
                MoveToRandomPoints();
                break;
        }
    }

    private void MoveToRandomPoints()
    {
        // Select a random point from the list
        targetPoint = trainingPoints[Random.Range(0, trainingPoints.Count)];
        isMoving = true;
    }

    public void StopNavigating()
    {
        isMoving = false;
        agent.enabled = true;
        SetIdleOrSickAnim();
    }

    //Pet on-off on sleep
    public void SetPetToInsideHomeOnSleepStart()
    {
        petAnim.transform.gameObject.SetActive(false);
    }

    public void SetPetToOutsideHomeOnSleepComplete()
    {
        petAnim.transform.gameObject.SetActive(true);
    }

    void SetIdleOrSickAnim()
    {
        if (petCareStateManager.petDataRef.petData.isSick)
        {
            petAnim._ChangeAnimationState(_AnimState.Sick);
        }
        else
        {
            petAnim._ChangeAnimationState(_AnimState.Idle);
        }
    }
}
