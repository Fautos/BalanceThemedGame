using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using TMPro;

public class MiniTaskController : MonoBehaviour
{
    public int pendingTask, completedTask;
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
    [SerializeField] string tasklist;

    // Start is called before the first frame update
    public void GenerateTasks(int numberOfTasks)
    {
        // Reset variables
        pendingTask = numberOfTasks;
        completedTask = 0;
        tasklist = "";
        taskClasses.Clear();
        index = 0;

        // Choose "numberOfTasks" task from the list
        List<string> taskChoosen = tasks.OrderBy(x => Random.Range(0f, 1f)).Take(numberOfTasks).ToList();

        foreach (string task in taskChoosen)
        {
            taskClasses.Add(new TaskClass(TaskName: task, TaskText: taskTexList[index], Indicator: indicatorsList[index], IndicatorColor: taskColorsList[index]));
            index ++;

            // MiniGameSelection(task);
        }
        
        index = 0;

        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
