using UnityEngine;
using UnityEngine.AI;

public class NPCPathWalker : MonoBehaviour
{
    public Transform[] waypoints;
    public bool loop = true;
    public float waitTimeAtPoint = 0f;

    private NavMeshAgent agent;
    private Animator animator;
    private int currentIndex = 0;
    private float waitTimer = 0f;
    private bool waiting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned.");
            enabled = false;
            return;
        }

        GoToWaypoint(currentIndex);
    }

    void Update()
    {
        if (agent == null) return;

        // Animation: walk when agent is moving
        if (animator != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
        }

        if (waiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                waiting = false;
                MoveToNextWaypoint();
            }
            return;
        }

        // Wait until path is finished calculating
        if (agent.pathPending)
            return;

        // Check if reached destination
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f)
            {
                if (waitTimeAtPoint > 0f)
                {
                    waiting = true;
                    waitTimer = 0f;
                }
                else
                {
                    MoveToNextWaypoint();
                }
            }
        }
    }

    void MoveToNextWaypoint()
    {
        currentIndex++;

        if (currentIndex >= waypoints.Length)
        {
            if (loop)
                currentIndex = 0;
            else
            {
                enabled = false;
                return;
            }
        }

        GoToWaypoint(currentIndex);
    }

    void GoToWaypoint(int index)
    {
        if (waypoints[index] != null)
        {
            agent.SetDestination(waypoints[index].position);
        }
    }
}