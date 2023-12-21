using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission
{
    private LaneState laneWidth;

    public string description;
    public int missionID;

    public float timeLimit;
    public float timeRemaining;
    public float timeToAccept = 30f;

    public GameObject pickUpLocation;
    public GameObject dropOffLocation;
    public bool isPickedUp;
    public bool isDroppedOff;

    public int reward;
    public int penalty;

    public bool isAccepted;
    public bool isDeclined;
    public bool isCompleted;
    public bool isFailed;

    public string eventsTriggered;

    public Mission(string description, GameObject pickUpLocation, GameObject dropOffLocation, float timeLimit, int reward, int penalty, int missionID)
    {
        this.description = description;
        this.pickUpLocation = pickUpLocation;
        this.dropOffLocation = dropOffLocation;
        this.timeLimit = timeLimit;
        this.reward = reward;
        this.penalty = penalty;
        this.missionID = missionID;
    }

    public void DisplayMission()
    {
        isAccepted = false;
        timeToAccept -= Time.deltaTime;
    }

    public void UpdateAvailableMission()
    {
        timeToAccept -= Time.deltaTime;
        if (timeToAccept <= 0)
        {
            DeclineMission();
        }
    }

    public void AcceptMission()
    {
        isAccepted = true;
        timeRemaining = timeLimit;
    }
    
    public void DeclineMission()
    {
        isDeclined = true;
    }

    public void StartMission()
    {
        if (isAccepted)
        {
            timeRemaining -= Time.deltaTime;
        }
    }

    public void CompleteMission()
    {
        isCompleted = true;
        //reward player
    }

    public void FailMission()
    {
        isFailed = true;
        //penalize player
    }

    public void GetPickingUp()
    {
        isPickedUp = true;
    }
}

/*public class PackageMission : Mission
{
    public PackageMission()
    {

    }
}

public class PassengerMission : Mission
{
    public PassengerMission()
    {

    }
}*/
