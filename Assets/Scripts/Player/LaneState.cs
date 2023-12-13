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
    public Vector3[] laneDirection;
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

    private void FixedUpdate()
    {
        laneDetection();
    }

    void laneDetection()
    {
        onLanes = Physics.OverlapBox(transform.position + indicatorBounds.center, indicatorBounds.size / 2, Quaternion.identity, LayerMask.GetMask("Road Lanes"));
        laneDirection = new Vector3[onLanes.Length];
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + indicatorBounds.center, indicatorBounds.size);
    }*/
}
