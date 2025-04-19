using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task5_DeliverMailScript : MonoBehaviour, ITask
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region ITask interface functions
    public void StartTask()
    {
        // Activate the mark
        
    }

    // A method to check if the task is over
    public bool CheckCompletion()
    {
        return false;
    }

    // And a method to destroy the task
    public void FinishTask()
    {

    }

    #endregion

}
