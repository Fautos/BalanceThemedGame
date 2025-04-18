using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Task2_CleanBathroomScript : WindowTaskClass
{
    [SerializeField] List<Sprite> plugerSprites = new List<Sprite>{};
    [SerializeField] List<Sprite> spaceBarSprites = new List<Sprite>{};

    // Minigame variables
    [SerializeField] bool miniGameCompleted, spaceBarAnimation;
    [SerializeField] int maxScore, gameStage, plugerSpriteIndex, _score;
    [SerializeField] int Score{get{return _score;} 
                                set{
                                    if(value >= 0)
                                    _score = value;
                                    else
                                    _score = 0;
                                }}
    [SerializeField] float reduceCountCd;
    private Coroutine reduceCountCoroutine;

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
                FinishTimers();

                // We place the pluger
                MiniGameScreen.transform.Find("Plunger").GetComponent<Image>().sprite = plugerSprites[plugerSpriteIndex];

                // And launch the timer corrutine
                if (reduceCountCoroutine == null)
                {
                    reduceCountCoroutine = StartCoroutine(ReduceCountCoroutine());
                }

                gameStage ++;
                break;
            }
            case 1:
            {
                // In this minigame the player has to press the key "space" N times as fast as he can
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Score++;
                    SwitchSprites();
                }

                // Then we update the slider
                SliderUpdate();

                // Once they reach the goal, the game is completed 
                if (Score >= maxScore)
                {
                    gameStage = 0;
                    miniGameCompleted = true;
                    StopCoroutine(reduceCountCoroutine);
                    reduceCountCoroutine = null;
                }
                
                break;
            }
        }

        return miniGameCompleted;
    }

    public override void ResetMinigameVariables()
    {
        Score = 0;
        reduceCountCd = 2f;
        maxScore = 25;
        plugerSpriteIndex = 0;
        gameStage = 0;
        miniGameCompleted = false;
        spaceBarAnimation = true;
    }

    public override void FinishTimers()
    {
        if (reduceCountCoroutine != null)
        {
            StopCoroutine(reduceCountCoroutine);
            reduceCountCoroutine = null;
        }
    }
    #endregion

    // Timer coroutine
    IEnumerator ReduceCountCoroutine()
    {
        while (!miniGameCompleted)
        {
            yield return new WaitForSeconds(reduceCountCd);
            if (miniGameCompleted)
            {
                break;
            }

            Score -= 5;
        }
    }

    // Function to switch between pluger sprites
    private void SwitchSprites()
    {
        if (plugerSpriteIndex == 0)
        {
            plugerSpriteIndex = 1;
        }else if (plugerSpriteIndex == 1)
        {
            plugerSpriteIndex = 2;
        }else if (plugerSpriteIndex == 2)
        {
            plugerSpriteIndex = 1;
        }

        MiniGameScreen.transform.Find("Plunger").GetComponent<Image>().sprite = plugerSprites[plugerSpriteIndex];

        // To animate the spacebar
        if (spaceBarAnimation)
        {
            spaceBarAnimation = false;
            MiniGameScreen.transform.Find("Toilet/Spacebar").GetComponent<Image>().sprite = spaceBarSprites[1];
        }
        else{
            spaceBarAnimation = true;
            MiniGameScreen.transform.Find("Toilet/Spacebar").GetComponent<Image>().sprite = spaceBarSprites[0];
        }
        
    }

    // Function to update the slider
    private void SliderUpdate()
    {
        float percentage = (float)Score/maxScore;

        MiniGameScreen.transform.Find("Toilet/Scrollbar").GetComponent<Scrollbar>().value = percentage;
    }

}