using UnityEngine;
using TMPro;

[System.Serializable]
public class GameOverScreenManager 
{

    private GameObject GameOverScreen;
    private TMP_Text resultText;
    
    // Class initialization
    public GameOverScreenManager(GameObject GameOverScreen)
    {
        // Get the general GameObject and disable it
        this.GameOverScreen = GameOverScreen;

        if(GameOverScreen.activeInHierarchy)
        {
            GameOverScreen.SetActive(false);
        }
        
        // Get the texts spaces
        Transform TaskTextTF = GameOverScreen.transform.Find("ResultText");
        if (TaskTextTF != null)
        {
            resultText = TaskTextTF.gameObject.GetComponent<TextMeshProUGUI>();
        }

        // Lastly we disable the screen so it starts off
        if (GameOverScreen.activeInHierarchy)
        {
            GameOverScreen.SetActive(false);
        }
    }

    // This function activates the screen
    public void ActivateScreen(int playerHp, float playerReputation)
    {
        // First we display the screen
        if (!GameOverScreen.activeInHierarchy)
        {
            GameOverScreen.SetActive(true);
        }

        resultText.text = "";
        
        // Then we set the texts
        if (playerHp <= 0)
        {
            // If player died
            if (playerReputation <= 0)
            {
                resultText.text = "You did everything right... and still ended up dead.\nIrony runs deep in these walls.\nTo survive here, you’ll have to go beyond the bounds of morality.";
            }
            else
            {
                resultText.text = "You tried to rule the prison...\nBut ended up just another victim.\nIn here, trouble only leads to more trouble.";
            }
        }
        else
        {
            // If the player stay alive till the 5 day
            if(playerReputation <= -5)
            {
                resultText.text = "You've imposed your rule.\nYou’ll stay behind bars forever...\nBut at least you're the king of the inmates.";
            }
            else if(playerReputation> -5 && playerReputation < 5)
            {
                resultText.text ="You weren't deemed worthy of freedom.\nYour days will pass behind bars, in reflection.\nMaybe one day, you’ll understand the balance behind bars.";
            }
            else if(playerReputation>= 5)
            {
                resultText.text ="Congratulations.\nYou balanced your reputation, earned respect, and kept your life.\nYou've proven you're ready to be free.";
            }
        }
    }

    public void DisableScreen()
    {
        // Then we disable the screen
        if(GameOverScreen.activeInHierarchy)
        {
            GameOverScreen.SetActive(false);
        }
    }

}