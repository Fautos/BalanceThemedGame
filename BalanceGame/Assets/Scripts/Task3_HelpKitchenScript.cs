using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class Task3_HelpKitchenScript : WindowTaskClass
{
    [SerializeField] List<GameObject> foodLayers = new List<GameObject>{};
    [SerializeField] List<GameObject> buttonsGO = new List<GameObject>{};
    [SerializeField] List<int> ingredientsList = new List<int>();

    // Minigame variables
    [SerializeField] bool miniGameCompleted;
    [SerializeField] int gameStage, maxScore, _score, numIngredients, keysPressed;
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

                gameStage ++;
                break;
            }
            case 1:
            {
                // And disable all the food layers and buttons
                CleanPizza();

                // Then we choose a random recipe
                numIngredients = Random.Range(1, 4);
                ingredientsList = SelectRandomIngredients(numIngredients);

                // And show the player the buttons they need to click 
                foreach (int ingredient in ingredientsList)
                {
                    buttonsGO[ingredient].SetActive(true);
                }
                UpdatePizzaCount();

                // With this variable we will count how many keys the players has pressed
                keysPressed = 0;
                
                gameStage ++;

                break;
            }
            case 2:
            {
                // We wait till the player finish their recipe
                if (keysPressed >= numIngredients)
                {
                    Score++;
                    UpdatePizzaCount();

                    // When they finish the pizza we check how many pizzas are left
                    if (Score >= maxScore)
                    {
                        gameStage = 3;
                        miniGameCompleted = true;
                    }
                    else
                    {
                        gameStage = 1;
                    }
                }

                // If the player press C and the cheese was an ingredient
                if (Input.GetKeyDown(KeyCode.C) && ingredientsList.Contains(0))
                {
                    // We show the cheese layer
                    foodLayers[0].SetActive(true);
                    // Add the trigger
                    keysPressed ++;
                    // And remove the button
                    buttonsGO[0].SetActive(false);
                    ingredientsList.RemoveAll(item => item == 0);  
                }
                if (Input.GetKeyDown(KeyCode.T) && ingredientsList.Contains(1))
                {
                    // We show the cheese layer
                    foodLayers[1].SetActive(true);
                    // Add the trigger
                    keysPressed ++;
                    // And remove the button
                    buttonsGO[1].SetActive(false);
                    ingredientsList.RemoveAll(item => item == 1);  
                }
                if (Input.GetKeyDown(KeyCode.P) && ingredientsList.Contains(2))
                {
                    // We show the cheese layer
                    foodLayers[2].SetActive(true);
                    // Add the trigger
                    keysPressed ++;
                    // And remove the button
                    buttonsGO[2].SetActive(false);
                    ingredientsList.RemoveAll(item => item == 2);  
                }

                break;
            }
            case 3:
            {
                Debug.Log("Kitchen minigame should be finished");
                break;
            }
        }

        return miniGameCompleted;
    }

    public override void ResetMinigameVariables()
    {
        Score = 0;
        maxScore = 3;
        gameStage = 0;
        numIngredients = 0;
        keysPressed=0;
        miniGameCompleted = false;
    }

    public override void FinishTimers()
    {
        // There are no timer in this minigame
    }
    #endregion

    // Function to disable all the buttons and food layers
    public void CleanPizza()
    {
        // For the buttons
        foreach (GameObject button in buttonsGO)
        {
            if(button.activeInHierarchy)
            {
                button.SetActive(false);
            }
        }

        // And the food layers
        foreach (GameObject foodLayer in foodLayers)
        {
            if(foodLayer.activeInHierarchy)
            {
                foodLayer.SetActive(false);
            }
        }
    }

    // Select random ingredients
    List<int> SelectRandomIngredients(int n)
    {
        // Crear una lista con los números posibles
        List<int> possibleNumbers = new List<int> { 0, 1, 2 };

        // Crear una lista para los números seleccionados
        List<int> selectedNumbers = new List<int>();

        // Mezclamos la lista para obtener una selección aleatoria
        System.Random rng = new System.Random();
        possibleNumbers.Sort((a, b) => rng.Next(-1, 2));  // Shuffle list

        // Seleccionamos los primeros N números después de mezclar
        for (int i = 0; i < n; i++)
        {
            selectedNumbers.Add(possibleNumbers[i]);
        }

        return selectedNumbers;
    }

    // Function to update the counter
    private void UpdatePizzaCount()
    {
        MiniGameScreen.transform.Find("Kitchen/PizzasLeft/Text").GetComponent<TMP_Text>().text = Score + "/" + maxScore;
    }
}
