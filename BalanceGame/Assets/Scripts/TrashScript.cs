using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashScript : MonoBehaviour
{
    public Sprite[] trashSprites;
    private SpriteRenderer spriteRenderer;
    private DayTimer dayTimer;

    void Start()
    {
        // To controll the sprite
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetSprite(Random.Range(0, trashSprites.Length));

        // Get the timer
        dayTimer = GameObject.Find("GameManager").GetComponent<DayTimer>();
    }

    public void SetSprite(int index)
    {
        // Make sure the index is in range
        if (index >= 0 && index < trashSprites.Length)
        {
            spriteRenderer.sprite = trashSprites[index];  
        }
        else
        {
            Debug.LogError("Índice fuera de rango.");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
