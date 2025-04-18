using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using TMPro;

public class MiniTaskController : MonoBehaviour
{
    public int completedTask;
    private int index = 0;
    [SerializeField] List<string> tasks = new List<string>{"CleanYard", 
                                        "CleanBathroom",
                                        "DeliverMail",
                                        "HelpKitchen",
                                        "HelpWorkshop"};
    [SerializeField] List<Color> taskColorsList = new List<Color>{Color.blue,
                                                            Color.green,
                                                            Color.yellow};
    [SerializeField] List<TaskClass> taskClasses = new List<TaskClass>();
    [SerializeField] List<GameObject> indicatorsList = new List<GameObject>();
    [SerializeField] List<GameObject> taskTexList = new List<GameObject>();    

    // Function to generate the tasks
    public void GenerateTasks(int numberOfTasks)
    {
        // Reset variables
        completedTask = 0;
        taskClasses.Clear();
        index = 0;

        // Choose "numberOfTasks" task from the list
        //List<string> taskChoosen = tasks.OrderBy(x => Random.Range(0f, 1f)).Take(numberOfTasks).ToList();
        List<string> taskChoosen = new List<string>{"HelpKitchen"};


        foreach (string task in taskChoosen)
        {
            taskClasses.Add(new TaskClass(TaskName: task, TaskText: taskTexList[index], Indicator: indicatorsList[index], IndicatorColor: taskColorsList[index]));
            index ++;
        }
        
        index = 0;      

    }

    // Function to check if the task are completed
    public void CheckTaskCompleted()
    {
        // We go through all the tasks and check if they are finished
        if (taskClasses.Count != 0)
        {
            // We run through the loop in reverse to avoid problems with the index in case we remove something
            for(int i = taskClasses.Count - 1; i >= 0; i--)
            {
                // If any task is complete it will be finished and removed from the list
                if(taskClasses[i].TaskComplete())
                {
                    taskClasses.RemoveAt(i);
                    completedTask++;
                }
            }
        }    
    }

    // Function to finish all the remaining tasks
    public void FinishDayTasks()
    {
        if (taskClasses.Count != 0)
        {
            // We run through the loop in reverse to avoid problems with the index in case we remove something
            for(int i = taskClasses.Count - 1; i >= 0; i--)
            {
                taskClasses[i].FinishTask();
                taskClasses.RemoveAt(i);
            }
        }
    }
}
