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
    public int successfulMissionCount;
    public int requiredSuccessfulMissions;

    public static List<Mission> availableMissions = new List<Mission>();
    public static List<Mission> acceptedMissions = new List<Mission>();
    public static List<Mission> completedMissions = new List<Mission>();

    public List<GameObject> pickUpLocations = new List<GameObject>();
    public List<GameObject> dropOffLocations = new List<GameObject>();
    public GameObject pickUpLight;
    public GameObject dropOffLight;

    public float randomRegenerationTime;

    // Start is called before the first frame update
    void Start()
    {
        missionManager = this;

        pickUpLocations = GameObject.FindGameObjectsWithTag("PickUp & DropOff").ToList();
        dropOffLocations = GameObject.FindGameObjectsWithTag("PickUp & DropOff").ToList();

        currentMissionID = 0;
        successfulMissionCount = 0;
        availableMissions.Clear();
        acceptedMissions.Clear();
        completedMissions.Clear();
        //public Mission(string description, GameObject pickUpLocation, GameObject dropOffLocation, float timeLimit, int reward, int penalty)

        if (VehicleManager.playerVehicle.vehicleCapacity <= 2)
        {
            requiredSuccessfulMissions = 1;
        }
        else if (VehicleManager.playerVehicle.vehicleCapacity <= 4)
        {
            requiredSuccessfulMissions = 5;
        }
        else if (VehicleManager.playerVehicle.vehicleCapacity <= 10)
        {
            requiredSuccessfulMissions = 15;
        }
        else
        {
            requiredSuccessfulMissions = 4;
        }

        //Chapter 1 missions
        AddNewMission($"Pick up the package from {GameObject.Find("Storage").transform.GetChild(0).gameObject} and drop it off at {dropOffLocations[3]}", GameObject.Find("Storage").transform.GetChild(0).gameObject, dropOffLocations[3], 100f, 100, 50);
        AddNewMission($"Pick up the parcel from {pickUpLocations[1]} and deliver it to {dropOffLocations[6]}", pickUpLocations[1], dropOffLocations[6], 100f, 100, 50);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameSceneManager.isPausing)
        {
            //Update remaining time to accept available missions
            for (int i = 0; i < availableMissions.Count; i++)
            {
                availableMissions[i].UpdateAvailableMission();
                if (availableMissions[i].timeToAccept <= 0)
                {
                    completedMissions.Add(availableMissions[i]);
                    availableMissions.RemoveAt(i);
                    break;
                }
            }

            //Update status of accepted missions
            for (int i = 0; i < acceptedMissions.Count; i++)
            {
                acceptedMissions[i].StartMission();
                if (acceptedMissions[i].timeRemaining <= 0)
                {
                    acceptedMissions[i].FailMission();
                    break;
                }
            }

            // Generate random missions after a random time
            if (randomRegenerationTime <= 0f)
            {
                GenerateRandomMissions(Random.Range(1, 3));
                randomRegenerationTime = Random.Range(50f, 100f);
            }
            else
            {
                randomRegenerationTime -= Time.deltaTime;
            }

            UpdateFinishedMissions();
        }
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

        Debug.Log("Pick up locations: " + $"{pickUpLocation.name}");
    }

    public void GenerateRandomMissions(int numberOfMissionsGenerated)
    {
        for (int i = 0; i < numberOfMissionsGenerated; i++)
        {
            GameObject randomPickUp = pickUpLocations[Random.Range(0, pickUpLocations.Count)];
            dropOffLocations.Remove(randomPickUp);
            GameObject randomDropOff = dropOffLocations[Random.Range(0, dropOffLocations.Count)];
            dropOffLocations.Add(randomPickUp);

            float TimeLimit = (randomPickUp.transform.position - randomDropOff.transform.position).magnitude * 4 / VehicleManager.playerVehicle.vehicleSpeed + 20f;
            int randomReward = Random.Range(100, 300);
            int randomPenalty = Random.Range(75, 150);
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
                Vector3 pickUpLightPosition = new Vector3(mission.pickUpLocation.transform.position.x, mission.pickUpLocation.transform.position.y + 10f, mission.pickUpLocation.transform.position.z);
                GameObject tempPickUpLight = Instantiate(pickUpLight, pickUpLightPosition, Quaternion.identity);
                tempPickUpLight.name = $"PickUpLight_{mission.missionID}";
                tempPickUpLight.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                Vector3 dropOffLightPosition = new Vector3(mission.dropOffLocation.transform.position.x, mission.dropOffLocation.transform.position.y + 10f, mission.dropOffLocation.transform.position.z);
                GameObject tempDropOffLight = Instantiate(dropOffLight, dropOffLightPosition, Quaternion.identity);
                tempDropOffLight.name = $"DropOffLight_{mission.missionID}";
                tempDropOffLight.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

                acceptedMissions.Add(mission);
                availableMissions.RemoveAt(i);
                break;
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
                successfulMissionCount++;

                Destroy(GameObject.Find($"PickUpLight_{acceptedMissions[i].missionID}"));
                Destroy(GameObject.Find($"DropOffLight_{acceptedMissions[i].missionID}"));

                completedMissions.Add(acceptedMissions[i]);
                acceptedMissions.RemoveAt(i);
                continue;
            }
            else if (acceptedMissions[i].isFailed)
            {
                MoneyManager.money -= acceptedMissions[i].penalty;

                Destroy(GameObject.Find($"PickUpLight_{acceptedMissions[i].missionID}"));
                Destroy(GameObject.Find($"DropOffLight_{acceptedMissions[i].missionID}"));

                completedMissions.Add(acceptedMissions[i]);
                acceptedMissions.RemoveAt(i);
                continue;
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
