using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public bool mapIsLoaded = false;

    // Start is called before the first frame update
    void Start()
    {
        //UnityEngine.SceneManagement.SceneManager.LoadScene("Main Moving Scene");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Input detected");

            if (mapIsLoaded)
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Route Map Plan");
                Debug.Log("Unloaded Route Map Plan");
                mapIsLoaded = false;
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Route Map Plan", UnityEngine.SceneManagement.LoadSceneMode.Additive);
                Debug.Log("Loaded Route Map Plan");
                mapIsLoaded = true;
            }
        }
    }
}
