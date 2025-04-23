using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceAttack : MonoBehaviour
{
    [SerializeField] public int policeThresholdMin = -5; 
    [SerializeField] public int policeThresholdMax = 5;

    public void SetThresholds(int minThreshold, int maxThreshold)
    {
        this.policeThresholdMin = minThreshold;
        this.policeThresholdMax = maxThreshold;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("The police officer catch someone.");
        PoliceController police = GetComponentInParent<PoliceController>();

        // If the police officer catch the player
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            // Depending on their reputation different things may happend
            if (player != null)
            {
                float playerReputation = player.reputation + player.aditionalReputation;
                
                // If their reputation is too low they will be send back to their cell and the day will be over
                if (playerReputation < policeThresholdMin)
                {
                    player.tpHome();
                    // Day end
                    GameObject.Find("GameManager").GetComponent<DayTimer>().FinishDay();
                } 
                // If it's low but not too low the player will only be send back to their prision
                else if(playerReputation >= policeThresholdMin && playerReputation < policeThresholdMax)
                {
                    player.tpHome();
                    GameObject.Find("GameManager").GetComponent<GameManager>().badActions ++;
                }
                // Else nothing happend (in fact if the player reputation is high enough the police should not chase the player)
                                
            }
        }
        // If the police officer catch an enemy it will be send back to their prision
        else if (collision.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();

            if (enemy != null)
            {
                enemy.SendToCell();
            }
        }

        if (police != null)
        {
            police.OnTargetCaptured(collision.transform);
        }
    }
}