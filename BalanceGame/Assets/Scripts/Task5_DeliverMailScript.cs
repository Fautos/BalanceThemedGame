using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task5_DeliverMailScript : MonoBehaviour, ITask, IWindow
{
    private List<Vector3> spawnPositions = new List<Vector3>{
                                                new Vector3(12.5f, 16,0),
                                                new Vector3(1,-17,0),
                                                new Vector3(-21, -20,0),
                                            };
    [SerializeField] protected GameObject triggerGO, receiver;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        // First we get the trigger's gameobject
        triggerGO = transform.GetChild(0).gameObject;

        if (triggerGO != null)
        {
            if (triggerGO.activeInHierarchy)
            {
                triggerGO.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("No trigger found for this task.");
        }

        // The receiver
        receiver = transform.GetChild(1).gameObject;

        if (receiver != null)
        {
            if (receiver.activeInHierarchy)
            {
                receiver.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("Receiver not found.");
        }

        // And the player
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region ITask interface functions
    public void StartTask()
    {
        // To start the task we have to enable the trigger
        triggerGO.SetActive(true);        
    }

    // A method to check if the task is over
    public bool CheckCompletion()
    {
        // If the trigger or the receiver are active it means that the game is not completed
        return !(triggerGO.activeInHierarchy || receiver.activeInHierarchy);
    }

    // And a method to destroy the task
    public void FinishTask()
    {
        // If the exclamation mark is active we have to disable it
        if (triggerGO.activeInHierarchy)
        {
            triggerGO.SetActive(false);
        }

        // The same goes to the receiver
        if(receiver.activeInHierarchy)
        {
            receiver.SetActive(false);
        }
    }

    #endregion

    #region IWindows function
    public void StartMiniGame()
    {
        // First we "give" the mail to the player
        playerController.ReceiveMail(true);
        
        // Disable the indicator game object
        triggerGO.SetActive(false);

        // And spawn the receiver in a random location
        receiver.transform.position = spawnPositions[Random.Range(0, spawnPositions.Count)];
        if(!receiver.activeInHierarchy)
        {
            receiver.SetActive(true);
        }
    }

    public void RestartMiniGame()
    {
        // The indicator will be turned on again
        triggerGO.SetActive(true);

        // And the receiver must de disable
        if(receiver.activeInHierarchy)
        {
            receiver.SetActive(false);
        }
    }

    #endregion

}
