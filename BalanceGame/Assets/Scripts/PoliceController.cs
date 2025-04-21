using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding; // Asumiendo que seguirás usando Pathfinding igual que con EnemyController

public class PoliceController : MonoBehaviour
{
    #region Public Configuration
    public bool isActive = true;
    public float viewRadius = 8f;
    public LayerMask prisonerLayer;
    public LayerMask obstacleLayer;
    public float reactionDelay = 0.5f;
    public float maxChaseDistance = 150f;
    public float walkSpeed = 5000f;
    public float chaseSpeed = 7500f; // Más rápido en persecución
    public float nextWaypointDistance = 1.5f;

    // Reputación thresholds
    public float reputationThresholdSendToCell = 50f;
    public float reputationThresholdEndDay = 30f;
    #endregion

    #region Private Fields
    private enum PoliceState { Idle, Reacting, Chasing, Returning }
    private PoliceState currentState = PoliceState.Idle;

    private List<Transform> targets = new List<Transform>();
    private Transform currentTarget;
    private Transform initialPosition;
    private Vector3 originalPosition;
    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath;
    private Seeker seeker;
    private Rigidbody2D rb;
    private GameObject policeSprites;
    private GameObject policeArm;
    private Transform player;
    private bool isReacting = false;
    #endregion

    #region Public Configuration
    public float reputationThresholdCapture = 50f;
    public float reputationThresholdPunish = 30f;
    #endregion

    #region Private Fields
    private List<Transform> currentTargets = new List<Transform>();
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        initialPosition = transform;
        originalPosition = transform.position;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        policeSprites = transform.Find("PolicemanGFX")?.gameObject;
        policeArm = policeSprites.transform.Find("Policeman_Arm")?.gameObject;
        policeArm.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        prisonerLayer = LayerMask.GetMask("Enemy");
        obstacleLayer = LayerMask.GetMask("Obstacle");

        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
    }

    private void FixedUpdate()
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
                CheckChaseConditions();
                break;

            case PoliceState.Returning:
                if (path != null) MoveAlongPath();
                if (Vector2.Distance(transform.position, originalPosition) < 0.5f)
                    currentState = PoliceState.Idle;
                break;
        }
    }
    #endregion

    #region Conflict Detection
    private void DetectConflicts()
    {
        Collider2D[] conflictsInRange = Physics2D.OverlapCircleAll(transform.position, viewRadius, prisonerLayer);

        foreach (Collider2D conflict in conflictsInRange)
        {
            Transform arm = conflict.transform.Find("EnemyGFX/Enemy_Arm"); // O equivalente para los presos
            if (arm != null && arm.gameObject.activeSelf)
            {
                Vector2 dirToConflict = (conflict.transform.position - transform.position).normalized;
                float distanceToConflict = Vector2.Distance(transform.position, conflict.transform.position);

                if (!Physics2D.Raycast(transform.position, dirToConflict, distanceToConflict, obstacleLayer))
                {
                    if (!targets.Contains(conflict.transform))
                        targets.Add(conflict.transform);

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
            return;
        }

        // Elegir el objetivo más cercano
        currentTarget = GetClosestTarget();
        UpdatePath();
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
        if (distance > maxChaseDistance)
        {
            targets.Clear();
            currentTarget = null;
            currentState = PoliceState.Returning;
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

    #region Arm Interaction
    public bool IsTarget(Transform target)
    {
        return currentTargets.Contains(target);
    }

    public void PunishTarget(Transform target)
    {
        if (target.CompareTag("Player"))
        {
            PlayerController playerController = target.GetComponent<PlayerController>();

            if (playerController != null)
            {
                float totalReputation = playerController.reputation + playerController.aditionalReputation;

                if (totalReputation <= reputationThresholdPunish)
                {
                    // Terminar el día (puedes cambiar esto por tu propio sistema)
                    Debug.Log("Policía ha capturado al jugador: Día terminado.");
                    // Aquí iría tu sistema para terminar el día
                }
                else if (totalReputation <= reputationThresholdCapture)
                {
                    // Solo enviar a la celda
                    Debug.Log("Policía ha capturado al jugador: Enviado a celda.");
                    playerController.tpHome();
                }
                else
                {
                    // Alta reputación: Solo castigan al preso
                    Debug.Log("Jugador tiene buena reputación. No castigado.");
                }
            }
        }
        else if (target.CompareTag("Enemy"))
        {
            EnemyController enemyController = target.GetComponent<EnemyController>();

            if (enemyController != null)
            {
                Debug.Log("Policía ha capturado a un prisionero.");
                enemyController.SendToCell();
            }
        }

        // Después de castigar, eliminamos al objetivo de la lista
        currentTargets.Remove(target);

        // Si ya no hay más objetivos, volver al puesto
        if (currentTargets.Count == 0)
        {
            ReturnToPost();
        }
    }

    private void ReturnToPost()
    {
        currentTarget = initialPosition;
        currentState = PoliceState.Returning;
        UpdatePath();
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
}

