using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionUIUpdate : MonoBehaviour
{
    private GameObject map3D;
    private BoxCollider map3DCollider;
    private RectTransform map2D;

    [SerializeField] GameObject pickUpLocationIcon;
    [SerializeField] GameObject dropOffLocationIcon;
    [SerializeField] Sprite acceptedPickUpLocationIcon;
    public static GameObject mapCanvas;
    public Canvas canvas;

    private List<int> instantiatedPickUpIconsID;
    private List<int> instantiatedDropOffIconsID;

    private GameObject missionInfoPanel;
    private TextMeshProUGUI missionInfo;
    private TextMeshProUGUI missionStatus;
    private TextMeshProUGUI status;
    private TextMeshProUGUI missionReward;
    private TextMeshProUGUI missionTime;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        mapCanvas = GameObject.Find("Map Canvas");
        map3D = GameObject.Find("Map 3D");
        map2D = GameObject.Find("Map").GetComponent<RectTransform>();

        map3DCollider = map3D.GetComponent<BoxCollider>();
        instantiatedPickUpIconsID = new List<int>();
        instantiatedDropOffIconsID = new List<int>();

        missionInfoPanel = GameObject.Find("Mission Info Panel");
        missionInfo = GameObject.Find("Description Text").GetComponent<TextMeshProUGUI>();
        missionStatus = GameObject.Find("Status Text").GetComponent<TextMeshProUGUI>();
        status = GameObject.Find("Mission Status").GetComponent<TextMeshProUGUI>();
        missionReward = GameObject.Find("Reward Text").GetComponent<TextMeshProUGUI>();
        missionTime = GameObject.Find("Time Text").GetComponent<TextMeshProUGUI>();

        missionInfoPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Main Moving Scene")
        {
            // Update locations for available missions
            for (int i = 0; i < MissionManager.availableMissions.Count; i++)
            {
                if (!instantiatedPickUpIconsID.Contains(MissionManager.availableMissions[i].missionID))
                {
                    SpawnLocationIcon(MissionManager.availableMissions[i].pickUpLocation, pickUpLocationIcon, MissionManager.availableMissions[i]);
                    instantiatedPickUpIconsID.Add(MissionManager.availableMissions[i].missionID);

                    missionInfoPanel.transform.SetAsLastSibling();
                }
            }

            // Update locations for accepted missions
            for (int i = 0; i < MissionManager.acceptedMissions.Count; i++)
            {
                if (!instantiatedPickUpIconsID.Contains(MissionManager.acceptedMissions[i].missionID) & !instantiatedDropOffIconsID.Contains(MissionManager.acceptedMissions[i].missionID))
                {
                    SpawnLocationIcon(MissionManager.acceptedMissions[i].pickUpLocation, pickUpLocationIcon, MissionManager.acceptedMissions[i]);
                    instantiatedPickUpIconsID.Add(MissionManager.acceptedMissions[i].missionID);

                    SpawnLocationIcon(MissionManager.acceptedMissions[i].dropOffLocation, dropOffLocationIcon, MissionManager.acceptedMissions[i]);
                    instantiatedDropOffIconsID.Add(MissionManager.acceptedMissions[i].missionID);

                    missionInfoPanel.transform.SetAsLastSibling();
                }
            }

            // Destroy icons for completed missions
            for (int i = MissionManager.completedMissions.Count - 1; i >= 0; i--)
            {
                if (MissionManager.completedMissions[i] == null) { continue; }

                for (int j = instantiatedPickUpIconsID.Count - 1; j >= 0; j--)
                {
                    if (MissionManager.completedMissions[i] != null &&
                        instantiatedPickUpIconsID[j] == MissionManager.completedMissions[i].missionID)
                    {
                        if (mapCanvas.activeSelf)
                        {
                            missionInfoPanel.SetActive(false);
                        }

                        Destroy(GameObject.Find($"PickUpLocation_{MissionManager.completedMissions[i].missionID}"));
                        Debug.Log($"Destroyed PickUpLocation_{MissionManager.completedMissions[i].missionID}");
                        instantiatedPickUpIconsID.RemoveAt(j);

                        break;
                    }
                    else { continue; }
                }

                for (int j = instantiatedDropOffIconsID.Count - 1; j >= 0; j--)
                {
                    if (MissionManager.completedMissions[i] != null &&
                        instantiatedDropOffIconsID[j] == MissionManager.completedMissions[i].missionID)
                    {
                        missionInfoPanel.SetActive(false);

                        Destroy(GameObject.Find($"DropOffLocation_{MissionManager.completedMissions[i].missionID}"));
                        Debug.Log($"Destroyed DropOffLocation_{MissionManager.completedMissions[i].missionID}");
                        instantiatedDropOffIconsID.RemoveAt(j);


                        break;
                    }
                    else { continue; }
                }

                GetMissionInfoText(MissionManager.completedMissions[i]);
                MissionManager.completedMissions[i] = null;
            }

            StartCoroutine(UpdateMissionInfoRealTime());
        }
    }

    public void StopDelivering()
    {
        foreach (int missionID in instantiatedPickUpIconsID)
        {
            Destroy(GameObject.Find($"PickUpLocation_{missionID}"));
        }
        instantiatedPickUpIconsID.Clear();

        foreach (int missionID in instantiatedDropOffIconsID)
        {
            Destroy(GameObject.Find($"DropOffLocation_{missionID}"));
        }
        instantiatedDropOffIconsID.Clear();

        MissionManager.availableMissions.Clear();
        MissionManager.acceptedMissions.Clear();
        MissionManager.completedMissions.Clear();
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
        mapPosition.x = normalizedPos.x * map2D.sizeDelta.x - (map2D.sizeDelta.x * 0.5f) + map2D.anchoredPosition.x;
        mapPosition.y = normalizedPos.y * map2D.sizeDelta.y - (map2D.sizeDelta.y * 0.5f) + map2D.anchoredPosition.y;

        //Debug.Log("Updated location icon position: " + mapPosition);
        return mapPosition;
    }

    public void SpawnLocationIcon(GameObject location, GameObject icon, Mission mission)
    {
        Vector2 mapPosition = Get2DMapCoordinate(location);
        GameObject locationIcon = Instantiate(icon, mapPosition, Quaternion.identity, mapCanvas.transform);
        locationIcon.name = $"{icon.name}_{mission.missionID}";
        locationIcon.transform.SetParent(mapCanvas.transform);

        locationIcon.GetComponent<RectTransform>().anchoredPosition = mapPosition;

        // Add event trigger for hover
        EventTrigger trigger = locationIcon.AddComponent<EventTrigger>();
        var pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((e) => ShowMissionInfo(e, mission));
        trigger.triggers.Add(pointerEnter);

        var pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((e) => AcceptMission(e, mission));
        trigger.triggers.Add(pointerClick);

        var pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((e) => HideMissionInfo(e, mission));
        trigger.triggers.Add(pointerExit);
    }

    public void ShowMissionInfo(BaseEventData eventData, Mission mission)
    {
        PointerEventData pointerData = eventData as PointerEventData;

        if (pointerData != null)
        {
            missionInfoPanel.SetActive(true);
            missionInfo.name = $"Mission Info_{mission.missionID}";

            AudioManager.audioManager.PlayHoverSound();

            GameObject tempPickUpIcon = GameObject.Find($"PickUpLocation_{mission.missionID}");
            tempPickUpIcon.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1.2f);

            if (!mission.isAccepted)
            {
                missionInfo.text = $"Pick up the package from {mission.pickUpLocation.transform.parent.gameObject.name} and deliver to {mission.dropOffLocation.transform.parent.gameObject.name}.";
                missionReward.text = $"{mission.reward}";
                missionTime.text = $"{Mathf.RoundToInt(mission.timeLimit)}";
                status.text = "Time to accept";
                missionStatus.text = $"{Mathf.RoundToInt(mission.timeToAccept)}";

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
                missionInfo.text = $"Pick up the package from {mission.pickUpLocation.transform.parent.gameObject.name} and deliver to {mission.dropOffLocation.transform.parent.gameObject.name}.";
                missionReward.text = $"{mission.reward}";
                missionTime.text = $"{Mathf.RoundToInt(mission.timeRemaining)}";
                status.text = "Status";
                missionStatus.text = $"Not picked up";
            }
            else if (mission.isAccepted && !mission.isCompleted && !mission.isFailed && mission.isPickedUp && !mission.isDroppedOff)
            {
                missionInfo.text = $"Pick up the package from {mission.pickUpLocation.transform.parent.gameObject.name} and deliver to {mission.dropOffLocation.transform.parent.gameObject.name}.";
                missionReward.text = $"{mission.reward}";
                missionTime.text = $"{Mathf.RoundToInt(mission.timeRemaining)}";
                status.text = "Status";
                missionStatus.text = $"Picked up";
            }
            else if (mission.isCompleted || mission.isFailed)
            {
                Destroy(GameObject.Find($"PickUpLocation_{mission.missionID}"));
                Destroy(GameObject.Find($"DropOffLocation_{mission.missionID}"));
            }

            GameObject tempDropOffIcon = GameObject.Find($"DropOffLocation_{mission.missionID}");
            tempDropOffIcon.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
        else
        {
            missionInfoPanel.SetActive(false);
        }
    }

    public void HideMissionInfo(BaseEventData eventData, Mission mission)
    {
        PointerEventData pointerData = eventData as PointerEventData;

        if (pointerData != null)
        {
            GameObject tempPickUpIcon = GameObject.Find($"PickUpLocation_{mission.missionID}");
            tempPickUpIcon.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

            if (!mission.isAccepted)
            {
                Destroy(GameObject.Find($"DropOffLocation_{mission.missionID}"));

                for (int i = 0; i < instantiatedDropOffIconsID.Count; i++)
                {
                    if (instantiatedDropOffIconsID.Contains(mission.missionID))
                    {
                        instantiatedDropOffIconsID.Remove(instantiatedDropOffIconsID[i]);
                    }
                    else { Debug.Log("No drop off icon ID to remove"); }
                }
            }
            else
            {
                GameObject tempDropOffIcon = GameObject.Find($"DropOffLocation_{mission.missionID}");
                tempDropOffIcon.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            }

            missionInfoPanel.SetActive(false);
        }
    }

    public void AcceptMission(BaseEventData eventData, Mission mission)
    {
        PointerEventData pointerData = eventData as PointerEventData;

        if (pointerData != null && !mission.isAccepted)
        {
            AudioManager.audioManager.PlayAcceptOrderSound();

            MissionManager.missionManager.AcceptNewMission(mission);
            GameObject.Find("PickUpLocation_" + mission.missionID).GetComponent<Image>().sprite = acceptedPickUpLocationIcon;
            GameObject.Find("PickUpLocation_" + mission.missionID).GetComponent<Animator>().enabled = false;

            //ShowMissionInfo(eventData, mission);
            //Destroy(statusButton.GetComponent<EventTrigger>());
        }
    }

    public void GetMissionInfoText(Mission mission)
    {
        if (!mission.isAccepted)
        {
            missionInfo.text = $"Pick up the package from {mission.pickUpLocation.transform.parent.gameObject.name} and deliver to {mission.dropOffLocation.transform.parent.gameObject.name}.";
            missionReward.text = $"{mission.reward}";
            missionTime.text = $"{Mathf.RoundToInt(mission.timeLimit)}";
            status.text = "Time to accept";
            missionStatus.text = $"{Mathf.RoundToInt(mission.timeToAccept)}";
        }
        else if (mission.isAccepted && !mission.isCompleted && !mission.isFailed && !mission.isPickedUp)
        {
            missionInfo.text = $"Pick up the package from {mission.pickUpLocation.transform.parent.gameObject.name} and deliver to {mission.dropOffLocation.transform.parent.gameObject.name}.";
            missionReward.text = $"{mission.reward}";
            missionTime.text = $"{Mathf.RoundToInt(mission.timeRemaining)}";
            status.text = "Status";
            missionStatus.text = $"Not picked up";
        }
        else if (mission.isAccepted && !mission.isCompleted && !mission.isFailed && mission.isPickedUp && !mission.isDroppedOff)
        {
            missionInfo.text = $"Pick up the package from {mission.pickUpLocation.transform.parent.gameObject.name} and deliver to {mission.dropOffLocation.transform.parent.gameObject.name}.";
            missionReward.text = $"{mission.reward}";
            missionTime.text = $"{Mathf.RoundToInt(mission.timeRemaining)}";
            status.text = "Status";
            missionStatus.text = $"Picked up";
        }
        else if (mission.isCompleted || mission.isFailed)
        {
            Destroy(GameObject.Find($"PickUpLocation_{mission.missionID}"));
            Destroy(GameObject.Find($"DropOffLocation_{mission.missionID}"));
        }
    }

    public IEnumerator UpdateMissionInfoRealTime()
    {
        while (missionInfoPanel.activeInHierarchy && SceneManager.GetActiveScene().name == "Main Moving Scene")
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


