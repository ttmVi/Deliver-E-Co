using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public static MissionManager missionManager;

    public List<Mission> missions = new List<Mission>();
    public int currentMissionID;

    public static List<Mission> availableMissions = new List<Mission>();
    public static List<Mission> acceptedMissions = new List<Mission>();
    public static List<Mission> completedMissions = new List<Mission>();

    public List<GameObject> pickUpLocations = new List<GameObject>();
    public List<GameObject> dropOffLocations = new List<GameObject>();
    public PickUpnDropOffCheck PickUpnDropOffCheck;

    public float randomRegenerationTime;

    // Start is called before the first frame update
    void Start()
    {
        missionManager = this;

        pickUpLocations = GameObject.FindGameObjectsWithTag("PickUp & DropOff").ToList();
        dropOffLocations = GameObject.FindGameObjectsWithTag("PickUp & DropOff").ToList();


        currentMissionID = 0;
        //public Mission(string description, GameObject pickUpLocation, GameObject dropOffLocation, float timeLimit, int reward, int penalty)

        //Chapter 1 missions
        AddNewMission("Pick up the package from the warehouse and drop it off at the customer", pickUpLocations[2], dropOffLocations[0], 100f, 100, 50);
        AddNewMission("Pick up the parcel from the warehouse and deliver it to the ... house", pickUpLocations[3], dropOffLocations[1], 100f, 100, 50);
    }

    // Update is called once per frame
    void Update()
    {
        //Update remaining time to accept available missions
        for (int i = 0; i < availableMissions.Count; i++)
        {
            availableMissions[i].UpdateAvailableMission();
            if (availableMissions[i].timeToAccept <= 0)
            {
                completedMissions.Add(availableMissions[i]);
                availableMissions.RemoveAt(i);
                continue;
            }
        }

        //Update status of accepted missions
        for (int i = 0; i < acceptedMissions.Count; i++)
        {
            acceptedMissions[i].StartMission();
        }

        // Generate random missions after a random time
        if (randomRegenerationTime <= 0f)
        {
            GenerateRandomMissions(Random.Range(1,3));
            randomRegenerationTime = Random.Range(50f, 100f);
        }
        else
        {
            randomRegenerationTime -= Time.deltaTime;
        }

        UpdateFinishedMissions();
    }

    public void AddNewMission(string description, GameObject pickUpLocation, GameObject dropOffLocation, float timeLimit, int reward, int penalty)
    {
        currentMissionID++;
        Mission mission = new Mission(description, pickUpLocation, dropOffLocation, timeLimit, reward, penalty, currentMissionID);
        availableMissions.Add(mission);
        pickUpLocation.AddComponent<PickUpnDropOffCheck>();
        pickUpLocation.GetComponent<PickUpnDropOffCheck>().missionID = currentMissionID;
        dropOffLocation.AddComponent<PickUpnDropOffCheck>();
        dropOffLocation.GetComponent<PickUpnDropOffCheck>().missionID = currentMissionID;

        pickUpLocations.Remove(pickUpLocation);
        dropOffLocations.Remove(dropOffLocation);

        Debug.Log($"New mission added: {mission.description}");
    }

    public void GenerateRandomMissions(int numberOfMissionsGenerated)
    {
        for (int i = 0; i < numberOfMissionsGenerated; i++)
        {
            GameObject randomPickUp = pickUpLocations[Random.Range(0, pickUpLocations.Count)];
            dropOffLocations.Remove(randomPickUp);
            GameObject randomDropOff = dropOffLocations[Random.Range(0, dropOffLocations.Count)];
            dropOffLocations.Add(randomPickUp);

            float TimeLimit = (randomPickUp.transform.position - randomDropOff.transform.position).magnitude * 3 / VehicleManager.playerVehicle.vehicleSpeed + 50f;
            int randomReward = Random.Range(100, 1000);
            int randomPenalty = Random.Range(50, 200);
            string description = $"Pick up the package from {randomPickUp.name} and drop it off at {randomDropOff.name}";

            AddNewMission(description, randomPickUp, randomDropOff, TimeLimit, randomReward, randomPenalty);
            
        }
    }

    public void AcceptNewMission(Mission mission)
    {
        mission.AcceptMission();
        for (int i = 0; i < availableMissions.Count; i++)
        {
            if (mission == availableMissions[i])
            {
                //mission.pickUpLocation.AddComponent<PickUpnDropOffCheck>();

                acceptedMissions.Add(mission);
                availableMissions.RemoveAt(i);
            }
        }
    }

    /*public void DeclineNewMission(Mission mission) //Or remove mission when timeToAccept is 0
    {
        for (int i = 0; i < availableMissions.Count; i++)
        {
            if (availableMissions[i].isDeclined)
            {
                availableMissions.Remove(availableMissions[i]);
                completedMissions.Add(availableMissions[i]);
            }
            mission.DeclineMission();
        }
    }*/

    public void UpdateFinishedMissions()
    {
        for (int i = 0; i < acceptedMissions.Count; i++)
        {
            if (acceptedMissions[i].isCompleted)
            {
                MoneyManager.money += acceptedMissions[i].reward;

                //Destroy(acceptedMissions[i].pickUpLocation.GetComponent<PickUpnDropOffCheck>());
                //Destroy(acceptedMissions[i].dropOffLocation.GetComponent<PickUpnDropOffCheck>());

                completedMissions.Add(acceptedMissions[i]);
                acceptedMissions.RemoveAt(i);
            }
            else if (acceptedMissions[i].isFailed)
            {
                MoneyManager.money -= acceptedMissions[i].penalty;

                //Destroy(acceptedMissions[i].pickUpLocation.GetComponent<PickUpnDropOffCheck>());
                //Destroy(acceptedMissions[i].dropOffLocation.GetComponent<PickUpnDropOffCheck>());

                completedMissions.Add(acceptedMissions[i]);
                acceptedMissions.RemoveAt(i);

            }
        }
    }

    public void UpdateCompletedMissions()
    {
        for (int i = 0; i < completedMissions.Count; i++)
        {
            Destroy(completedMissions[i].pickUpLocation.GetComponent<PickUpnDropOffCheck>());
            Destroy(completedMissions[i].dropOffLocation.GetComponent<PickUpnDropOffCheck>());

            pickUpLocations.Add(completedMissions[i].pickUpLocation);
            dropOffLocations.Add(completedMissions[i].dropOffLocation);
        }
    }
}
