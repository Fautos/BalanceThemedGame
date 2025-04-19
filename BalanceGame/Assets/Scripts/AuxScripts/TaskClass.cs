using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

// I cant hinherit from monobehaviour because it causes problems
public class TaskClass //: MonoBehaviour
{
    private bool taskComplete = false;
    public string taskName;
    private GameObject taskText, Indicator, taskGO;
    private ITask taskScript;
    private Color IndicatorColor;

    public TaskClass(string TaskName, GameObject TaskText, GameObject Indicator, Color IndicatorColor)
    {
        // Initialize the variables
        this.taskText = TaskText;
        this.taskName = TaskName;
        this.Indicator = Indicator;
        this.IndicatorColor = IndicatorColor;

        Debug.Log("Task name: " + taskName +"\nTask tex: "+TaskText + "\nIndicator: "+Indicator+"\nIndicator color: "+IndicatorColor);

        // And the the task
        InitTask();
    }

    public void InitTask()
    {
        IndicatorScript indicatorScript= Indicator.GetComponent<IndicatorScript>();
        Vector3 target = Vector3.zero;

        // Depending on the task we enable different things
        switch (this.taskName)
        {
            // Clean the yard
            case "CleanYard":
            {
                // Take the game object
                taskGO = GameObject.Find("TaskController/CleanYardTask"); 

                // Set the target position
                target = new Vector3(-12, -10, 0);

                // Add the task to the list
                taskText.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "Clean prison yard\n";
                break;
            }
            // Clean the bathroom
            case "CleanBathroom":
            {
                // Take the game object
                taskGO = GameObject.Find("TaskController/CleanBathroomTask");
                
                // Set the target position
                target = new Vector3(3.5f, 16, 0);

                // Add the task to the list
                taskText.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "Unclog the toilet\n";
                break;
            }
            // Help in the kitchen
            case "HelpKitchen":
            {
                // Take the game object
                taskGO = GameObject.Find("TaskController/HelpKitchenTask");

                // Set the target position
                target = new Vector3(12, -15, 0);

                // Add the task to the list
                taskText.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "Assist in kitchen\n";
                break;
            }
            // Help in the workshop
            case "HelpWorkshop":
            {
                // Take the game object
                taskGO = GameObject.Find("TaskController/HelpWorkshopTask");

                // Set the target position
                target = new Vector3(-38.5f, -18, 0);

                // Add the task to the list
                taskText.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "Assist in workshop\n";
                break;
            }
            // Submit letters
            case "DeliverMail":
            {
                // Set the target position
                target = new Vector3(-25, 13, 0);

                // Add the task to the list
                taskText.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "Deliver the mail\n";
                break;
            }
        }

        // Paint the dot
        taskText.transform.Find("Image").gameObject.GetComponent<Image>().color = IndicatorColor;

        // At last we set the indicator
        Indicator.GetComponent<IndicatorScript>().ChangeColor(IndicatorColor);
        Indicator.GetComponent<IndicatorScript>().SetTarget(target);
        Indicator.GetComponent<IndicatorScript>().HideCursor(false);

        // And start the task
        taskScript = taskGO.GetComponent<ITask>();
        taskScript.StartTask();

    }

    public bool TaskComplete()
    {
        // Since we are using Interfaces we only have to look at the method
        taskComplete = taskScript.CheckCompletion();

        // If the task is complete
        if (taskComplete)
        {
            FinishTask();
        }
        
        return taskComplete;
    }

    public void FinishTask()
    {
        // First we finish the task (in case it was not finish by itself)
        taskScript.FinishTask();
        // Strike through the text
        taskText.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "<s>" + taskText.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text + "</s>" ;

        // Hide the indicator
        Indicator.GetComponent<IndicatorScript>().HideCursor(true);

    }
}

// This interface must be used by all the task game objects
public interface ITask
{
    // In all the game object we must define a method to start the task
    void StartTask();

    // A method to check if the task is over
    bool CheckCompletion();

    // And a method to destroy the task
    void FinishTask();
}

// This interface will also be used in minigames which requires aditional screens
public interface IWindow
{
    // There must be a method to launch the minigame
    void StartMiniGame();
}