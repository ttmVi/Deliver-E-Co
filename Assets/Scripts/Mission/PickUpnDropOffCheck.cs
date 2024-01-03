using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;

public class PickUpnDropOffCheck : MonoBehaviour
{
    public int missionID;
    private Mission mission;

    private TextMeshProUGUI status;

    public AudioSource audioSource;
    public AudioClip[] soundEffects;

    private void Start()
    {
        status = GameObject.Find("Status").GetComponent<TextMeshProUGUI>();
        status.text = "";

        audioSource = GameObject.Find("Player").GetComponent<AudioSource>();
        soundEffects = GameObject.Find("Direction").GetComponent<PathFinding>().soundEffects;

        for (int i = 0; i < MissionManager.availableMissions.Count; i++)
        {
            if (MissionManager.availableMissions[i].missionID == missionID)
            {
                mission = MissionManager.availableMissions[i];
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F pressed");
            if (mission.isAccepted && !mission.isPickedUp && !mission.isDroppedOff)
            {
                PlayPickUpSound();

                status.text = "Package is picked up";
                mission.isPickedUp = CheckPickUpAndDropOff(mission, mission.pickUpLocation.GetComponent<BoxCollider>());
                Debug.Log("Picked up: " + mission.isPickedUp);
            }
            else if (mission.isAccepted && mission.isPickedUp && !mission.isDroppedOff)
            {
                PlayDropOffSound();

                status.text = "Package is dropped off";
                mission.isDroppedOff = CheckPickUpAndDropOff(mission, mission.dropOffLocation.GetComponent<BoxCollider>());
                mission.isCompleted = mission.isDroppedOff;
                Debug.Log("Dropped off: " + mission.isDroppedOff);
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (mission != null)
        {
            if (mission.isAccepted && !mission.isPickedUp)
            {
                Vector3 gizmosSize = new Vector3(mission.pickUpLocation.GetComponent<BoxCollider>().size.x * mission.pickUpLocation.transform.localScale.x + 2f * 2, mission.pickUpLocation.GetComponent<BoxCollider>().size.y * mission.pickUpLocation.transform.localScale.y + 2f * 2, mission.pickUpLocation.GetComponent<BoxCollider>().size.z * mission.pickUpLocation.transform.localScale.z + 2f * 2);
                Gizmos.color = Color.red;
                Gizmos.DrawCube(mission.pickUpLocation.transform.position, gizmosSize);
            }
            else if (mission.isAccepted && !mission.isDroppedOff)
            {
                Vector3 gizmosSize = new Vector3(mission.dropOffLocation.GetComponent<BoxCollider>().size.x * mission.dropOffLocation.transform.localScale.x + 2f * 2, mission.dropOffLocation.GetComponent<BoxCollider>().size.y * mission.dropOffLocation.transform.localScale.y + 2f * 2, mission.dropOffLocation.GetComponent<BoxCollider>().size.z * mission.dropOffLocation.transform.localScale.z + 2f * 2);
                Gizmos.color = Color.green;
                Gizmos.DrawCube(mission.dropOffLocation.transform.position, gizmosSize);
            }
        }
    }

    public bool CheckPickUpAndDropOff(Mission mission, BoxCollider location)
    {
        bool isPickedUpOrDroppedOff = false;
        Vector3 triggerArea = new Vector3(location.size.x * location.transform.localScale.x + 2f * 2, location.size.y * location.transform.localScale.y + 2f * 2, location.size.z * location.transform.localScale.z + 2f * 2);
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
        else
        {
            isPickedUpOrDroppedOff = false;
            Debug.Log("Player not detected");
        }

        Debug.Log("Finish checking for player");
        return isPickedUpOrDroppedOff;
    }

    public void UpdateMissionInfo()
    {
        for (int i = 0; i < MissionManager.availableMissions.Count; i++)
        {
            if (MissionManager.missionManager.missions[i].missionID == missionID)
            {
                MissionManager.missionManager.missions[i].isPickedUp = mission.isPickedUp;
                MissionManager.missionManager.missions[i].isDroppedOff = mission.isDroppedOff;
            }
        }
        for (int i = 0; i < MissionManager.acceptedMissions.Count; i++)
        {
            if (MissionManager.missionManager.missions[i].missionID == missionID)
            {
                MissionManager.missionManager.missions[i].isPickedUp = mission.isPickedUp;
                MissionManager.missionManager.missions[i].isDroppedOff = mission.isDroppedOff;
            }
        }
    }

    public void PlayPickUpSound()
    {
        audioSource.clip = soundEffects[2];
        audioSource.Play();
    }

    public void PlayDropOffSound()
    {
        audioSource.clip = soundEffects[3];
        audioSource.Play();
    }
}
