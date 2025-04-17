using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System.Data.Common;


public class GameManager : MonoBehaviour
{

    [SerializeField] bool gameOver = false;
    [SerializeField] int day, maxDays, dayStage, taskPerDay, goodActions, badActions;
    [SerializeField] float karma;
    [SerializeField] DayTimer dayTimer;
    [SerializeField] PlayerController playerController;
    [SerializeField] MiniTaskController taskController;
    [SerializeField] GameObject DayOverScreen, spawnIndicator;
    [SerializeField] DayOverScreenManager dayOverScreenManager;
    [SerializeField] TMP_Text dayCounterText;

    private void Start()
    {
        // Get components
        dayTimer = GameObject.Find("GameManager").GetComponent<DayTimer>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        taskController = GameObject.Find("TaskController").GetComponent<MiniTaskController>();
        spawnIndicator = GameObject.Find("Player/TargetIndicatorSpawnPoint").gameObject;

        // Initialize some variables
        day = 1;
        maxDays = 5;
        karma = 0;
        dayStage = 0;
        taskPerDay = 1;
        ResetDayVariables();

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
                        if (Input.GetKey(KeyCode.Space))
                        {
                            day ++;
                            dayStage = 0;
                            ResetDayVariables();
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
        // First we turn off the spawn indicator
        if (spawnIndicator.activeInHierarchy)
        {
            spawnIndicator.SetActive(false);
        } 

        // Select random duties
        taskController.GenerateTasks(taskPerDay);

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
        playerController.canMove = true;
    }

    // This function controls the part of the game when you control your character
    private void Midday()
    {
        // During the midday we should check if any of the tasks is completed and the actions of the player
        taskController.CheckTaskCompleted();

        // If all the tasks are completed the player can go rest
        if (taskController.completedTask == taskPerDay)
        {
            Debug.Log("All tasks of the day are completed");
        }

        // If there is less than 30 sec left we indicate the player that they should go to bed
        if (!spawnIndicator.activeInHierarchy && dayTimer.timeLeft <= 30)
        {
            spawnIndicator.SetActive(true);
        } else if(spawnIndicator.activeInHierarchy && dayTimer.timeLeft > 30)
        {
            spawnIndicator.SetActive(false);
        }
    }

    // This function is activated when the day is over and it should performs the final checks before moving to the next day
    private void Dusk()
    {
        // Finish all the task in progress
        taskController.FinishDayTasks();

        // Dont allow the player to move
        playerController.canMove = false;

        // If the player ends the day in their room that's consider a good action, else it will be a bad action
        if (playerController.inSpawn)
        {
            goodActions ++;
        }
        else
        {
            badActions ++;
        }

        // Calculate the results of the day and update karma
        int taskResult = taskController.completedTask;

        // Show the results and add the reputation to the player
        dayOverScreenManager.ActivateScreen(taskResult, taskPerDay, goodActions, badActions);
        playerController.UpdateReputation(taskResult, taskPerDay, goodActions, badActions);

        // Wait the player to press continue
        Debug.Log("Day finished");
    }

    private void UpdateUI()
    {
        dayCounterText.text = "Day "+ day;
    }

    private void ResetDayVariables()
    {
        goodActions = 0;
        badActions = 0;
    }

}
