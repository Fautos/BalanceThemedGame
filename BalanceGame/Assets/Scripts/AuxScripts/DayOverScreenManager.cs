using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DayOverScreenManager //: MonoBehaviour
{
    private GameObject DayOverScreen, PositiveResult, NegativeResult;
    private TMP_Text TaskText, GoodActionsText, BadActionsText;
    
    // Class initialization
    public DayOverScreenManager(GameObject DayOverScreen)
    {
        // Get the general GameObject and disable it
        this.DayOverScreen = DayOverScreen;

        if(DayOverScreen.activeInHierarchy)
        {
            DayOverScreen.SetActive(false);
        }

        // Get the positive/negative gameobjects
        Transform PositiveResultTF = DayOverScreen.transform.Find("TextSpace/ResultSpace/Positive");
        if (PositiveResultTF != null)
        {
            PositiveResult = PositiveResultTF.gameObject;
            //Debug.Log("Positive encontrado");
        }

        Transform NegativeResultTF = DayOverScreen.transform.Find("TextSpace/ResultSpace/Negative");
        if (NegativeResultTF != null)
        {
            NegativeResult = NegativeResultTF.gameObject;
            //Debug.Log("Negative encontrado");
        }
        
        // Get the texts spaces
        Transform TaskTextTF = DayOverScreen.transform.Find("TextSpace/DataSpace/TaskCompleteText");
        if (TaskTextTF != null)
        {
            TaskText = TaskTextTF.gameObject.GetComponent<TextMeshProUGUI>();
            //Debug.Log("Task text encontrado");
        }

        Transform GATextTF = DayOverScreen.transform.Find("TextSpace/DataSpace/GoodActionsText");
        if (GATextTF != null)
        {
            GoodActionsText = GATextTF.gameObject.GetComponent<TextMeshProUGUI>();
            //Debug.Log("Good actions text encontrado");
        }

        Transform BATextTF = DayOverScreen.transform.Find("TextSpace/DataSpace/BadActionsText");
        if (BATextTF != null)
        {
            BadActionsText = BATextTF.gameObject.GetComponent<TextMeshProUGUI>();
            //Debug.Log("Bad actions text encontrado");
        }

        // Lastly we disable the screen so it starts off
        if (DayOverScreen.activeInHierarchy)
        {
            DayOverScreen.SetActive(false);
        }
    }

    // This function activates the screen
    public void ActivateScreen(int TaskCompleted, int TaskPerDay, int GoodActions, int BadActions)
    {
        // First we display the screen
        if (!DayOverScreen.activeInHierarchy)
        {
            DayOverScreen.SetActive(true);
        }

        // Then we set the texts
        TaskText.text = "Tasks completed ........ " + TaskCompleted + "/" + TaskPerDay;
        GoodActionsText.text = "Good actions ............. " + GoodActions;
        BadActionsText.text = "Bad actions .............. " + BadActions;

        // Then we display either the good or bad emoji depending on the results
        if(PositiveResult.activeInHierarchy)
        {
            PositiveResult.SetActive(false);
        }
        if(NegativeResult.activeInHierarchy)
        {
            NegativeResult.SetActive(false);
        }

        if (BadActions == 0 && TaskCompleted >= TaskPerDay-1)
        {
            PositiveResult.SetActive(true);
        }
        else
        {
            NegativeResult.SetActive(true);
        }
    }

    public void DisableScreen()
    {
        // First we disable the emojis
        if(PositiveResult.activeInHierarchy)
        {
            PositiveResult.SetActive(false);
        }
        if(NegativeResult.activeInHierarchy)
        {
            NegativeResult.SetActive(false);
        }

        // Then we disable the screen
        if(DayOverScreen.activeInHierarchy)
        {
            DayOverScreen.SetActive(false);
        }
    }
}
