using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public GameObject mapCanvas;
    public GameObject backToCustomizing;

    public bool mapIsLoaded;

    // Start is called before the first frame update
    void Start()
    {
        mapCanvas = GameObject.Find("Map Canvas");
        mapCanvas.SetActive(false);

        backToCustomizing = GameObject.Find("Back To Customizing");
        backToCustomizing.SetActive(false);

        //missionManager.GetComponent<MissionManager>().enabled = false;
        //missionManager.GetComponent<MissionUIUpdate>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        mapIsLoaded = mapCanvas.activeSelf;

        if (Input.GetKeyDown(KeyCode.Return) && SceneManager.GetActiveScene().name == "Main Moving Scene")
        {
            LoadMapUI();
        }
    }
    
    public void LoadMapUI()
    {
        Debug.Log("Input detected");

        if (mapIsLoaded)
        {
            mapCanvas.SetActive(false);
            backToCustomizing.SetActive(true);
            Debug.Log("Unloaded Route Map Plan");
            mapIsLoaded = false;
        }
        else
        {
            mapCanvas.SetActive(true);
            backToCustomizing.SetActive(false);
            Debug.Log("Loaded Route Map Plan");
            mapIsLoaded = true;
        }
    }

    public void StartDelivering()
    {
        if (VehicleManager.playerVehicle != null)
        {
            SceneManager.LoadScene("Main Moving Scene");
            GameObject missionManager = GameObject.Find("MissionsManager");

            missionManager.GetComponent<MissionManager>().enabled = true;
            missionManager.GetComponent<MissionUIUpdate>().enabled = true;
            MissionUIUpdate.mapCanvas = GameObject.Find("Map Canvas");
        }


    }

    public void StartCustomizing()
    {
        GameObject missionManager = GameObject.Find("MissionsManager");
        missionManager.GetComponent<MissionManager>().enabled = false;
        missionManager.GetComponent<MissionUIUpdate>().enabled = false;

        SceneManager.LoadScene("Vehicle Customize");
        VehicleManager.playerVehicle = null;
    }
}
