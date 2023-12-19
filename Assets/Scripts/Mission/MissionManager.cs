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

    public Text missionDescription;
    public Text missionTimeLimit;
    public Text missionTimeRemaining;
    public Text missionTimeToAccept;
    public Text missionReward;
    public Text missionStatus;

    public float randomRegenerationTime;

    void Awake()
    {
        pickUpLocations = GameObject.FindGameObjectsWithTag("Buildings").ToList();
        dropOffLocations = GameObject.FindGameObjectsWithTag("Buildings").ToList();

        if (missionManager == null)
        {
            missionManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (missionManager != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentMissionID = 0;
        //public Mission(string description, GameObject pickUpLocation, GameObject dropOffLocation, float timeLimit, int reward, int penalty)

        //Chapter 1 missions
        AddNewMission("Pick up the package from the warehouse and drop it off at the customer", pickUpLocations[0], dropOffLocations[0], 100f, 100, 50);
        AddNewMission("Pick up the parcel from the warehouse and deliver it to the ... house", pickUpLocations[0], dropOffLocations[1], 100f, 100, 50);
    }

    // Update is called once per frame
    void Update()
    {
        //Display available missions
        //DisplayAvailableMission(Input.mousePosition);

        //Update remaining time to accept available missions
        for (int i = 0; i < availableMissions.Count; i++)
        {
            availableMissions[i].UpdateAvailableMission();
        }

        //Accept new missions

        //Update status of accepted missions

        for (int i = 0; i < acceptedMissions.Count; i++)
        {
            acceptedMissions[i].StartMission();
            //UpdateAcceptedMission(acceptedMissions[i]);
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
    }

    public void UpdateAcceptedMission(Mission mission)
    {
        if (mission.isAccepted && !mission.isCompleted && !mission.isFailed && !mission.isPickedUp)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (PickUpnDropOffCheck.CheckPickUpAndDropOff(mission, mission.pickUpLocation.GetComponent<BoxCollider>()))
                {
                    mission.GetPickingUp();
                }
            }
        }
        else if (mission.isAccepted && !mission.isCompleted && !mission.isFailed && mission.isPickedUp)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (PickUpnDropOffCheck.CheckPickUpAndDropOff(mission, mission.dropOffLocation.GetComponent<BoxCollider>()))
                {
                    mission.isDroppedOff = true;
                    mission.CompleteMission();
                }
            }
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

        Debug.Log($"New mission added: {mission.description}");
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
        for (int i = 0; i < availableMissions.Count; i++)
        {
            if (availableMissions[i] == null)
            {
                continue;
            }

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
        mission.AcceptMission();
        for (int i = 0; i < availableMissions.Count; i++)
        {
            if (mission == availableMissions[i])
            {
                acceptedMissions.Add(mission);
                availableMissions.RemoveAt(i);

                mission.pickUpLocation.AddComponent<PickUpnDropOffCheck>();
            }
        }
    }

    public void DeclineNewMission(Mission mission) //Or remove mission when timeToAccept is 0
    {
        for (int i = 0; i < availableMissions.Count; i++)
        {
            if (availableMissions[i] == mission)
            //if (availableMissions[i].timeToAccept == 0)
            {
                availableMissions.RemoveAt(i);
            }
            mission.DeclineMission();
        }
    }

    public void DisplayAcceptedMissions(Vector2 mousePosition)
    {
        for (int i = 0; i < acceptedMissions.Count; i++)
        {
            if (acceptedMissions[i] == null)
            {
                continue;
            }

            //if (acceptedMissions[i].position == mousePosition)
            missionDescription.text = acceptedMissions[i].description;
            missionTimeLimit.text = $"Time remaining for the mission is: {Mathf.RoundToInt(acceptedMissions[i].timeRemaining)}";
            missionStatus.text = $"The package is picked up: {acceptedMissions[i].isPickedUp}";
        }
    }

    public void CompleteMission(int i)
    {
        acceptedMissions.RemoveAt(i);
        //Add reward to player
    }

    public void FailMission(int i)
    {
        acceptedMissions.RemoveAt(i);
        //Add penalty to player
    }

    public void UpdateFinishedMissions()
    {
        for (int i = 0; i < acceptedMissions.Count; i++)
        {
            if (acceptedMissions[i].isCompleted)
            {
                CompleteMission(i);
                acceptedMissions.RemoveAt(i);
                completedMissions.Add(acceptedMissions[i]);
                //Add reward to player
            }
            else if (acceptedMissions[i].isFailed)
            {
                FailMission(i);
                acceptedMissions.RemoveAt(i);
                completedMissions.Add(acceptedMissions[i]);
                //Add penalty to player
            }
        }
    }
}
