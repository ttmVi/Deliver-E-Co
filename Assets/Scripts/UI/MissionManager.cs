using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public static MissionManager missionManager;

    public List<Mission> missions = new List<Mission>();

    //public Mission[] missions;
    public Mission[] availableMissions;
    public Mission[] acceptedMissions;

    public List<GameObject> pickUpLocations = new List<GameObject>();
    public List<GameObject> dropOffLocations = new List<GameObject>();

    public Text missionDescription;
    public Text missionTimeLimit;
    public Text missionTimeRemaining;
    public Text missionTimeToAccept;
    public Text missionReward;
    public Text missionStatus;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // create an array storing predefined missions and randomly add one into the available missions array
    public void AddNewMission(string description, GameObject pickUpLocation, GameObject dropOffLocation, float timeLimit, int reward, int penalty)
    {
        Mission mission = new Mission(description, pickUpLocation, dropOffLocation, timeLimit, reward, penalty);
        availableMissions.Append(mission);
    }

    public void GenerateRandomMissions(int numberOfMissionsGenerated)
    {
        for (int i = 0; i < numberOfMissionsGenerated; i++)
        {
            GameObject randomPickUp = pickUpLocations[Random.Range(0, pickUpLocations.Count)];
            GameObject randomDropOff = dropOffLocations[Random.Range(0, dropOffLocations.Count)];
            float TimeLimit = (randomPickUp.transform.position - randomDropOff.transform.position).magnitude * 2 + 50f;
            int randomReward = Random.Range(100, 1000);
            int randomPenalty = Random.Range(50, 200);
            string description = $"Pick up the package from {randomPickUp.name} and drop it off at {randomDropOff.name}";

            AddNewMission(description, randomPickUp, randomDropOff, TimeLimit, randomReward, randomPenalty);
        }
    }

    public void DisplayAvailableMission(Vector2 mousePosition)
    {
        for (int i = 0; i < availableMissions.Length; i++)
        {
            //if (availableMissions[i].position == mousePosition)
            //{
            missionDescription.text = availableMissions[i].description;
            missionTimeLimit.text = $"Time limit for the mission is: {availableMissions[i].timeLimit}";
            missionReward.text = $"Reward for the mission is: {availableMissions[i].reward}";
            missionStatus.text = $"Status of the mission is: Accepted {availableMissions[i].isAccepted}";
            missionTimeToAccept.text = $"Time to accept the mission is: {Mathf.RoundToInt(availableMissions[i].timeToAccept)}";
            //}
        }
    }

    public void AcceptNewMission(Mission mission)
    {
        for (int i = 0; i < availableMissions.Length; i++)
        {
            if (mission == availableMissions[i])
            {
                acceptedMissions.Prepend(mission);
                availableMissions[i] = null;
            }
        }
        mission.AcceptMission();
    }

    public void DeclineNewMission() //Or remove mission when timeToAccept is 0
    {
        for (int i = 0; i < availableMissions.Length; i++)
        {
            if (availableMissions[i].timeToAccept == 0)
            {
                availableMissions[i] = null;
            }
        }
    }

    public void DisplayAcceptedMissions()
    {
        for (int i = 0; i < acceptedMissions.Length; i++)
        {
            missionDescription.text = acceptedMissions[i].description;
            missionTimeLimit.text = $"Time remaining for the mission is: {Mathf.RoundToInt(acceptedMissions[i].timeRemaining)}";
            missionStatus.text = $"The package is picked up: {acceptedMissions[i].isPickedUp}\n The package is dropped: {acceptedMissions[i].isDroppedOff}";
        }
    }

    public void CompleteMission(Mission mission)
    {
        for (int i = 0; i < acceptedMissions.Length; i++)
        {
            if (mission == acceptedMissions[i])
            {
                acceptedMissions[i] = null;
            }
        }
        mission.CompleteMission();
    }

    public void FailMission(Mission mission)
    {
        for (int i = 0; i < acceptedMissions.Length; i++)
        {
            if (mission == acceptedMissions[i])
            {
                acceptedMissions[i] = null;
            }
        }
        mission.FailMission();
    }
}
