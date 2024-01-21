using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public static TimeManager timeManager;
    public float levelTimeLimit = 90f;
    private TextMeshProUGUI timeText;

    private bool pausingTime = false;
    private bool isPlayingWarning = false;

    void Start()
    {
        levelTimeLimit = 200f;
        StartCoroutine(CountTime());
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Main Moving Scene")
        {
            if (levelTimeLimit <= 0)
            {
                if (MissionManager.missionManager.successfulMissionCount < MissionManager.missionManager.requiredSuccessfulMissions)
                {
                    StartCoroutine(GameSceneManager.LoseLevel("You ran out of time!"));
                }
                else 
                { 
                    StartCoroutine(GameSceneManager.WinLevel()); 
                    Debug.Log("You won!");
                }
            }
            else if (levelTimeLimit <= 10f && !isPlayingWarning)
            {
                StartCoroutine(Playing10SecondsWarning());
            }
            else if (levelTimeLimit <= 20f && !isPlayingWarning)
            {
                StartCoroutine(Playing20SecondsWarning());
                if (levelTimeLimit <= 10f)
                {
                    isPlayingWarning = false;
                }
            }

            if (!GameSceneManager.isPausing && pausingTime)
            {
                StartCoroutine(CountTime());
                //Debug.Log("Resumed Time");
            }
        }
    }

    public IEnumerator CountTime()
    {
        if (SceneManager.GetActiveScene().name == "Main Moving Scene")
        {
            pausingTime = false;
            timeText = GameObject.Find("Time Limit").GetComponent<TextMeshProUGUI>();

            while (levelTimeLimit > 0)
            {
                levelTimeLimit--;
                UpdateTime();

                yield return new WaitForSeconds(1);

                if (GameSceneManager.isPausing)
                {
                    pausingTime = true;
                    //Debug.Log("Paused Time");
                    yield break;
                }
            }
        }
        else
        {
            levelTimeLimit = 200f;
            timeText = null;
        }
    }

    public void UpdateTime()
    {
        timeText = GameObject.Find("Time Limit").GetComponent<TextMeshProUGUI>();
        float minutes = Mathf.FloorToInt(levelTimeLimit / 60);
        float seconds = Mathf.FloorToInt(levelTimeLimit % 60);
        timeText.text = $"{minutes.ToString("00")}:{seconds.ToString("00")}";
    }

    public IEnumerator Playing20SecondsWarning()
    {
        isPlayingWarning = true;
        AudioManager.audioManager.PlayTime20SecondsLeftSound();
        yield return new WaitForSeconds(8f);
        isPlayingWarning = false;
    }

    public IEnumerator Playing10SecondsWarning()
    {
        isPlayingWarning = true;
        AudioManager.audioManager.PlayTime10SecondsLeftSound();
        yield return new WaitForSeconds(8f);
        isPlayingWarning = false;
    }
}
