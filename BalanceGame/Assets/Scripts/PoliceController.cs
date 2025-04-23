using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PoliceController : MonoBehaviour, IGeneralCharacter
{
    #region Public Configuration
    public bool isActive = true;
    public float viewRadius = 15;
    public LayerMask conflictLayer;
    public LayerMask obstacleLayer;
    public float reactionDelay = 0.5f;
    public float maxChaseDistance = 15;
    public float walkSpeed = 12000;
    public float chaseSpeed = 30000;
    public float nextWaypointDistance = 1.5f;

    // Reputación thresholds
    public int policeThresholdMin = -5;
    public int policeThresholdMax = 5;
    #endregion

    #region Private Fields
    private enum PoliceState { Idle, Reacting, Chasing, Returning }
    private PoliceState currentState = PoliceState.Idle;
    private List<Transform> targets = new List<Transform>();
    private Transform currentTarget;
    private Vector3 originalPosition;
    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath;
    private Seeker seeker;
    private Rigidbody2D rb;
    private GameObject policeSprites;
    private GameObject policeArm;
    private Transform player;
    private PlayerController playerController;
    private bool isReacting = false;
    private DayTimer dayTimer;
    private GameManager gameManager;
    private Coroutine reactCoroutineRef;
    private bool reactCoroutineRunning = false;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        originalPosition = transform.position;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        dayTimer = GameObject.Find("GameManager").GetComponent<DayTimer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        // Police components
        policeSprites = transform.Find("PolicemanGFX")?.gameObject;
        policeArm = policeSprites.transform.Find("Policeman_Arm")?.gameObject;
        policeArm.GetComponent<PoliceAttack>().SetThresholds(policeThresholdMin, policeThresholdMax);
        policeArm.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerController = player.GetComponent<PlayerController>();

        conflictLayer = LayerMask.GetMask("Enemy") | LayerMask.GetMask("Player");
        obstacleLayer = LayerMask.GetMask("Obstacle");

        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
    }

    private void FixedUpdate()
    {
        if (!dayTimer.dayOver && !gameManager.gameOver)
        {
            if (!isActive) return;
            
            switch (currentState)
            {
                case PoliceState.Idle:
                    DetectConflicts();
                    LookAtPlayer();
                    break;

                case PoliceState.Reacting:
                    // Quietos durante el retraso
                    break;

                case PoliceState.Chasing:
                    if (path != null) MoveAlongPath();
                    //DetectConflicts();
                    CheckChaseConditions();
                    break;

                case PoliceState.Returning:
                    if (path != null) MoveAlongPath();
                    //DetectConflicts();
                    if (Vector2.Distance(transform.position, originalPosition) < 2.5f)
                    {
                        currentState = PoliceState.Idle;
                        isReacting = false;
                        policeArm.SetActive(false); 
                        targets.Clear(); 
                    }
                    break;
            }
        }
        else if (dayTimer.dayOver)
        {
            ResetNPC();
        }
    }
    #endregion

    #region Conflict Detection
    private void DetectConflicts()
    {
        // 1. Limpiamos targets inválidos
        targets.RemoveAll(t => {
            if (t == null) return true;
            Transform arm = t.transform.Find("EnemyGFX/Enemy_Arm");
            return arm == null || !arm.gameObject.activeSelf;
        });

        // 2. Detectamos nuevos conflictos
        Collider2D[] conflictsInRange = Physics2D.OverlapCircleAll(transform.position, viewRadius, conflictLayer);

        foreach (Collider2D conflict in conflictsInRange)
        {
            Transform arm = conflict.transform.Find("EnemyGFX/Enemy_Arm");
            if (arm != null && arm.gameObject.activeSelf)
            {
                Vector2 dirToConflict = (conflict.transform.position - transform.position).normalized;
                float distanceToConflict = Vector2.Distance(transform.position, conflict.transform.position);

                if (!Physics2D.Raycast(transform.position, dirToConflict, distanceToConflict, obstacleLayer))
                {
                    if (!targets.Contains(conflict.transform))
                        targets.Add(conflict.transform);
                        
                        // If the player reputation is low enough they will be chased aswell
                        if((playerController.reputation + playerController.aditionalReputation) < policeThresholdMax && !targets.Contains(player))
                        {
                            targets.Add(player);
                        }

                    if (!isReacting)
                        StartCoroutine(ReactToConflict());

                    break;
                }
            }
        }
    }
    #endregion

    #region Reacting and Chasing
    private IEnumerator ReactToConflict()
    {
        isReacting = true;
        currentState = PoliceState.Reacting;

        yield return new WaitForSeconds(reactionDelay);

        ChooseNextTarget();
        currentState = PoliceState.Chasing;
        policeArm.SetActive(true);
        UpdatePath();
    }

    private void ChooseNextTarget()
    {
        if (targets.Count == 0)
        {
            currentState = PoliceState.Returning;
            policeArm.SetActive(false);
            currentTarget = null;
            currentWaypoint = 0;
            UpdatePathToPosition(originalPosition);
        }
        else
        {
            currentTarget = GetClosestTarget();
            UpdatePath();
        }
    }

    private Transform GetClosestTarget()
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform target in targets)
        {
            float distance = Vector2.Distance(transform.position, target.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = target;
            }
        }

        return closest;
    }

    private void CheckChaseConditions()
    {
        if (currentTarget == null)
        {
            ChooseNextTarget();
            return;
        }

        float distance = Vector2.Distance(transform.position, originalPosition);
        Debug.Log(distance);
        if (distance > maxChaseDistance)
        {
            targets.Clear();
            currentTarget = null;
            currentState = PoliceState.Returning;
            policeArm.SetActive(false);
            UpdatePathToPosition(originalPosition);
        }
    }

    public void OnTargetCaptured(Transform capturedTarget)
    {
        if (targets.Contains(capturedTarget))
            targets.Remove(capturedTarget);

        if (targets.Count > 0)
        {
            ChooseNextTarget();
            currentState = PoliceState.Chasing;
        }
        else
        {
            currentState = PoliceState.Returning;
            currentTarget = null;
            policeArm.SetActive(false);
            UpdatePathToPosition(originalPosition);
        }
    }

    #endregion

    #region Movement
    private void MoveAlongPath()
    {
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }

        reachedEndOfPath = false;

        Move();
    }

    private void Move()
    {
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        float speed = (currentState == PoliceState.Chasing) ? chaseSpeed : walkSpeed;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
            currentWaypoint++;

        RotateSprite(force);
    }

    private void UpdatePath()
    {
        if (seeker.IsDone() && currentTarget != null)
            seeker.StartPath(rb.position, currentTarget.position, OnPathComplete);
    }

    private void UpdatePathToPosition(Vector3 position)
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, position, OnPathComplete);
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    #endregion

    #region Graphics and Rotation
    private void RotateSprite(Vector3 moveDir)
    {
        if (moveDir.magnitude <= 0.01f) return;

        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);

        float rotationSpeed = 360f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (policeSprites != null)
            policeSprites.transform.rotation = transform.rotation;
    }

    private void LookAtPlayer()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= viewRadius)
        {
            Vector2 dirToPlayer = (player.position - transform.position).normalized;
            if (!Physics2D.Raycast(transform.position, dirToPlayer, distance, obstacleLayer))
            {
                float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);

                if (policeSprites != null)
                    policeSprites.transform.rotation = transform.rotation;
            }
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
    #endregion

    #region IGeneralCharacter functions
    public void StopNPC(bool stop)
    {
        isActive = !stop;

        if (stop)
        {
            rb.velocity = Vector2.zero;

            if (reactCoroutineRunning && reactCoroutineRef != null)
            {
                StopCoroutine(reactCoroutineRef);
                reactCoroutineRunning = false;
            }
        }
    }

    public void ResetNPC()
    {
        if (reactCoroutineRunning && reactCoroutineRef != null)
        {
            StopCoroutine(reactCoroutineRef);
            reactCoroutineRunning = false;
        }
        
        // Resetear estados
        isActive = true;
        isReacting = false;
        currentState = PoliceState.Idle;
        targets.Clear();
        currentTarget = null;
        currentWaypoint = 0;
        path = null;

        policeArm.SetActive(false);

        // Resetear posición y velocidad
        transform.position = originalPosition;
        rb.velocity = Vector2.zero;
    }
    
    #endregion
}

