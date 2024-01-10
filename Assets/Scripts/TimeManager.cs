using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public static TimeManager timeManager;
    public float levelTimeLimit = 90f;
    private TextMeshProUGUI timeText;

    void Start()
    {
        levelTimeLimit = 90f;
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
                    Debug.Log("Time's up!");
                    StartCoroutine(GameSceneManager.LoseLevel("you ran out of time!"));
                }
                else { GameSceneManager.StartCustomizing(); }
            }
        }
    }

    public IEnumerator CountTime()
    {
        if (SceneManager.GetActiveScene().name == "Main Moving Scene")
        {
            timeText = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();

            Debug.Log("Starting to count time");

            while (levelTimeLimit > 0)
            {
                Debug.Log("1 second passed");
                levelTimeLimit--;
                timeText.text = $"Time: {levelTimeLimit.ToString()}";
                Debug.Log(levelTimeLimit);

                yield return new WaitForSeconds(1);
            }
        }
        else
        {
            levelTimeLimit = 90f;
            timeText = null;
        }
    }
}
