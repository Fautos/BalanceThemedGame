using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatScript : MonoBehaviour
{
    public bool catActive;
    [SerializeField] List<Sprite> catSprites;
    private List<Vector3> spawnPositions = new List<Vector3>{
                                                new Vector3(-2.5f, 5,0),
                                                new Vector3(-12,-16.5f,0),
                                                new Vector3(-22.5f, -7,0),
                                                new Vector3(-12.5f, 16.5f,0),
                                                new Vector3(1f, -18,0),
                                                new Vector3(-36.5f, -12,0),
                                                new Vector3(-11, -10,0),
                                            };
    private GameObject exclamationMark;
    private Transform player;
    private GameManager gameManager;
    private DayTimer dayTimer;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        catActive = false;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        dayTimer = GameObject.Find("GameManager").GetComponent<DayTimer>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        exclamationMark = transform.Find("ExclamationMark").gameObject;
        spriteRenderer = transform.GetComponent<SpriteRenderer>();

        if (exclamationMark.activeInHierarchy)
        {
            exclamationMark.SetActive(false);
        }

        // Set the spawn point
        transform.position = spawnPositions[Random.Range(0, spawnPositions.Count)];

        // And the sprite renderer to "sleeping cat"
        spriteRenderer.sprite = catSprites[0];
    }

    // Update is called once per frame
    void Update()
    {
        if ((Vector2.Distance(player.transform.position, transform.position) < 10) && catActive)
        {
            if (!exclamationMark.activeInHierarchy)
            {
                exclamationMark.SetActive(true);
            }
        }
        else
        {
            if (exclamationMark.activeInHierarchy)
            {
                exclamationMark.SetActive(false);
            }
        }   

        if (dayTimer.dayOver)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !catActive)
        {
            catActive = true;
            spriteRenderer.sprite = catSprites[1];
            gameManager.goodActions ++;
        }
    }
}
