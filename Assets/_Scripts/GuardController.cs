using UnityEngine;
using UnityEngine.AI;

public class GuardController : MonoBehaviour
{
    public enum GuardState
    {
        Idle,
        Patrolling,
        Alert,
        Chasing
    }

    [Header("State Settings")]
    [SerializeField] private GuardState currentState = GuardState.Patrolling;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waypointStopDistance = 0.5f;

    [Header("Detection Settings")]
    [SerializeField] private float sightRange = 10f;
    [SerializeField] private float sightAngle = 90f;
    [SerializeField] private float alertTime = 3f;
    [SerializeField] private Transform player;
    [SerializeField] private float suspiciousTime = 3f;


    // Component references
    private NavMeshAgent navAgent;
    private Animator animator;


    // Animation parameter hashes for efficiency
    private int speedHash;
    private int isAlertHash;
    private int isChasingHash;
    private bool isSuspicious = false;
    private float suspiciousTimer = 0f;
    private int isSuspiciousHash;

    // Patrol variables
    private int currentWaypointIndex = 0;
    private float alertTimer = 0f;

    void Start()
    {
        // Get components
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Set up animation parameter hashes
        speedHash = Animator.StringToHash("Speed");
        isAlertHash = Animator.StringToHash("IsAlert");
        isChasingHash = Animator.StringToHash("IsChasing");
        isSuspiciousHash = Animator.StringToHash("IsSuspicious");


        // Find player if not set
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        // Start patrolling
        if (waypoints.Length > 0 && currentState == GuardState.Patrolling)
        {
            SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
    public void BecomeSuspicious()
    {
        isSuspicious = true;
        print(isSuspicious);
        suspiciousTimer = suspiciousTime;
    }

    void Update()
    {
        // State machine
        switch (currentState)
        {
            case GuardState.Idle:
                UpdateIdle();
                break;

            case GuardState.Patrolling:
                UpdatePatrolling();
                break;

            case GuardState.Alert:
                UpdateAlert();
                break;

            case GuardState.Chasing:
                UpdateChasing();
                break;
        }

        if (isSuspicious)
        {
            suspiciousTimer -= Time.deltaTime;
            if (suspiciousTimer <= 0)
            {
                isSuspicious = false;
                print("not suspicious");
            }
        }

        // Update animations
        UpdateAnimations();

        // Check for player detection
        CheckForPlayerDetection();
    }

    void UpdateIdle()
    {
        // Guard is stationary
        navAgent.isStopped = true;
    }

    void UpdatePatrolling()
    {
        // Guard follows waypoints
        navAgent.isStopped = false;

        // Check if reached current waypoint
        if (navAgent.remainingDistance <= waypointStopDistance)
        {
            // Move to next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void UpdateAlert()
    {
        // Guard stops and looks around
        navAgent.isStopped = true;

        // Count down alert timer
        alertTimer -= Time.deltaTime;
        if (alertTimer <= 0)
        {
            // Return to patrolling
            TransitionToState(GuardState.Patrolling);
        }
    }

    void UpdateChasing()
    {
        // Guard chases player
        navAgent.isStopped = false;

        if (player != null)
        {
            SetDestination(player.position);
        }

        // Check if lost sight of player
        if (!CanSeePlayer())
        {
            // Transition to alert state
            alertTimer = alertTime;
            TransitionToState(GuardState.Alert);
        }
    }

    void UpdateAnimations()
    {
        // Update animation parameters based on state and speed
        float currentSpeed = navAgent.velocity.magnitude / navAgent.speed;
        animator.SetFloat(speedHash, currentSpeed);
        animator.SetBool(isAlertHash, currentState == GuardState.Alert);
        animator.SetBool(isChasingHash, currentState == GuardState.Chasing);
        animator.SetBool(isSuspiciousHash, isSuspicious || currentState == GuardState.Alert || currentState == GuardState.Chasing);

    }

    void CheckForPlayerDetection()
    {
        if (player == null) return;

        // Don't check if already chasing
        if (currentState == GuardState.Chasing) return;

        // Check if player is in sight
        if (CanSeePlayer())
        {
            // Player detected - chase!
            TransitionToState(GuardState.Chasing);
        }
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        // Check distance
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > sightRange) return false;

        // Check angle
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > sightAngle / 2) return false;

        // Check obstacles with raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer.normalized, out hit, distanceToPlayer))
        {
            if (hit.transform != player) return false;
        }

        // Check if player is crouching (optional - only if your player controller exposes this)
        FirstPersonController playerController = player.GetComponent<FirstPersonController>();
        if (playerController != null && playerController.IsCrouching())
        {
            // Harder to see player when crouching
            return distanceToPlayer < sightRange / 2;
        }

        return true;
    }

    void TransitionToState(GuardState newState)
    {
        // Exit current state
        switch (currentState)
        {
            case GuardState.Idle:
                break;

            case GuardState.Patrolling:
                break;

            case GuardState.Alert:
                break;

            case GuardState.Chasing:
                break;
        }

        // Set new state
        currentState = newState;

        // Enter new state
        switch (newState)
        {
            case GuardState.Idle:
                navAgent.isStopped = true;
                break;

            case GuardState.Patrolling:
                if (waypoints.Length > 0)
                {
                    SetDestination(waypoints[currentWaypointIndex].position);
                }
                break;

            case GuardState.Alert:
                navAgent.isStopped = true;
                alertTimer = alertTime;
                break;

            case GuardState.Chasing:
                if (player != null)
                {
                    SetDestination(player.position);
                }
                break;
        }
    }

    void SetDestination(Vector3 destination)
    {
        navAgent.isStopped = false;
        navAgent.SetDestination(destination);
    }

    // Called by Unity Editor to visualize the waypoints
    void OnDrawGizmos()
    {
        // Draw patrol path
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.blue;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            // Draw waypoint
            Gizmos.DrawSphere(waypoints[i].position, 0.3f);

            // Draw line to next waypoint
            if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
            else if (waypoints[0] != null) // Line from last to first
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
            }
        }

        // Draw sight cone
        if (Application.isPlaying && currentState != GuardState.Chasing)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position + Vector3.up, transform.forward * sightRange);
        }
    }
}