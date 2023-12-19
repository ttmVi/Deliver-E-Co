using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class MissionUIUpdate : MonoBehaviour
{
    private GameObject map3D;
    private BoxCollider map3DCollider;
    private RectTransform map2D;

    public GameObject pickUpLocationIcon;
    public GameObject dropOffLocationIcon;
    public Canvas mapCanvas;

    private List<int> instantiatedPickUpIconsID;
    private List<int> instantiatedDropOffIconsID;

    private TextMeshProUGUI missionInfo;
    private TextMeshProUGUI missionStatus;
    private GameObject statusButton;
    private GameObject missionInfoPanel;

    // Start is called before the first frame update
    void Start()
    {
        mapCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        map3D = GameObject.Find("Map 3D");
        map2D = GameObject.Find("Map").GetComponent<RectTransform>();

        map3DCollider = map3D.GetComponent<BoxCollider>();
        instantiatedPickUpIconsID = new List<int>();
        instantiatedDropOffIconsID = new List<int>();

        missionInfo = GameObject.Find("Mission Info").GetComponent<TextMeshProUGUI>();
        missionStatus = GameObject.Find("Mission Status").GetComponent<TextMeshProUGUI>();
        statusButton = GameObject.Find("Status Button");
        missionInfoPanel = GameObject.Find("Mission Info Panel");

        missionInfoPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Update locations for available missions
        for (int i = 0; i < MissionManager.availableMissions.Count; i++)
        {
            if (!instantiatedPickUpIconsID.Contains(MissionManager.availableMissions[i].missionID))
            {
                SpawnLocationIcon(MissionManager.availableMissions[i].pickUpLocation, pickUpLocationIcon, MissionManager.availableMissions[i]);
                instantiatedPickUpIconsID.Add(MissionManager.availableMissions[i].missionID);
            }
        }

        // Update locations for accepted missions
        for (int i = 0; i < MissionManager.acceptedMissions.Count; i++)
        {
            if (!instantiatedPickUpIconsID.Contains(MissionManager.acceptedMissions[i].missionID))
            {
                SpawnLocationIcon(MissionManager.acceptedMissions[i].pickUpLocation, pickUpLocationIcon, MissionManager.acceptedMissions[i]);
                instantiatedPickUpIconsID.Add(MissionManager.acceptedMissions[i].missionID);

                SpawnLocationIcon(MissionManager.acceptedMissions[i].dropOffLocation, dropOffLocationIcon, MissionManager.acceptedMissions[i]);
                instantiatedDropOffIconsID.Add(MissionManager.acceptedMissions[i].missionID);
            }
        }

        // Destroy icons for completed missions
        for (int i = 0; i < MissionManager.completedMissions.Count; i++)
        {
            for (int j = 0; j < instantiatedPickUpIconsID.Count; j++)
            {
                if (instantiatedPickUpIconsID[j] == MissionManager.completedMissions[i].missionID &&
                    instantiatedDropOffIconsID[j] == MissionManager.completedMissions[i].missionID)
                {
                    Destroy(GameObject.Find($"PickUpLocation_{MissionManager.completedMissions[i].missionID}"));
                    instantiatedPickUpIconsID.Remove(instantiatedPickUpIconsID[j]);

                    Destroy(GameObject.Find($"DropOffLocation_{MissionManager.completedMissions[i].missionID}"));
                    instantiatedDropOffIconsID.Remove(instantiatedDropOffIconsID[j]);

                    MissionManager.completedMissions.RemoveAt(i);
                }
                else { continue; }
            }
        }

        StartCoroutine(UpdateMissionInfoRealTime());
    }

    public Vector2 Get2DMapCoordinate(GameObject location)
    {
        Vector3 relativePosition = location.transform.position - map3D.transform.position;

        relativePosition.x -= map3DCollider.center.x * map3D.transform.localScale.x;
        relativePosition.z -= map3DCollider.center.z * map3D.transform.localScale.z;

        Vector2 normalizedPos;
        normalizedPos.x = (relativePosition.x / (map3DCollider.size.x * map3D.transform.localScale.x)) + 0.5f;
        normalizedPos.y = (relativePosition.z / (map3DCollider.size.z * map3D.transform.localScale.z)) + 0.5f;

        normalizedPos.x = Mathf.Clamp(normalizedPos.x, 0, 1);
        normalizedPos.y = Mathf.Clamp(normalizedPos.y, 0, 1);
        //Debug.Log("Normalized position: " + normalizedPos);

        Vector2 mapPosition;
        mapPosition.x = normalizedPos.x * map2D.sizeDelta.x - (map2D.sizeDelta.x * 0.5f);
        mapPosition.y = normalizedPos.y * map2D.sizeDelta.y - (map2D.sizeDelta.y * 0.5f);

        //Debug.Log("Updated location icon position: " + mapPosition);
        return mapPosition;
    }

    public void SpawnLocationIcon(GameObject location, GameObject icon, Mission mission)
    {
        Vector2 mapPosition = Get2DMapCoordinate(location);
        GameObject locationIcon = Instantiate(icon, mapPosition, Quaternion.identity, mapCanvas.transform);
        locationIcon.name = $"{icon.name}_{mission.missionID}";

        locationIcon.GetComponent<RectTransform>().anchoredPosition = mapPosition;

        // Add event trigger for hover
        EventTrigger trigger = locationIcon.AddComponent<EventTrigger>();
        var pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((e) => ShowMissionInfo(e, mission, locationIcon));
        trigger.triggers.Add(pointerEnter);

        var pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((e) => HideMissionInfo(e, mission, locationIcon));
        trigger.triggers.Add(pointerClick);
    }

    public void ShowMissionInfo(BaseEventData eventData, Mission mission, GameObject locationIcon)
    {
        PointerEventData pointerData = eventData as PointerEventData;

        if (pointerData != null)
        {
            //locationIcon.GetComponent<Image>().tintColor = Color.red;

            missionInfoPanel.SetActive(true);
            missionInfo.name = $"Mission Info_{mission.missionID}";

            if (!mission.isAccepted)
            {
                missionInfo.text = $"Pick up the package from {mission.pickUpLocation.name} and deliver to {mission.dropOffLocation.name}. \n" +
                    $"Time limit: {mission.timeLimit}. \n" +
                    $"Reward: {mission.reward}. \n" +
                    $"Penalty: {mission.penalty}. \n" +
                    $"Time to accept: {mission.timeToAccept}.";
                missionStatus.text = "Accept";

                if (!instantiatedDropOffIconsID.Contains(mission.missionID))
                {
                    SpawnLocationIcon(mission.dropOffLocation, dropOffLocationIcon, mission);
                    instantiatedDropOffIconsID.Add(mission.missionID);
                }

                for (int i = 0; i < MissionManager.availableMissions.Count; i++)
                {
                    for (int j = 0; j < instantiatedDropOffIconsID.Count; j++)
                    {
                        if (instantiatedDropOffIconsID[j] == MissionManager.availableMissions[i].missionID && instantiatedDropOffIconsID[j] != mission.missionID)
                        {
                            Destroy(GameObject.Find($"DropOffLocation_{MissionManager.availableMissions[i].missionID}"));
                            instantiatedDropOffIconsID.Remove(instantiatedDropOffIconsID[j]);
                        }
                        else { continue; }
                    }
                }
            }
            else if (mission.isAccepted && !mission.isCompleted && !mission.isFailed && !mission.isPickedUp)
            {
                missionInfo.text = $"Pick up the package from {mission.pickUpLocation.name} and deliver to {mission.dropOffLocation.name}. \n" +
                    $"Time limit: {Mathf.RoundToInt(mission.timeRemaining)}. \n" +
                    $"Reward: {mission.reward}. \n" +
                    $"Penalty: {mission.penalty}.";

                missionStatus.text = "Accepted";
            }
            else if (mission.isAccepted && !mission.isCompleted && !mission.isFailed && mission.isPickedUp && !mission.isDroppedOff)
            {
                missionInfo.text = $"Pick up the package from {mission.pickUpLocation.name} and deliver to {mission.dropOffLocation.name}. \n" +
                    $"Time limit: {Mathf.RoundToInt(mission.timeRemaining)}. \n" +
                    $"Reward: {mission.reward}. \n" +
                    $"Penalty: {mission.penalty}.";

                missionStatus.text = "Picked up";
            }
            else if (mission.isCompleted || mission.isFailed)
            {
                //sth here i dunno yet
            }

            EventTrigger trigger = statusButton.AddComponent<EventTrigger>();
            var pointerClick = new EventTrigger.Entry();
            pointerClick.eventID = EventTriggerType.PointerClick;
            pointerClick.callback.AddListener((e) => AcceptMission(e, mission));
            trigger.triggers.Add(pointerClick);
        }
        else
        {
            missionInfoPanel.SetActive(false);
        }
    }

    public void HideMissionInfo(BaseEventData eventData, Mission mission, GameObject locationIcon)
    {
        PointerEventData pointerData = eventData as PointerEventData;

        if (pointerData != null)
        {
            if (!mission.isAccepted)
            {
                Destroy(GameObject.Find($"DropOffLocation_{mission.missionID}"));
                Debug.Log("Destroyed drop off icon");

                for (int i = 0; i < instantiatedDropOffIconsID.Count; i++)
                {
                    if (instantiatedDropOffIconsID.Contains(mission.missionID))
                    {
                        instantiatedDropOffIconsID.Remove(instantiatedDropOffIconsID[i]);
                    }
                    else { Debug.Log("No drop off icon ID to remove"); }
                }
            }

            missionInfoPanel.SetActive(false);
        }
    }

    public void AcceptMission(BaseEventData eventData, Mission mission)
    {
        PointerEventData pointerData = eventData as PointerEventData;

        if (pointerData != null)
        {
            MissionManager.missionManager.AcceptNewMission(mission);
        }
    }

    public void GetMissionInfoText(Mission mission)
    {
        if (!mission.isAccepted)
        {
            missionInfo.text = $"Pick up the package from {mission.pickUpLocation.name} and deliver to {mission.dropOffLocation.name}. \n" +
                $"Time limit: {mission.timeLimit}. \n" +
                $"Reward: {mission.reward}. \n" +
                $"Penalty: {mission.penalty}. \n" +
                $"Time to accept: {mission.timeToAccept}.";

            missionStatus.text = "Accept";
        }
        else if (mission.isAccepted && !mission.isCompleted && !mission.isFailed && !mission.isPickedUp)
        {
            missionInfo.text = $"Pick up the package from {mission.pickUpLocation.name} and deliver to {mission.dropOffLocation.name}. \n" +
                $"Time limit: {Mathf.RoundToInt(mission.timeRemaining)}. \n" +
                $"Reward: {mission.reward}. \n" +
                $"Penalty: {mission.penalty}.";

            missionStatus.text = "Accepted";
        }
        else if (mission.isAccepted && !mission.isCompleted && !mission.isFailed && mission.isPickedUp && !mission.isDroppedOff)
        {
            missionInfo.text = $"Pick up the package from {mission.pickUpLocation.name} and deliver to {mission.dropOffLocation.name}. \n" +
                $"Time limit: {Mathf.RoundToInt(mission.timeRemaining)}. \n" +
                $"Reward: {mission.reward}. \n" +
                $"Penalty: {mission.penalty}.";

            missionStatus.text = "Picked up";
        }
        else if (mission.isCompleted || mission.isFailed)
        {
            //sth here i dunno yet
        }
    }

    public IEnumerator UpdateMissionInfoRealTime()
    {
        while (missionInfoPanel.activeInHierarchy)
        {
            int missionID = -1;
            if (!int.TryParse(missionInfo.name.Split('_')[1], out missionID))
            {
                yield break; // Invalid mission ID, stop coroutine
            }

            Mission missionToUpdate = MissionManager.availableMissions
                .Concat(MissionManager.acceptedMissions)
                .FirstOrDefault(m => m.missionID == missionID);

            if (missionToUpdate != null)
            {
                GetMissionInfoText(missionToUpdate);
            }

            yield return new WaitForSeconds(0.5f); // Update every 0.5 seconds
        }
    }
}

