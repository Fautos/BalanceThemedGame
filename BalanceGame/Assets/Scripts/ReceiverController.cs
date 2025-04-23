using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiverController : MonoBehaviour
{
    private GameObject player;
    private LayerMask obstacleLayer;

    void Start()
    {
        player = GameObject.Find("Player");
        obstacleLayer = LayerMask.GetMask("Obstacle");   
    }

    // Update is called once per frame
    void Update()
    {
        LookAtPlayer();
    }

    private void LookAtPlayer()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 dirToPlayer = (player.transform.position - transform.position).normalized;
        if (!Physics2D.Raycast(transform.position, dirToPlayer, distance, obstacleLayer))
        {
            float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
        }
    }
}
