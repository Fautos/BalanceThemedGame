using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pathfinding;
//using System.Numerics;

// Tutorial for the pathfinding: https://www.youtube.com/watch?v=jvtFUfJ6CP8
public class EnemyController : MonoBehaviour
{
    public Transform target; 
    public Transform enemyGFX;
    public float speed;
    public float nextWaypointDistance = 1f;
    
    // Pathfinding variables
    private Path path;
    private int currentWaypoint;
    public bool reachedEndOfPath;
    private Seeker seeker;
    private Rigidbody2D rb;
    private GameObject enemySprites;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize some variables
        speed = 10000;
        currentWaypoint = 0;
        reachedEndOfPath = false;

        // Get the components
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        enemySprites = transform.Find("EnemyGFX").gameObject;

        // Pathfinding
        InvokeRepeating("UpdatePath", 0, 0.5f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // For the pathfinding
        if (path == null)
        {
            return;
        }
        if(currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else{
            reachedEndOfPath = false;
        }

        Move();
    }

    #region Enemy actions functions
    void Move()
    {
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // To rotate the sprite
        rotateSprite(force);
    }

    #endregion

    #region Pathfinding functions
    // Pathfinding function
    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, target.position, OnPathComplete);
        }
    }

    // Callback for the seeker function
    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    #endregion

    #region Sprites functions
    void rotateSprite(Vector3 moveDir)
    {
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        enemySprites.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
    #endregion
}
