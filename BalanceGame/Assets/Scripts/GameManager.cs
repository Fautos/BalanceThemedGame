using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class GameManager : MonoBehaviour
{

    [SerializeField] bool gameOver = false;
    [SerializeField] int day, maxDays, dayStage;
    [SerializeField] float karma;
    [SerializeField] DayTimer dayTimer;
    [SerializeField] PlayerController playerController;
    [SerializeField] MiniTaskController taskController;

    private void Start()
    {
        // Get components
        dayTimer = GameObject.Find("GameManager").GetComponent<DayTimer>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        taskController = GameObject.Find("TaskController").GetComponent<MiniTaskController>();

        // Initialize some variables
        day = 1;
        maxDays = 5;
        karma = 0;
        dayStage = 0;

    }

    private void FixedUpdate()
    {
        Gameloop();

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            dayTimer.FinishDay();
        }
    }

    // Gameloop
    private void Gameloop()
    {
        if (!gameOver)
        {
            // First we check the day, if it's the last day the game is over
            if (day < maxDays)
            {
                // We control the game with this cycle
                switch (dayStage)
                {
                    // If it's in the morning (dayStage = 0)
                    case 0:
                    {
                        Dawn();
                        dayStage = 1;
                        break;
                    }
                    // If it's in the midday (dayStage = 1)
                    case 1:
                    {
                        Midday();

                        if (dayTimer.dayOver)
                        {
                            dayStage = 2;
                        }
                        break;
                    }
                    // If it's in the night (dayStage = 2)
                    case 2:
                    {
                        //Dusk();

                        if (Input.GetKeyUp(KeyCode.V))
                        {
                            day ++;
                            dayStage = 0;
                        }

                        break;
                    }
                }

            }
            else
            {
                gameOver = true;
            }
        }
        else
        {
            // If the game is over we show the final score
            Debug.Log("GameOver");
        }

    }

    // This function set the start of the game
    private void Dawn()
    {
        // Select random duties

        // Move to spawn and recover health
        playerController.tpHome();

        // Start timer
        dayTimer.StartTimer();

        // Start playing
    }

    // This function controls the part of the game when you control your character
    private void Midday()
    {

    }

    // This function is activated when the day is over and it should performs the final checks before moving to the next day
    private void Dusk()
    {
        // Fade the screen 
        
        // Calculate the results of the day and update karma
        
        // Show the results

        // Day +1
        day ++;

        // Wait the player to press continue
        Debug.Log("Day finished");
    }

}
