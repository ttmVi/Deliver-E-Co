using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public Mission[] missions;
    public Mission[] availableMissions;
    public Mission[] acceptedMissions;

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
    public void AddNewMission(Mission mission)
    {
        availableMissions.Append(mission);
    }

    public void DisplayAvailableMissions()
    {
        for (int i = 0; i < availableMissions.Length; i++)
        {
            missionDescription.text = availableMissions[i].description;
            missionTimeLimit.text = $"Time limit for the mission is: {availableMissions[i].timeLimit}";
            missionReward.text = $"Reward for the mission is: {availableMissions[i].reward}";
            missionStatus.text = $"Status of the mission is: {availableMissions[i].isAccepted}";
            missionTimeToAccept.text = $"Time to accept the mission is: {Mathf.RoundToInt(availableMissions[i].timeToAccept)}";
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

    public void DeclineNewMission(Mission mission)
    {
        for (int i = 0; i < availableMissions.Length; i++)
        {
            if (mission == availableMissions[i])
            {
                availableMissions[i] = null;
            }
        }
        mission.DeclineMission();
    }

    public void DisplayAcceptedMissions()
    {
        for (int i=0; i < acceptedMissions.Length; i++)
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
