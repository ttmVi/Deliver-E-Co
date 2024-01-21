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

    //private GameObject manual;

    public GameObject mapCanvas;

    private static GameObject losingCanvas;
    private static GameObject winningCanvas;
    private GameObject pausingCanvas;
    private GameObject notification;

    public TextMeshProUGUI loseText;

    public bool mapIsLoaded = false;
    public static bool isPausing = false;

    // Start is called before the first frame update
    void Start()
    {
        mapCanvas = GameObject.Find("Map Canvas");
        mapCanvas.SetActive(false);

        if (SceneManager.GetActiveScene().name == "Main Moving Scene")
        {
            losingCanvas = GameObject.Find("Losing Canvas");
            losingCanvas.SetActive(false);
            winningCanvas = GameObject.Find("Winning Canvas");
            winningCanvas.SetActive(false);
            notification = GameObject.Find("Notification");
            notification.SetActive(false);
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

            AudioManager.audioManager.PlayMainGameplayBGM();
        }
        else if (SceneManager.GetActiveScene().name == "Vehicle Customize")
        {
            AudioManager.audioManager.PlayVehicleSelectionBGM();
        }
        else if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            AudioManager.audioManager.PlayMainMenuBGM();
            //manual = GameObject.Find("Manual");
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
            AudioManager.audioManager.PlayCloseMapSound();
            mapIsLoaded = false;
        }
        else
        {
            mapCanvas.SetActive(true);
            AudioManager.audioManager.PlayOpenMapSound();
            mapIsLoaded = true;
        }
    }

    public void LoadManual()
    {
        Application.OpenURL("https://drive.google.com/file/d/1SUnNK543u9tRYq5uEt5pS4qoELoBGHbT/view?usp=sharing");
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
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in audioSources)
        {
            audioSource.enabled = true;
        }

        SceneManager.LoadScene("Vehicle Customize");
        VehicleManager.playerVehicle = null;
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

        while (!losingCanvas.activeSelf)
        {
            GameObject.Find("ResourcesManager").GetComponent<AQICalculation>().EndDayCheck();

            MoneyManager.money -= 100;
            break;
        }
        losingCanvas.SetActive(true);
        AudioManager.audioManager.PlayLosingSound();

        TextMeshProUGUI completedMissionsCount = GameObject.Find("CompletedMission Count").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI moneyLost = GameObject.Find("Money Lost").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI loseInfo = GameObject.Find("Losing Info").GetComponent<TextMeshProUGUI>();

        completedMissionsCount.text = $"{MissionManager.missionManager.successfulMissionCount}/{MissionManager.missionManager.requiredSuccessfulMissions}";
        moneyLost.text = "-100";
        loseInfo.text = loseReason;
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

        while (!winningCanvas.activeSelf)
        {
            GameObject.Find("ResourcesManager").GetComponent<AQICalculation>().EndDayCheck();
            break;
        }

        winningCanvas.SetActive(true);
        AudioManager.audioManager.PlayWinningSound();

        TextMeshProUGUI completedMissionsCount = GameObject.Find("CompletedMission Count").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI moneyEarned = GameObject.Find("Money Earned").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI winInfo = GameObject.Find("Winning Info").GetComponent<TextMeshProUGUI>();

        completedMissionsCount.text = $"{MissionManager.missionManager.successfulMissionCount}/{MissionManager.missionManager.requiredSuccessfulMissions}";
        moneyEarned.text = $"{MoneyManager.money - GameObject.Find("ResourcesManager").GetComponent<MoneyManager>().startingMoney}";

        if (MissionManager.missionManager.successfulMissionCount > MissionManager.missionManager.requiredSuccessfulMissions)
        {
            winInfo.text = "Great job! Keep it up!";
        }
        else
        {
            winInfo.text = "Try to do better tomorrow!";
        }

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

    public void ForceEndDay()
    {
        if (MissionManager.missionManager.successfulMissionCount < MissionManager.missionManager.requiredSuccessfulMissions)
        {
            StartCoroutine(Notification("You didn't complete enough orders!"));
        }
        else
        {
            StartCustomizing();
        }
    }

    public static IEnumerator EndGame()
    {
        Debug.Log("You lost! Remember to check for the AQI index and keep it as low as possible!");
        yield return new WaitForSeconds(5f);
        
        Debug.Log("Exiting game...");
        yield return new WaitForSeconds(3f);
        Application.Quit();
    }

    public IEnumerator Notification(string notif)
    {
        GameObject resumeButton = GameObject.Find("Resume Button");
        GameObject exitButton = GameObject.Find("Exit Button");

        resumeButton.SetActive(false);
        exitButton.SetActive(false);
        notification.SetActive(true);

        TextMeshProUGUI notificationText = GameObject.Find("Notification").GetComponent<TextMeshProUGUI>();
        notificationText.text = notif;

        yield return new WaitForSeconds(3f);
        notificationText.text = "";

        notification.SetActive(false);
        resumeButton.SetActive(true);
        exitButton.SetActive(true);
    }
}
