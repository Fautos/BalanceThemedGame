using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public bool inSpawn, canMove;
    public float reputation, aditionalReputation;
    public LayerMask interactableLayer;
    private int _HP;
    private GameObject playerSprites;
    [SerializeField] private bool canInteract;
    [SerializeField] private float MoveForce, interactDistance, attackCD;
    [SerializeField] private Vector3 spawnPoint;
    [SerializeField] public int maxHP = 5;

    public int HP {get{return _HP;}
                    set{
                        if (value > maxHP)
                        {
                            Debug.LogWarning("Too much HP");
                            _HP = maxHP;
                        }
                        else
                        {
                            _HP = value;
                        }
                    } }

    void Start()
    {
        // Get components
        playerSprites = transform.Find("Sprites").gameObject;

        // Set initial values
        maxHP = 5;
        HP = maxHP;
        MoveForce = 5;
        attackCD = 0.5f;
        interactDistance = 1.5f;
        canInteract = true;
        canMove = true;
        inSpawn = true;
        spawnPoint = transform.position;
        reputation = 0;
        aditionalReputation = 0;

    }

    void FixedUpdate()
    {
        // If the player "can move" we let them move 
        if(canMove)
        {
            Move();

            if (Input.GetKey(KeyCode.Space))
            {
                Interact();
            }
        }
    }

    void Move()
    {
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(hInput, vInput, 0);

        // In this case I prefered to use "translate" because I think it reflects better the movement in a top-down view
        transform.Translate(inputDirection.normalized * MoveForce * Time.deltaTime);

        // Then rotate the sprite
        if (inputDirection.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(vInput, hInput) * Mathf.Rad2Deg;
            playerSprites.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }

    void Interact()
    {
        if (canInteract)
        {
            // First we launch a Raycast to check if there is something iteractable
            Vector2 direction = playerSprites.transform.up;
            Vector2 origin = transform.position;

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, interactDistance, interactableLayer);
            
            // For debugging
            Debug.DrawRay(origin, direction * interactDistance, Color.green, 0.5f);

            if (hit.collider != null)
            {
                // Hay un objeto en el camino, tratamos de interactuar
                Debug.Log("Interacting with: " + hit.collider.name);

                /*/ Intenta obtener un componente de tipo Interactable (puedes definir tu propio script)
                var interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(); // Llama a la interacción
                    return;
                }

                // Si quieres basarte en tags:
                // if (hit.collider.CompareTag("Interactuable")) { ... }*/
            }
            else
            {
                Attack();
            }
        }
                
    }

    void Attack()
    {
        // If the player is attacking they cannot interact with anything until the attack is finished
        canInteract = false;

        if (!playerSprites.transform.Find("Arm").gameObject.activeInHierarchy)
        {
            playerSprites.transform.Find("Arm").gameObject.SetActive(true);
            StartCoroutine(AttackCdCoroutine());
        }
        
    }

    IEnumerator AttackCdCoroutine()
    {
        yield return new WaitForSeconds(attackCD);
        // When the attack is finished the player can interact again
        canInteract = true;
        playerSprites.transform.Find("Arm").gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            inSpawn = true;
        }        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            inSpawn = false;
        }
    }

    #region public functions
    // Function to go back to base
    public void tpHome()
    {
        transform.position = spawnPoint;
    }

    public void UpdateReputation(int taskCompleted, int taskPerDay, int goodActions, int badActions)
    {
        // To compute the reputation balance at the end of the day we follow the next rules
        reputation += 2*taskCompleted - (taskPerDay-taskCompleted);
        reputation += goodActions - 2*badActions;

        // Tha additional reputation is only based on your actions, to show more respect to other people without affecting your final score
        aditionalReputation += -badActions + 0.5f*goodActions;

    }
    
    #endregion

}
