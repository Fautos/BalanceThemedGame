using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pathfinding;
using System.Linq;

// Tutorial for the pathfinding: https://www.youtube.com/watch?v=jvtFUfJ6CP8
public class EnemyController : MonoBehaviour
{
    #region Public Configuration
    public bool isActive = true;
    public float health = 5f, currentHealth;
    public float reputationThreshold = 50f;
    public float respawnTime = 10f;
    public float speed = 10000f;
    public float nextWaypointDistance = 1.5f;
    public float viewRadius = 5f;
    public float viewAngle = 90f;
    public Transform enemyGFX;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    // Combat settings
    [SerializeField] private float attackCooldown = 1.0f; // Cooldown between attacks
    [SerializeField] private float attackRange = 2.5f;    // Distance needed to attack
    private bool canAttack = true;

    // Graphics
    private GameObject enemyArm; // Reference to the enemy's arm
    #endregion

    #region Private Fields
    private Transform player;
    private List<Vector3> waypointsRoute = new List<Vector3>();
    private int indexRoute = 0;
    private Vector3 currentTarget;

    private enum EnemyState { Moving, Waiting, Chasing }
    private EnemyState currentState = EnemyState.Moving;
    private Coroutine waitNewWaypoint;

    private bool isAttacking = false;
    private bool isKnockedOut = false;
    private bool isChasing = false;

    private Vector3 spawnPoint;
    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath;
    private Seeker seeker;
    private Rigidbody2D rb;
    private GameObject enemySprites;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        // Initialize references
        spawnPoint = transform.position;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        enemySprites = transform.Find("EnemyGFX")?.gameObject;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        enemyArm = enemySprites.transform.Find("Enemy_Arm").gameObject;
        enemyArm.SetActive(false); // Make sure it's deactivated at start
        currentHealth = health;
        playerLayer = LayerMask.GetMask("Player");
        obstacleLayer = LayerMask.GetMask("Obstacle");

        // Load route waypoints
        LoadWaypoints();

        if (waypointsRoute.Count > 0)
        {
            indexRoute = 0;
            currentTarget = waypointsRoute[indexRoute];
        }

        // Start pathfinding updates
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
    }

    private void FixedUpdate()
    {
        if (!isActive || isKnockedOut || waypointsRoute.Count == 0) return;

        switch (currentState)
        {
            case EnemyState.Moving:
                DetectPlayer();
                if (path != null) MoveAlongPath();
                break;

            case EnemyState.Waiting:
                DetectPlayer();
                break;

            case EnemyState.Chasing:
                CheckChaseConditions();
                if (path != null) MoveAlongPath();
                TryAttackPlayer();
                break;
        }
    }
    #endregion

    #region Initialization
    private void LoadWaypoints()
    {
        Transform route = transform.Find("Route_waypoints");
        if (route == null) return;

        List<Transform> waypointObjects = new List<Transform>();
        foreach (Transform waypoint in route)
            waypointObjects.Add(waypoint);

        waypointObjects = waypointObjects.OrderBy(wp => wp.name).ToList();

        foreach (Transform wp in waypointObjects)
            waypointsRoute.Add(wp.position);
    }
    #endregion

    #region Movement
    private void MoveAlongPath()
    {
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;

            if (currentState == EnemyState.Moving)
            {
                currentState = EnemyState.Waiting;
                if (waitNewWaypoint == null)
                    waitNewWaypoint = StartCoroutine(WaypointReachDelay());
            }

            return;
        }

        reachedEndOfPath = false;

        if (!isAttacking)
            Move();
    }

    private void Move()
    {
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
            currentWaypoint++;

        RotateSprite(force);
    }

    private IEnumerator WaypointReachDelay()
    {
        yield return new WaitForSeconds(5f);

        indexRoute = (indexRoute + 1) % waypointsRoute.Count;
        currentTarget = waypointsRoute[indexRoute];
        currentState = EnemyState.Moving;
        waitNewWaypoint = null;
        UpdatePath();
    }
    #endregion

    #region Pathfinding
    private void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, currentTarget, OnPathComplete);
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

    #region Player Detection and Combat
    private void DetectPlayer()
    {
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, playerLayer);

        foreach (Collider2D target in targetsInViewRadius)
        {
            Vector2 dirToTarget = (target.transform.position - transform.position).normalized;
            if (Vector2.Angle(transform.up, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector2.Distance(transform.position, target.transform.position);

                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleLayer))
                {
                    PlayerController playerController = target.GetComponent<PlayerController>();
                    float playerReputation = playerController.reputation + playerController.aditionalReputation;

                    if (playerReputation < reputationThreshold)
                    {
                        isChasing = true;
                        currentState = EnemyState.Chasing;
                        currentTarget = player.position;

                        if (waitNewWaypoint != null)
                        {
                            StopCoroutine(waitNewWaypoint);
                            waitNewWaypoint = null;
                        }

                        UpdatePath();
                        return;
                    }
                }
            }
        }
    }

    private void CheckChaseConditions()
    {
        if (player == null) return;

        PlayerController playerController = player.GetComponent<PlayerController>();
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > viewRadius * 2 || playerController.inSpawn || playerController.isInvisible)
        {
            isChasing = false;
            currentState = EnemyState.Moving;
            currentTarget = waypointsRoute[indexRoute];
            UpdatePath();
        }
        else
        {
            currentTarget = player.position;
            // Path update is handled in InvokeRepeating
        }
    }

    void TryAttackPlayer()
    {
        if (!canAttack || isKnockedOut || !isActive) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    IEnumerator AttackCoroutine()
    {
        canAttack = false;
        isAttacking = true;

        // Show the arm (attack animation)
        enemyArm.SetActive(true);

        // Short attack window
        yield return new WaitForSeconds(0.5f);

        enemyArm.SetActive(false);

        // Wait for the rest of the cooldown
        yield return new WaitForSeconds(attackCooldown - 0.5f);

        canAttack = true;
        isAttacking = false;
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        FlashOnDamage(); // New: start flashing when taking damage

        if (!isKnockedOut)
        {
            // Force chase when taking damage
            isChasing = true;
            currentState = EnemyState.Chasing;
            currentTarget = player.position;
            UpdatePath();
        }

        if (currentHealth <= 0f && !isKnockedOut)
        {
            isKnockedOut = true;
            isActive = false;
            Invoke(nameof(RecoverFromKnockout), respawnTime);
        }
    }

    private void RecoverFromKnockout()
    {
        // When recover the enemy goes back to their prison
        SendToCell();

        //currentHealth = health;
        //isKnockedOut = false;
        //isActive = true;
    }

    public void SendToCell()
    {
        transform.position = spawnPoint;
        isActive = false;
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

        if (enemySprites != null)
            enemySprites.transform.rotation = transform.rotation;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || player == null) return;

        // Draw view radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // Draw view angle
        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);

        // Draw chase line
        if (isChasing)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }

    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.z + 90;

        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    public void FlashOnDamage()
    {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < 3; i++)
        {
            foreach (var sprite in sprites)
                sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);

            foreach (var sprite in sprites)
                sprite.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion
}
