using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task1_CleanYardScript : MonoBehaviour, ITask
{
    private List<Vector3> spawnPositions = new List<Vector3>{
                                                new Vector3(-4.5f, 4,0),
                                                new Vector3(-21,0.5f,0),
                                                new Vector3(-12.5f, -9,0),
                                                new Vector3(-19, -16.5f,0),
                                                new Vector3(-5.5f, -14.5f,0),
                                                new Vector3(-3.5f, -20,0),
                                                new Vector3(-21.25f, -13,0),
                                            };
    private GameObject trashPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // First we take the "trash" prefab
        trashPrefab = Resources.Load<GameObject>("Prefabs/Trash");
    }

    #region Public functions
    public void StartTask()
    {
        GenerateTrash(3);
    }

    public bool CheckCompletion()
    {
        return !IsThereAnyTrash();
    }

    public void FinishTask()
    {
        if(IsThereAnyTrash())
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
    #endregion

    #region Private functions
    // Function to generate the trash
    private void GenerateTrash(int numTrash)
    {
        // First we check if the prefab is not null
        if (trashPrefab != null)
        {
            // Then we choose N position and spawn a random trash prefab in each of them
            List<Vector3> positionChoosen = GetRandomPositions(spawnPositions, numTrash);

            for (int i=0; i<numTrash; i++)
            {
                Instantiate(trashPrefab, positionChoosen[i], Quaternion.identity, transform);
            }
        }
        else{
            Debug.LogError("No 'Trash prefab' found.");
        }
    }

    private List<Vector3> GetRandomPositions(List<Vector3> positions, int count)
    {
        List<Vector3> chosenPositions = new List<Vector3>();
        List<int> indexes = new List<int>();

        // Make sure we dont try to chose more elements than the list size
        count = Mathf.Min(count, positions.Count);

        // Loop to chose "count" positions
        while(chosenPositions.Count < count)
        {
            // First we chose a random index
            int randomIndex = Random.Range(0, positions.Count);

            // If it's the first time we chose that index we add it to the list
            if(!indexes.Contains(randomIndex))
            {
                chosenPositions.Add(positions[randomIndex]);
                indexes.Add(randomIndex);
            }
        }

        return chosenPositions;
    }

    // Function to check if there is any trash yet in the yard
    private bool IsThereAnyTrash()
    {
        if(transform.childCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
}
