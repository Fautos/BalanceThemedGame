using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task2_CleanBathroomScript : MonoBehaviour, ITask
{
    public GameObject bathroomScreen, triggerGO;
    public List<Sprite> plugerSprites = new List<Sprite>{} ;

    public void StartTask()
    {
        ActivateTrigger();
    }

    public bool CheckCompletion()
    {
        return true;
    }

    public void FinishTask()
    {

    }

    private void ActivateTrigger()
    {

    }
}
