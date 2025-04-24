using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For SceneManager.LoadScene
using UnityEngine.SceneManagement;

public class IntroScript : MonoBehaviour
{
    public List<GameObject> scenesList;
    public int sceneIndex;
    public GameObject loadingScene;

    // Start is called before the first frame update
    void Start()
    {
        // First we disable all the scenes
        foreach (GameObject scene in scenesList)
        {
            if (scene.activeInHierarchy)
            {
                scene.SetActive(false);
            }
        }

        if(loadingScene.activeInHierarchy)
        {
            loadingScene.SetActive(false);
        }

        // And show the first scene
        sceneIndex = 0;
        scenesList[sceneIndex].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            PassScene();
        }
    }

    void PassScene()
    {
        // First we disable the previous scene
        scenesList[sceneIndex].SetActive(false);
        
        // And then we check if there is a new avaliable scene
        sceneIndex ++;
        if (sceneIndex >= scenesList.Count)
        {
            // If ther is no scene avaliable we go to the start menu
            LoadScene(1);
        }
        else
        {
            // Else we show the next scene
            scenesList[sceneIndex].SetActive(true);
        }
    }

    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        loadingScene.SetActive(true);

        while (!operation.isDone)
        {
            yield return null;
        }
    }
}
