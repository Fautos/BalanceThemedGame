using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using TMPro;

public class MiniTaskController : MonoBehaviour
{
    public int pendingTask, completedTask;
    [SerializeField] List<string> tasks = new List<string>{"CleanYard", 
                                        "CleanBathroom",
                                        "DeliverMail",
                                        "HelpKitchen",
                                        "HelpWorkshop"};

    [SerializeField] string tasklist;
    [SerializeField] TMP_Text ToDoList;

    // Start is called before the first frame update
    public void GenerateTasks(int numberOfTasks)
    {
        // Reset variables
        pendingTask = numberOfTasks;
        completedTask = 0;
        tasklist = "";

        // Choose "numberOfTasks" task from the list
        List<string> taskChoosen = tasks.OrderBy(x => Random.Range(0f, 1f)).Take(numberOfTasks).ToList();

        foreach (string task in taskChoosen)
        {
            MiniGameSelection(task);
        }

        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Minigames selection
    private void MiniGameSelection(string minigame)
    {

        switch (minigame)
        {
            // Clean the yard
            case "CleanYard":
            {
                // Three piles of trash will appear in the yard and you have to pick them
                tasklist += "Clean yard\n";
                break;
            }
            // Clean the bathroom
            case "CleanBathroom":
            {
                // Ventana emergente en la que tienes que clickar en 3 motas de suciedad. Se activa desde el baño.
                tasklist += "Clean bathroom\n";
                break;
            }
            // Help in the kitchen
            case "HelpKitchen":
            {
                tasklist += "Help in kitchen\n";
                break;
            }
            // Help in the workshop
            case "HelpWorkshop":
            {
                tasklist += "Help in workshop\n";
                break;
            }
            // Submit letters
            case "DeliverMail":
            {
                tasklist += "Deliver mail\n";
                break;
            }
        }

        ToDoList.text = tasklist;

    }
}
