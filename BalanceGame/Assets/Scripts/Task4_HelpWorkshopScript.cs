using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class Task4_HelpWorkshopScript : WindowTaskClass
{
    [SerializeField] List<GameObject> NeedleLayers = new List<GameObject>{};
    [SerializeField] List<GameObject> buttonsGO = new List<GameObject>{};

    // Minigame variables
    [SerializeField] bool miniGameCompleted;
    [SerializeField] int gameStage, maxScore, _score, needleIndex;
    [SerializeField] int Score{get{return _score;} 
                                set{
                                    if(value >= 0)
                                    _score = value;
                                    else
                                    _score = 0;
                                }}

    // Mandatory functions for the WindowTaskClass
    #region Window Task Class functions
    public override bool PlayMinigame()
    {
        switch(gameStage)
        {
            case 0:
            {
                // First we set the variables
                ResetMinigameVariables();
                UpdateCount();

                gameStage ++;
                break;
            }
            case 1:
            {
                // Then we active a needle
                SwitchNeedle();
                gameStage ++;

                break;
            }
            case 2:
            {
                // The game will end when the player has done 5 cycles
                if (Score >= maxScore)
                {
                    gameStage = 3;
                    miniGameCompleted = true;
                }

                // When the player hit the right button we switch the needle
                if (Input.GetKeyDown(KeyCode.D) && needleIndex == 0)
                {
                    SwitchNeedle();  
                }
                if (Input.GetKeyDown(KeyCode.A) && needleIndex == 1)
                {
                    Score ++;
                    SwitchNeedle();
                }

                break;
            }
            case 3:
            {
                Debug.Log("Workshop minigame should be finished");
                break;
            }
        }

        return miniGameCompleted;
    }

    public override void ResetMinigameVariables()
    {
        Score = 0;
        maxScore = 5;
        gameStage = 0;
        needleIndex = 1;
        miniGameCompleted = false;
    }

    public override void FinishTimers()
    {
        // There are no timer in this minigame
    }
    #endregion

    // Function to switch between needles
    public void SwitchNeedle()
    {
        // First disable all the layers
        foreach (GameObject needle in NeedleLayers)
        {
            if(needle.activeInHierarchy)
            {
                needle.SetActive(false);
            }
        }
        foreach (GameObject button in buttonsGO)
        {
            if(button.activeInHierarchy)
            {
                button.SetActive(false);
            }
        }

        // Switch between indexes
        if (needleIndex == 0)
        {
            needleIndex = 1;
        }
        else
        {
            needleIndex = 0;
        }

        // Active needle and button layer
        NeedleLayers[needleIndex].SetActive(true);
        buttonsGO[needleIndex].SetActive(true);

        //And update the counter
        UpdateCount();

    }

    // Function to update the counter
    private void UpdateCount()
    {
        MiniGameScreen.transform.Find("Workshop/WorkLeft/Text").GetComponent<TMP_Text>().text = Score + "/" + maxScore;
    }
}
