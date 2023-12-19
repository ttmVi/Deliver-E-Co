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
    public float timeToAccept = 100f;

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

    public bool CheckPickUpAndDropOff(BoxCollider location)
    {
        bool isPickedUpOrDroppedOff = false;
        Vector3 triggerArea = new Vector3(location.size.x + laneWidth.laneWidth * 2, location.size.y + laneWidth.laneWidth * 2, location.size.z + laneWidth.laneWidth * 2);
        Collider[] shipper = Physics.OverlapBox(location.transform.position, triggerArea / 2, Quaternion.identity, LayerMask.GetMask("Player"));
        Debug.Log("Trigger area: " + triggerArea);

        if (shipper.Length > 0)
        {
            Debug.Log("Player detected");
            for (int i = 0; i < shipper.Length; i++)
            {
                if (shipper[i].gameObject.name == "Player")
                {
                    isPickedUpOrDroppedOff = true;
                }
            }
        }
        else { 
            Debug.Log("Player not detected");
            isPickedUpOrDroppedOff = false;}

        return isPickedUpOrDroppedOff;
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