using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generic class for all the minigames that use aditional windows
public abstract class WindowTaskClass : MonoBehaviour, ITask, IWindow
{
    [SerializeField] bool isActive, gameCompleted, scapeBool;
    [SerializeField] protected GameObject triggerGO, MiniGameScreen;
    private PlayerController playerController;

    public void Start()
    {
        // First we get the trigger's gameobject
        triggerGO = transform.GetChild(0).gameObject;

        if (triggerGO != null)
        {
            if (triggerGO.activeInHierarchy)
            {
                triggerGO.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("No trigger found for this task.");
        }

        // And the player
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        // And initialize some variables
        isActive = false;
        gameCompleted = false;
        scapeBool = false;
    }

    public void Update()
    {
        // If the game is active you can play unless you beat it or you scape
        if (isActive && !scapeBool)
        {
            gameCompleted = PlayMinigame();
        }
        else if (isActive && scapeBool)
        {
            ExitMiniGame();
        }
        else if (gameCompleted)
        {
            ExitMiniGame();
            FinishTask();
        }

        // If you complete the game or press scape you exti the game
        if(Input.GetKey(KeyCode.Escape) || gameCompleted)
        {
            scapeBool = true;
        }
    }

    // Functions for the ITask interface
    #region ITask functions
    public void StartTask()
    {
        // To start the task we have to enable the trigger
        triggerGO.SetActive(true);

        // And initialize some variables
        isActive = false;
        gameCompleted = false;
        
        scapeBool = false;
    }

    public bool CheckCompletion()
    {
        // If the trigger is active it means that the game is not completed
        return !triggerGO.activeInHierarchy;
    }

    public void FinishTask()
    {
        // To finish the task we only have to disable the trigger
        if(triggerGO.activeInHierarchy)
        {
            triggerGO.SetActive(false);
        }

        // And finish the minigame if the window were open
        if (MiniGameScreen.activeInHierarchy)
        {
            ExitMiniGame();
        }
    }
    #endregion

    // Functions for the Wtask interface
    #region WTask functions
    public void StartMiniGame()
    {
        // First we hide the player and set the variable "isActive" to true
        playerController.HidePlayer(true);
        isActive = true;

        // Reset the minigame variables
        ResetMinigameVariables();
        FinishTimers();

        // And activate the minigame window
        if (!MiniGameScreen.activeInHierarchy)
        {
            MiniGameScreen.SetActive(true);
        }
    }

    public void ExitMiniGame()
    {
        // The player is again playable
        playerController.HidePlayer(false);
        isActive = false;
        scapeBool = false;

        // The corroutines are finished (in case they exist)
        FinishTimers();

        // And the minigame window is hidden
        if (MiniGameScreen.activeInHierarchy)
        {
            MiniGameScreen.SetActive(false);
        }
    }
    #endregion

    public abstract bool PlayMinigame();
    public abstract void ResetMinigameVariables();
    public abstract void FinishTimers();

}
