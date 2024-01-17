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
                    StartCoroutine(GameSceneManager.LoseLevel("you ran out of time!"));
                }
                else 
                { 
                    StartCoroutine(GameSceneManager.WinLevel()); 
                    Debug.Log("You won!");
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
                timeText.text = $"Time: {levelTimeLimit.ToString()}";

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
}
