using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour
{
    const string IDLE = "Idle";
    const string WALK = "Walk";

    public NavMeshAgent agent;
    Animator animator;
    public float range; // radius of sphere
    float lookRotationSpeed = 8f;

    public Transform centrePoint; // centre of the area the agent wants to move around in

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        FaceTarget();
        SetAnimations();
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 point;
            if(RandomPoint(centrePoint.position, range, out point)) // pass in centre point and radius of area
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); // visual debug for reference
        
                agent.SetDestination(point);
            }
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range; // random point in a sphere
        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            // 1.0f is the max distance from the random point to a point on the navmesh
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }

    void FaceTarget()
    {
        Vector3 direction = (agent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
    }

    void SetAnimations()
    {
        if(agent.velocity == Vector3.zero)
        {
            animator.Play(IDLE);
        } 
        else
        {
            animator.Play(WALK);
        }
    }
}
