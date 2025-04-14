using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRb;
    [SerializeField] private float MoveForce;
    [SerializeField] private int HP;
    [SerializeField] private Vector3 spawnPoint;

    void Start()
    {
        // Get components
        playerRb = GetComponent<Rigidbody2D>();

        // Set initial values
        HP = 5;
        MoveForce = 5;
        spawnPoint = transform.position;

    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        // In this case I prefered to use "translate" because I think it reflects better the movement in a top-down view
        //playerRb.AddForce(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * MoveForce * Time.deltaTime);
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * MoveForce * Time.deltaTime);
    }

    public void tpHome()
    {
        transform.position = spawnPoint;
    }
}
