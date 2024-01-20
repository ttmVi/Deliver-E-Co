using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager gameSceneManager;

    public GameObject mapCanvas;
    public GameObject backToCustomizing;

    private static GameObject losingCanvas;
    private static GameObject winningCanvas;
    private GameObject pausingCanvas;

    public TextMeshProUGUI loseText;

    public bool mapIsLoaded = false;
    public static bool isPausing = false;

    // Start is called before the first frame update
    void Start()
    {
        mapCanvas = GameObject.Find("Map Canvas");
        mapCanvas.SetActive(false);

        backToCustomizing = GameObject.Find("Back To Customizing");
        backToCustomizing.SetActive(false);

        if (SceneManager.GetActiveScene().name == "Main Moving Scene")
        {
            losingCanvas = GameObject.Find("Losing Canvas");
            losingCanvas.SetActive(false);
            winningCanvas = GameObject.Find("Winning Canvas");
            winningCanvas.SetActive(false);
            pausingCanvas = GameObject.Find("Pausing Canvas");
            pausingCanvas.SetActive(false);

            GameObject[] Obstacles = GameObject.FindGameObjectsWithTag("Sidewalk").Concat(GameObject.FindGameObjectsWithTag("Road Barriers")).ToArray();
            foreach (var obstacle in Obstacles)
            {
                obstacle.AddComponent<BoxCollider>();
                //obstacle.GetComponent<BoxCollider>().isTrigger = true;
                obstacle.AddComponent<PlayerColliding>();
            }

            GameObject[] GasStations = GameObject.FindGameObjectsWithTag("TramXang");
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

        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name == "Main Moving Scene")
        {
            if (!isPausing)
            {
                PauseLevel();
            }
            else
            {
                ResumeLevel();
            }
        }
    }
    
    public void LoadMapUI()
    {
        if (mapIsLoaded)
        {
            mapCanvas.SetActive(false);
            backToCustomizing.SetActive(true);
            AudioManager.audioManager.PlayCloseMapSound();
            mapIsLoaded = false;
        }
        else
        {
            mapCanvas.SetActive(true);
            backToCustomizing.SetActive(false);
            AudioManager.audioManager.PlayOpenMapSound();
            mapIsLoaded = true;
        }
    }

    public static void StartDelivering()
    {
        if (VehicleManager.playerVehicle != null)
        {
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            foreach (var audioSource in audioSources)
            {
                audioSource.enabled = true;
            }

            SceneManager.LoadScene("Main Moving Scene");
        }
    }

    public static void StartCustomizing()
    {
        //if (MissionManager.missionManager.successfulMissionCount >= MissionManager.missionManager.requiredSuccessfulMissions)
        //{
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in audioSources)
        {
            audioSource.enabled = true;
        }

        SceneManager.LoadScene("Vehicle Customize");
            VehicleManager.playerVehicle = null;
        //}
    }

    public static IEnumerator LoseLevel(string loseReason)
    {
        isPausing = true;
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in audioSources)
        {
            audioSource.enabled = false;
        }

        yield return new WaitForSeconds(1f);

        AudioManager.audioManager.PlayLosingSound();
        while (!losingCanvas.activeSelf)
        {
            GameObject.Find("ResourcesManager").GetComponent<AQICalculation>().EndDayCheck();

            MoneyManager.money -= 100;
            break;
        }
        losingCanvas.SetActive(true);


        TextMeshProUGUI completedMissionsCount = GameObject.Find("CompletedMission Count").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI moneyLost = GameObject.Find("Money Lost").GetComponent<TextMeshProUGUI>();

        completedMissionsCount.text = $"{MissionManager.missionManager.successfulMissionCount}/{MissionManager.missionManager.requiredSuccessfulMissions}";
        moneyLost.text = "-100";
        isPausing = false;

        //TextMeshProUGUI loseText = GameObject.Find("Losing").GetComponent<TextMeshProUGUI>();
        //loseText.text = $"You lost because {loseReason}.";
        //Debug.Log($"You lost because {loseReason}.");
        //yield return new WaitForSeconds(3f);

        //loseText.text = "";
        //loseText = null;
        //SceneManager.LoadScene("Vehicle Customize");
        VehicleManager.playerVehicle = null;
    }

    public static IEnumerator WinLevel()
    {
        isPausing = true;
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in audioSources)
        {
            audioSource.enabled = false;
        }

        AudioManager.audioManager.PlayWinningSound();
        while (!winningCanvas.activeSelf)
        {
            GameObject.Find("ResourcesManager").GetComponent<AQICalculation>().EndDayCheck();
            break;
        }

        winningCanvas.SetActive(true);
        TextMeshProUGUI completedMissionsCount = GameObject.Find("CompletedMission Count").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI moneyEarned = GameObject.Find("Money Earned").GetComponent<TextMeshProUGUI>();

        completedMissionsCount.text = $"{MissionManager.missionManager.successfulMissionCount}/{MissionManager.missionManager.requiredSuccessfulMissions}";
        moneyEarned.text = $"{MoneyManager.money - GameObject.Find("ResourcesManager").GetComponent<MoneyManager>().startingMoney}";

        VehicleManager.playerVehicle = null;

        Debug.Log("Winning Canvas Activated");
        isPausing = false;

        yield return null;
    }

    public void PauseLevel()
    {
        isPausing = true;
        pausingCanvas.SetActive(true);
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in audioSources)
        {
            audioSource.enabled = false;
        }
    }

    public void ResumeLevel()
    {
        isPausing = false;
        pausingCanvas.SetActive(false);
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in audioSources)
        {
            audioSource.enabled = true;
        }
    }

    public void EndGame()
    {
        Debug.Log("You lost! Remember to check for the AQI index and keep it as low as possible!");
    }
}
