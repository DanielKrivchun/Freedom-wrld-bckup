using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PetCareInputManager : MonoBehaviour
{
    public static PetCareInputManager instance;

    [Header("Events")]
    public SimpleGameEvent petCareReachedPointEvent;
    public SimpleGameEvent petTrainingReachedPointEvent;

    [Header("Script Ref")]
    public PetCareStateManager petCareStateManager;
    public ParticleEffectsManager particleEffectsManager;
    public PetCareCameraViewManager cameraViewManager;

    [Header("Running Training")]
    public List<Transform> runningPoints;
    public float runSpeed;
    public float runRotationSpeed;

    [Header("Swimming Training")]
    public List<Transform> swimmingPoints;
    public float swimSpeed;
    public float swimRotationSpeed;

    [Header("Flying Training")]
    public List<Transform> flyingPoints;
    public float flySpeed;
    public float flyRotationSpeed;

    [Header("Intelligence Training")]
    public Transform intelligencePoint;

    [Header("Pet Auto Navigate Config")]
    public float navmeshSpawnOffset;
    public float petAutoNavigateCheckTime;

    [HideInInspector]
    public PetAnimation petAnim;
    [HideInInspector]
    public Animator animator;

    NavMeshAgent agent;
    bool isCheckForPathCompletion = false, isTrainingPoint, isMoving, isPlayerAutoNavigatingOnMap, isRandomPoints;

    float moveSpeed, rotationSpeed;

    private Vector3 navmeshPos;
    private Transform targetPoint;
    private List<Transform> trainingPoints;

    float timer;
    int pointIndex = 0;

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
        animator = GetComponent<Animator>();

        transform.position = GetRandomPointOnNavMesh(transform.position, navmeshSpawnOffset);
        timer = petAutoNavigateCheckTime;
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


    void CheckForSetPetIdleOrNavigating()
    {
        if (petCareStateManager.petDataRef.petData != null || 
            petCareStateManager.petDataRef.petData?.sleepData.isSleeping == true || 
            petCareStateManager.petDataRef.petData?.ongoingTrainingData.isTraining == true || 
            petCareStateManager.isCareTaking)
        {
            return;
        }

        //Idle
        if (Random.Range(0f, 100f) < 50f)
        {
            SetPetToIdle();
        }
        //Walk around map
        else
        {
            Debug.Log("Set pet moving");
            SetDestinationPointForNavigationOnHomeIsland();
        }
    }

    public void SetPetToIdle()
    {
        Debug.Log("Set pet idle");
        agent.isStopped = true;
        isPlayerAutoNavigatingOnMap = false;
        SetIdleOrSickAnim();
    }

    public void SetDestinationPointForNavigationOnHomeIsland()
    {
        isPlayerAutoNavigatingOnMap = true;
        agent.isStopped = false;
        navmeshPos = GetRandomPointOnNavMesh(transform.position, navmeshSpawnOffset);
        petAnim._ChangeAnimationState(_AnimState.Wallk);
        agent.SetDestination(navmeshPos);
    }

    //Navmesh movement
    void CheckForPlayerNavmeshMovement()
    {
        if (isPlayerAutoNavigatingOnMap && Vector3.Distance(transform.position, navmeshPos) < 0.1f)
        {
            SetDestinationPointForNavigationOnHomeIsland();
        }
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
                    petCareStateManager.StartIdleTimer();
                }
                break;
        }
    }

    //Setting and moving player to destination
    public void SetDestinationPointForCareTakingOrTraining(Vector3 point, bool isTrainingPoint)
    {
        agent.isStopped = false;
        isCheckForPathCompletion = true;
        petAnim._ChangeAnimationState(_AnimState.Run);
        agent.SetDestination(point);

        this.isTrainingPoint = isTrainingPoint;
    }

    //Checking for path completion
    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = petAutoNavigateCheckTime;
            CheckForSetPetIdleOrNavigating();
        }


        CheckForPlayerReachedToDestination();

        CheckForPlayerNavmeshMovement();

        CheckForPlayerTrainingMovement();
    }

    // Check if pet reached the destination
    void CheckForPlayerReachedToDestination()
    {
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
    }

    //Reached to Pet Care destination point
    void ReachedToPetCareDestinationPoint()
    {
        SetIdleOrSickAnim();
        transform.DORotateQuaternion(Quaternion.Euler(0f, 180f, 0f), 1f);
        petCareReachedPointEvent.Raise();
    }

    //Moving pet to training path
    public void NavigatePlayerAroundTrainingPath(PetTraining currentTraining)
    {
        switch (currentTraining)
        {
            case PetTraining.Running:
                SetPetAnimationAndTrainingPoints(_AnimState.Run, runningPoints, false, runSpeed, runRotationSpeed);
                particleEffectsManager.StartRunningDirtEffect();
                cameraViewManager.SetCameraRunningTrainingView();
                break;

            case PetTraining.Swimming:
                SetPetAnimationAndTrainingPoints(_AnimState.Swimming, swimmingPoints, true, swimSpeed, swimRotationSpeed);
                particleEffectsManager.StartSwimmingWaterSplashEffect();
                cameraViewManager.SetCameraSwimmingTrainingView();
                break;

            case PetTraining.Flying:
                SetPetAnimationAndTrainingPoints(_AnimState.Flying, flyingPoints, false, flySpeed, flyRotationSpeed);
                particleEffectsManager.StartFlyingWindEffect();
                break;

            case PetTraining.Climbing:
                animator.enabled = true;
                cameraViewManager.SetCameraClimbingTrainingView();
                break;

            case PetTraining.Intelligence:
                petAnim._ChangeAnimationState(_AnimState.Idle);
                transform.DORotateQuaternion(Quaternion.Euler(0f, 180f, 0f), 1f);
                particleEffectsManager.StartPuzzleEffect();
                cameraViewManager.SetCameraFrontView();
                break;
        }
    }

    //Setting training animation, speed and path points
    void SetPetAnimationAndTrainingPoints(_AnimState animState, List<Transform> points, bool isMoveOnRandomPoints, float moveSpeed, float rotationSpeed)
    {
        //animation
        agent.enabled = false;
        petAnim._ChangeAnimationState(animState);

        //speed
        this.moveSpeed = moveSpeed;
        this.rotationSpeed = rotationSpeed;

        //path points
        trainingPoints = points;
        isRandomPoints = isMoveOnRandomPoints;
        pointIndex = 0;

        MoveToTrainingPathPoints();
    }

    //Training movement
    void CheckForPlayerTrainingMovement()
    {
        if (isMoving)
        {
            PetMovingWithLookAtTarget(targetPoint.position);

            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                MoveToTrainingPathPoints();
            }
        }
    }

    private void MoveToTrainingPathPoints()
    {
        // Select a random point from the list
        if (isRandomPoints)
        {
            targetPoint = trainingPoints[Random.Range(0, trainingPoints.Count)];
            isMoving = true;
        }
        //Select loop point from a list
        else
        {
            targetPoint = trainingPoints[pointIndex];
            isMoving = true;
            pointIndex++;

            if(pointIndex >= trainingPoints.Count)
            {
                pointIndex = 0;
            }
        }

    }

    void PetMovingWithLookAtTarget(Vector3 targetPos)
    {
        // Move the player towards the target point
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // Rotate the player to face the target point
        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    //This is for when user back to app and training is going on
    public void StartNavigatingPlayerAroundTrainingPath(PetTraining currentTraining)
    {
        //Setting pet to training point
        switch (currentTraining)
        {
            case PetTraining.Running:
                transform.position = runningPoints[0].position;
                break;

            case PetTraining.Swimming:
                transform.position = swimmingPoints[0].position;
                break;

            case PetTraining.Flying:
                transform.position = flyingPoints[0].position;
                break;

            case PetTraining.Climbing:
                animator.enabled = true;
                break;

            case PetTraining.Intelligence:
                transform.position = intelligencePoint.position;
                break;
        }

        NavigatePlayerAroundTrainingPath(currentTraining);
    }

    //Stop moving
    public IEnumerator StopNavigating()
    {
        cameraViewManager.SetCameraTopView();
        yield return new WaitForSeconds(1f);
        
        isMoving = false;
        agent.enabled = true;
        animator.enabled = false;
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

    //If pet is sick then play sick animation or play idle animation
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

    //Change animation state for Climbing training
    public void ChangeAnimationState(string animationName)
    {
        switch(animationName)
        {
            case "Climb":
                petAnim._ChangeAnimationState(_AnimState.Climbing);
                break;

            case "Walk":
                petAnim._ChangeAnimationState(_AnimState.Wallk);
                break;
        }
    }
}
