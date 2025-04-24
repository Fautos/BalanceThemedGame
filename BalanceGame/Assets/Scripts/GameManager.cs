using System.Collections;
using UnityEngine;

using TMPro;

// For SceneManager.LoadScene
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int day, goodActions, badActions;
    [SerializeField] public bool gameOver = false, debugMode;
    [SerializeField] int maxDays, dayStage, taskPerDay;
    [SerializeField] DayTimer dayTimer;
    [SerializeField] PlayerController playerController;
    [SerializeField] MiniTaskController taskController;
    [SerializeField] GameObject DayOverScreen, GameOverScreen, spawnIndicator, catPrefab;
    [SerializeField] DayOverScreenManager dayOverScreenManager;
    [SerializeField] GameOverScreenManager gameOverScreenManager;
    [SerializeField] TMP_Text dayCounterText;

    private void Start()
    {
        // Get components
        dayTimer = GameObject.Find("GameManager").GetComponent<DayTimer>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        taskController = GameObject.Find("TaskController").GetComponent<MiniTaskController>();
        spawnIndicator = GameObject.Find("Player/TargetIndicatorSpawnPoint").gameObject;
        catPrefab = Resources.Load<GameObject>("Prefabs/Cat");

        // Initialize some variables
        debugMode = false;
        day = 1;
        maxDays = 5;
        dayStage = 0;
        taskPerDay = 3;
        ResetDayVariables();

        // Make sure the game over and day over screens are off
        dayOverScreenManager = new DayOverScreenManager(DayOverScreen);

        // And the game over screen
        gameOverScreenManager = new GameOverScreenManager(GameOverScreen);

    }

    private void FixedUpdate()
    {
        Gameloop();

        // Borrar: Prueba para parar timer
        if (Input.GetKeyUp(KeyCode.LeftShift) && debugMode)
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
            if (day <= maxDays)
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
                        // Once the day over screen is on, you have to press the key "Enter" to pass the day
                        if (Input.GetKey(KeyCode.Return))
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
            GameOver();
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

        // Select random duties. The first two days only 2 task are avaliable
        if(day <= 2)
        {
            taskPerDay = 2;
        }
        else
        {
            taskPerDay = 3;
        }

        taskController.GenerateTasks(taskPerDay);

        // Move to spawn and recover health
        playerController.tpHome();

        if (playerController.HP < playerController.maxHP)
        {
            playerController.HP+=2;
            playerController.UpdateUI();
        }

        // Start timer and update UI
        dayTimer.StartTimer();
        UpdateUI();

        // And enable the cat
        CatSpawn();

        // Start playing
        playerController.HidePlayer(false);
    }

    // This function controls the part of the game when you control your character
    private void Midday()
    {
        // During the midday we should check if any of the tasks is completed and the actions of the player
        taskController.CheckTaskCompleted();

        // If all the tasks are completed the player can go rest
        if (taskController.completedTask == taskPerDay)
        {
            //Debug.Log("All tasks of the day are completed");
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
        playerController.HidePlayer(true);

        // If the player ends the day in their room that's consider a good action, else it will be a bad action
        if (playerController.inSpawn)
        {
            goodActions ++;
        }
        else
        {
            badActions ++;
        }

        // Show the results and add the reputation to the player
        int taskResult = taskController.completedTask;
        dayOverScreenManager.ActivateScreen(taskResult, taskPerDay, goodActions, badActions);
        playerController.UpdateReputation(taskResult, taskPerDay, goodActions, badActions);

        // Wait the player to press continue
        Debug.Log("Day finished");
    }

    public void GameOver()
    {   
        // If the agem is over we launch the game over screen with the results
        Debug.Log("GameOver");
        gameOver = true;
        dayTimer.FinishDay();

        // Dont allow the player to move
        playerController.HidePlayer(true);

        // The end game text
        gameOverScreenManager.ActivateScreen(playerController.HP, playerController.reputation);

        if(Input.GetKeyDown(KeyCode.Return))
        {
            LoadScene(1);
        }
    }

    void CatSpawn()
    {
        int probability = 2;

        if(day <= 2)
        {
            // If we are in day 1 or 2 there is a 50% probability of spawning a cat
            probability = 2;
        }
        else
        {
            // Else there is a 66%
            probability = 3;
        }

        if(Random.Range(0, probability) > 0)
        {
            Instantiate(catPrefab, transform.position, Quaternion.identity);
        }
    }

    private void UpdateUI()
    {
        dayCounterText.text = "Day "+ day +"/5";
    }

    private void ResetDayVariables()
    {
        goodActions = 0;
        badActions = 0;
    }

    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        while (!operation.isDone)
        {
            yield return null;
        }
    }

}

public interface IGeneralCharacter
{
    // Function to stop the NPC
    void StopNPC(bool stop);
    // Function to reset the NPC
    void ResetNPC();
}