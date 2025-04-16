using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class TaskClass //: MonoBehaviour
{
    private GameObject taskText, Indicator;
    private string taskName;
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
                // Three piles of trash will appear in the yard and you have to pick them
                
                // Set the target position
                target = new Vector3(-12, -10, 0);

                // Add the task to the list
                taskText.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "Clean prison yard\n";
                break;
            }
            // Clean the bathroom
            case "CleanBathroom":
            {
                // Ventana emergente en la que tienes que clickar en 3 motas de suciedad. Se activa desde el baño.
                
                // Set the target position
                target = new Vector3(12, 16, 0);

                // Add the task to the list
                taskText.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "Tidy the bathroom\n";
                break;
            }
            // Help in the kitchen
            case "HelpKitchen":
            {
                // Set the target position
                target = new Vector3(4, -18, 0);

                // Add the task to the list
                taskText.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "Assist in kitchen\n";
                break;
            }
            // Help in the workshop
            case "HelpWorkshop":
            {
                // Set the target position
                target = new Vector3(-28, -18, 0);

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

        // And paint the dot
        taskText.transform.Find("Image").gameObject.GetComponent<Image>().color = IndicatorColor;

        // At last we set the indicator
        Indicator.GetComponent<IndicatorScript>().ChangeColor(IndicatorColor);
        Indicator.GetComponent<IndicatorScript>().SetTarget(target);
        Indicator.GetComponent<IndicatorScript>().HideCursor(false);

    }

    public void FinishTask()
    {
        // Strike through the text
        taskText.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "<s>" + taskText.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text + "</s>" ;

        // Hide the indicator
        Indicator.GetComponent<IndicatorScript>().HideCursor(true);

    }
}
