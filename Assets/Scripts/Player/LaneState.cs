using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaneState : MonoBehaviour
{
    public bool isInHorizontalLane = false;
    public bool isInVerticalLane = false;
    public float laneWidth;
    public GameObject[] onLanes = new GameObject[2];
    public Vector3[] laneDirection = new Vector3[2];
    public string laneState;

    private BoxCollider indicatorBounds;

    // Start is called before the first frame update
    void Start()
    {
        indicatorBounds = GetComponent<BoxCollider>();

        indicatorBounds.size = new Vector3(laneWidth - 0.1f, 0.25f, laneWidth - 0.1f);
        indicatorBounds.center = new Vector3(0, -0.25f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < onLanes.Length; i++)
        {
            if (onLanes[i].CompareTag("Horizontal Lanes"))
            {
                isInHorizontalLane = true;
                if (onLanes[i].CompareTag("- Horizontal Lanes"))
                {
                    laneDirection[i] = new Vector3(-1, 0, 0);
                }
                else
                {
                    laneDirection[i] = new Vector3(1, 0, 0);
                }
            }
            else if (onLanes[i].CompareTag("Vertical Lanes"))
            {
                isInVerticalLane = true;
                if (onLanes[i].CompareTag("- Vertical Lanes"))
                {
                    laneDirection[i] = new Vector3(0, 0, -1);
                }
                else
                {
                    laneDirection[i] = new Vector3(0, 0, 1);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        onLanes.Append(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < onLanes.Length; i++)
        {
            if (onLanes[i] == other.gameObject)
            {
                onLanes[i] = null;
            }
        }
    }
}
