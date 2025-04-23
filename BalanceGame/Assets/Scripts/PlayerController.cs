using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public bool inSpawn, canMove, canReceiveDamage, isInvisible, hasPackage;
    public float reputation, aditionalReputation;
    public LayerMask interactableLayer;
    private int _HP;
    private GameObject playerSprites, arm, box;
    private GameManager gameManager;
    private DayTimer dayTimer;
    [SerializeField] private bool canInteract;
    [SerializeField] private float MoveForce, interactDistance, attackCD;
    [SerializeField] private Vector3 spawnPoint;
    [SerializeField] public int maxHP = 5;
    [SerializeField] GameObject reputationEmoji, HPUI, ExclamationMark;
    [SerializeField] List<Sprite> reputationEmojis;
    public Sprite fullHeartSprite, emptyHeartSprite;
    [SerializeField] public int HP {get{return _HP;}
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
        arm = playerSprites.transform.Find("Arm").gameObject;
        box = playerSprites.transform.Find("Box").gameObject;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        dayTimer = GameObject.Find("GameManager").GetComponent<DayTimer>();
        HPUI = GameObject.Find("Canvas/PlayerInfo/PlayerHp");
        reputationEmoji = GameObject.Find("Canvas/PlayerInfo/PlayerReputation/ReputationEmoji");
        ExclamationMark = transform.Find("ExclamationMark").gameObject;

        if (ExclamationMark.activeInHierarchy)
        {
            ExclamationMark.SetActive(false);
        }

        // Set initial values
        maxHP = 5;
        HP = maxHP;
        MoveForce = 5;
        attackCD = 1.0f;
        interactDistance = 0.3f;
        canInteract = true;
        canMove = true;
        canReceiveDamage = true;
        inSpawn = true;
        isInvisible = false;
        hasPackage = false;
        spawnPoint = transform.position;
        reputation = 0;
        aditionalReputation = 0;

    }

    void FixedUpdate()
    {
        // If the player "can move" we let them move 
        if(canMove)
        {
            // First we disable the exclamation mark if it's enable
            if (ExclamationMark.activeInHierarchy && !inSpawn)
            {
                ExclamationMark.SetActive(false);
            }

            Move();

            // If their is less than 30 secs the player can end the day if they are in the spawn
            if(dayTimer.timeLeft <= 30 && inSpawn)
            {
                ExclamationMark.SetActive(true);
                if (Input.GetKey(KeyCode.Space))
                {
                    ExclamationMark.SetActive(false);
                    dayTimer.FinishDay();
                }
            }
            else if (Input.GetKey(KeyCode.Space))
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
            Vector2 origin = transform.position + new Vector3(0, 0.25f, 0);

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, interactDistance, interactableLayer);
            
            // For debugging
            Debug.DrawRay(origin, direction * interactDistance, Color.green, 0.3f);

            // If the player hits 
            if (hit.collider != null)
            {
                // Hay un objeto en el camino, tratamos de interactuar
                Debug.Log("Interacting with: " + hit.collider.name);

                hit.transform.gameObject.GetComponentInParent<IWindow>().StartMiniGame();
            }
            else
            {
                Attack();
            }
        }
        else if (canInteract == false && hasPackage == true)
        {
            // First we launch a Raycast to check if the player found the receiver
            Vector2 direction = playerSprites.transform.up;
            Vector2 origin = transform.position + new Vector3(0, 0.35f, 0);

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, interactDistance, LayerMask.GetMask("Receiver"));
            
            // For debugging
            Debug.DrawRay(origin, direction * interactDistance, Color.red, 0.3f);

            // If the player hits the receiver
            if (hit.collider != null)
            {
                Debug.Log("Interacting with: " + hit.collider.name);

                // The minigame is completed 
                if(box.activeInHierarchy)
                {
                    box.SetActive(false);
                    hasPackage = false;
                }

                canInteract = true;

                hit.transform.gameObject.SetActive(false);
            }
        }
                
    }

    void Attack()
    {
        // If the player is attacking they cannot interact with anything until the attack is finished
        canInteract = false;

        if (!arm.activeInHierarchy)
        {
            arm.SetActive(true);
            StartCoroutine(AttackCdCoroutine());
        }
        
    }

    IEnumerator AttackCdCoroutine()
    {
        float activeTime = 0.5f; // Time the arm stays active
        float remainingCooldown = Mathf.Max(attackCD - activeTime, 0f);

        yield return new WaitForSeconds(activeTime);
        arm.SetActive(false);
        yield return new WaitForSeconds(remainingCooldown);
        
        canInteract = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            inSpawn = true;
        }        
    }

    private void OnTriggerStay2D(Collider2D collision)
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

        // The additional reputation is only based on your actions, to show more respect to other people without affecting your final score
        aditionalReputation += -badActions + 0.5f*goodActions;

        // Finally we update the UI
        UpdateUI();

    }

    // Function to hide the player
    public void HidePlayer(bool hide)
    {
        // You hide the player when they interact with something
        if (hide)
        {
            isInvisible = true;
            canMove = false;
        }
        else
        {
            isInvisible = false;
            canMove = true;
        }
    }

    public void TakeDamage(int damage)
    {
        if(canReceiveDamage)
        {
            // If the player has the package, they loose it
            if(hasPackage)
            {
                ReceiveMail(false);
            }

            HP -= damage;
            FlashOnDamage();
            UpdateUI();
        }

        if (HP <= 0f)
        {
            Debug.Log("Game over, player is dead.");
            gameManager.GameOver();            
        }
    }

    // Flashing when damage received    
    public void FlashOnDamage()
    {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        // Invulneravility frames
        canReceiveDamage = false;

        // Flashing effect
        for (int i = 0; i < 3; i++)
        {
            foreach (var sprite in sprites)
                sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);

            foreach (var sprite in sprites)
                sprite.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        canReceiveDamage = true;
    }

    public void ReceiveMail(bool receiveMail)
    {
        if(receiveMail == true)
        {
            // If the player has the mail, the sprite should reflect it
            if(!box.activeInHierarchy)
            {
                box.SetActive(true);
                hasPackage = true;
            }

            // While the player has the mail they can't attack
            canInteract = false;

        }else
        {
            // If he loose the mail the sprite should be updates
            if(box.activeInHierarchy)
            {
                box.SetActive(false);
                hasPackage = false;
            }

            // They are again able to attack
            canInteract = true;

            // But the task should be restarted
            GameObject.Find("TaskController/DeliverMailTask").GetComponent<Task5_DeliverMailScript>().RestartMiniGame();

        }

    }

    public void UpdateUI()
    {
        // To update the Hp
        if (HPUI == null) return;

        for (int i = 0; i < HPUI.transform.childCount; i++)
        {
            GameObject heart = HPUI.transform.GetChild(i).gameObject;
            Image heartImage = heart.GetComponent<Image>();

            if (i < HP)
                heartImage.sprite = fullHeartSprite;
            else
                heartImage.sprite = emptyHeartSprite;
        }

        // To update the reputation emoji
        int emojiIndex = 0;

        if (reputation < -5)
        {
            emojiIndex = 0;
        } else if (reputation >= -5 && reputation < 0)
        {
            emojiIndex = 1;
        } else if (reputation >= 0 && reputation < 5)
        {
            emojiIndex = 2;
        } else if (reputation >= 5)
        {
            emojiIndex = 3;
        }

        reputationEmoji.GetComponent<Image>().sprite = reputationEmojis[emojiIndex];

    }

    #endregion

}
