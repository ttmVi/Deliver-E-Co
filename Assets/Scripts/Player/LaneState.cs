using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaneState : MonoBehaviour
{
    public bool isInHorizontalLane = false;
    public bool isInVerticalLane = false;
    public float laneWidth;
    public Collider[] onLanes;
    public Vector3[] laneDirection = new Vector3[5];

    private BoxCollider indicatorBounds;

    // Start is called before the first frame update
    void Start()
    {
        laneWidth = 2f;

        GameObject TrafficLanes = GameObject.Find("TrafficLanes");
        for (int i = 0; i < TrafficLanes.transform.GetChild(0).childCount; i++)
        {
            TrafficLanes.transform.GetChild(0).GetChild(i).gameObject.AddComponent<BoxCollider>();
            TrafficLanes.transform.GetChild(0).GetChild(i).gameObject.GetComponent<BoxCollider>().isTrigger = true;
        }
        for (int i = 0; i < TrafficLanes.transform.GetChild(1).childCount; i++)
        {
            TrafficLanes.transform.GetChild(1).GetChild(i).gameObject.AddComponent<BoxCollider>();
            TrafficLanes.transform.GetChild(1).GetChild(i).gameObject.GetComponent<BoxCollider>().isTrigger = true;
        }
        for (int i = 0; i < TrafficLanes.transform.GetChild(2).childCount; i++)
        {
            TrafficLanes.transform.GetChild(2).GetChild(i).gameObject.AddComponent<BoxCollider>();
            TrafficLanes.transform.GetChild(2).GetChild(i).gameObject.GetComponent<BoxCollider>().isTrigger = true;
        }
        for (int i = 0; i < TrafficLanes.transform.GetChild(3).childCount; i++)
        {
            TrafficLanes.transform.GetChild(3).GetChild(i).gameObject.AddComponent<BoxCollider>();
            TrafficLanes.transform.GetChild(3).GetChild(i).gameObject.GetComponent<BoxCollider>().isTrigger = true;
        }

        indicatorBounds = GetComponent<BoxCollider>();

        indicatorBounds.size = new Vector3(laneWidth - 1.25f, 1f, laneWidth - 1.25f);
        indicatorBounds.center = new Vector3(0, -0.75f, 0);        
    }

    // Update is called once per frame
    void Update()
    {
        laneDetection();

        for (int i = 0; i < onLanes.Length; i++)
        {
            if (onLanes[i].gameObject.CompareTag("Horizontal Lanes"))
            {
                isInHorizontalLane = true;
                laneDirection[i] = new Vector3(1, 0, 0);
            }
            else if (onLanes[i].gameObject.CompareTag("- Horizontal Lanes"))
            {
                isInHorizontalLane = true;
                laneDirection[i] = new Vector3(-1, 0, 0);
            }
            else if (onLanes[i].gameObject.CompareTag("Vertical Lanes"))
            {
                isInVerticalLane = true;
                laneDirection[i] = new Vector3(0, 0, 1);
            }
            else if (onLanes[i].gameObject.CompareTag("- Vertical Lanes"))
            {
                isInVerticalLane = true;
                laneDirection[i] = new Vector3(0, 0, -1);
            }
        }
    }

    void laneDetection()
    {
        onLanes = Physics.OverlapBox(transform.position + indicatorBounds.center, indicatorBounds.size / 2, Quaternion.identity, LayerMask.GetMask("Road Lanes"));
        laneDirection = new Vector3[onLanes.Length];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + indicatorBounds.center, indicatorBounds.size);
    }
}
