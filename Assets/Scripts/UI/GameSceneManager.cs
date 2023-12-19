using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public Canvas mapCanvas;

    public bool mapIsLoaded = false;

    // Start is called before the first frame update
    void Start()
    {
        mapCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        mapCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Input detected");

            if (mapIsLoaded)
            {
                mapCanvas.enabled = false;
                Debug.Log("Unloaded Route Map Plan");
                mapIsLoaded = false;
            }
            else
            {
                mapCanvas.enabled = true;
                Debug.Log("Loaded Route Map Plan");
                mapIsLoaded = true;
            }
        }
    }
    
    public void StartDelivering()
    {
        SceneManager.LoadScene("Main Moving Scene");
        mapCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }
}
