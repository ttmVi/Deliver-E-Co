using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public GameObject mapCanvas;
    public GameObject backToCustomizing;

    public TextMeshProUGUI loseText;

    public bool mapIsLoaded = false;

    // Start is called before the first frame update
    void Start()
    {
        mapCanvas = GameObject.Find("Map Canvas");
        mapCanvas.SetActive(false);

        backToCustomizing = GameObject.Find("Back To Customizing");
        backToCustomizing.SetActive(false);

        if (SceneManager.GetActiveScene().name == "Main Moving Scene")
        {
            GameObject[] Obstacles = GameObject.FindGameObjectsWithTag("Sidewalk").Concat(GameObject.FindGameObjectsWithTag("Road Barriers")).ToArray();
            Debug.Log($"Found {Obstacles.Length} obstacles.");
            foreach (var obstacle in Obstacles)
            {
                obstacle.AddComponent<BoxCollider>();
                //obstacle.GetComponent<BoxCollider>().isTrigger = true;
                obstacle.AddComponent<PlayerColliding>();
            }

            GameObject[] GasStations = GameObject.FindGameObjectsWithTag("TramXang");
            Debug.Log($"Found {GasStations.Length} gas stations.");
            foreach (var gasStation in GasStations)
            {
                gasStation.AddComponent<GasStation>();
            }
        }
        //missionManager.GetComponent<MissionManager>().enabled = false;
        //missionManager.GetComponent<MissionUIUpdate>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //mapIsLoaded = mapCanvas.activeInHierarchy;

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

    public static void StartDelivering()
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

    public static void StartCustomizing()
    {
        if (MissionManager.missionManager.successfulMissionCount >= MissionManager.missionManager.requiredSuccessfulMissions)
        {
            GameObject missionManager = GameObject.Find("MissionsManager");
            missionManager.GetComponent<MissionManager>().enabled = false;
            missionManager.GetComponent<MissionUIUpdate>().enabled = false;

            SceneManager.LoadScene("Vehicle Customize");
            VehicleManager.playerVehicle = null;
        }
    }

    public static IEnumerator LoseLevel(string loseReason)
    {
        TextMeshProUGUI loseText = GameObject.Find("Losing").GetComponent<TextMeshProUGUI>();
        loseText.text = $"You lost because {loseReason}.";
        Debug.Log($"You lost because {loseReason}.");
        yield return new WaitForSeconds(3f);

        loseText.text = "";
        loseText = null;
        MoneyManager.money -= 100;
        SceneManager.LoadScene("Vehicle Customize");
        VehicleManager.playerVehicle = null;
    }
}
