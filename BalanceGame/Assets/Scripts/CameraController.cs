using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    private Collider2D mapBounds;
    private Transform player;
    private Vector3 initialPosition;
    private bool isInBounds = true;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        mapBounds = GameObject.Find("Map").GetComponent<Collider2D>();
        initialPosition = transform.position;  
    }

    void Update()
    {
        if (isInBounds)
        {
            Vector3 targetPosition = player.position;

            // Restringir la cámara al área de los límites (mapBounds)
            Vector3 clampedPosition = new Vector3(
                Mathf.Clamp(targetPosition.x, mapBounds.bounds.min.x, mapBounds.bounds.max.x),
                Mathf.Clamp(targetPosition.y, mapBounds.bounds.min.y, mapBounds.bounds.max.y),
                transform.position.z);

            transform.position = clampedPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == mapBounds)
        {
            isInBounds = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == mapBounds)
        {
            isInBounds = true;
        }
    }
}
