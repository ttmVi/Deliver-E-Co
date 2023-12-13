using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission : MonoBehaviour
{
    public string description;

    public float timeLimit;
    public float timeRemaining;
    public float timeToAccept;

    public Vector3[] PickUpLocations;
    public Vector3 DropOffLocation;
    public bool isPickedUp;
    public bool isDroppedOff;

    public int reward;
    public int penalty;

    public bool isAccepted;
    public bool isCompleted;
    public bool isFailed;

    public string eventsTriggered;

    public Mission()
    {

    }

    public void DisplayMission()
    {
        isAccepted = false;
        timeToAccept -= Time.deltaTime;
    }

    public void AcceptMission()
    {
        isAccepted = true;
        timeRemaining = timeLimit;
    }
    
    public void DeclineMission()
    {
        isAccepted = false;
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
}

public class PackageMission : Mission
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
}
