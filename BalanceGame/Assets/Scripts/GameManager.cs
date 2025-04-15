using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System.Data.Common;


public class GameManager : MonoBehaviour
{

    [SerializeField] bool gameOver = false;
    [SerializeField] int day, maxDays, dayStage;
    [SerializeField] float karma;
    [SerializeField] DayTimer dayTimer;
    [SerializeField] PlayerController playerController;
    [SerializeField] MiniTaskController taskController;
    [SerializeField] GameObject DayOverScreen;
    [SerializeField] DayOverScreenManager dayOverScreenManager;
    [SerializeField] TMP_Text dayCounterText;

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

        // Make sure the game over and day over screens are off
        dayOverScreenManager = new DayOverScreenManager(DayOverScreen);

    }

    private void FixedUpdate()
    {
        Gameloop();

        // Borrar: Prueba para parar timer
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
                        Dusk();
                        dayStage = 3;

                        break;
                    }
                    // Condition to pass the day
                    case 3:
                    {
                        // Once the day over screen is on, you have to press the key "E" to pass the day
                        if (Input.GetKey(KeyCode.E))
                        {
                            day ++;
                            dayStage = 0;
                            dayOverScreenManager.DisableScreen();
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
        taskController.GenerateTasks(3);

        // Move to spawn and recover health
        playerController.tpHome();

        if (playerController.HP < playerController.maxHP)
        {
            playerController.HP ++;
        }

        // Start timer and update UI
        dayTimer.StartTimer();
        UpdateUI();

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
        int taskResult = taskController.completedTask;

        // Show the results
        dayOverScreenManager.ActivateScreen(taskResult, 1, 0);

        // Wait the player to press continue
        Debug.Log("Day finished");
    }

    private void UpdateUI()
    {
        dayCounterText.text = "Day "+ day;
    }

}
