using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
    private Rigidbody player;
    private float segmentDistance;
    
    public GameObject direction;
    public PathFinding pathFinding;

    // Start is called before the first frame update
    void Start()
    {
        direction = GameObject.Find("Direction");
        player = GetComponent<Rigidbody>();
        segmentDistance = (direction.transform.position - transform.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        //
        //{
            Move();
        //}
        //player.velocity = (direction.transform.position - transform.position) * 2;
    }

    void Move()
    {
        Vector3 previousPosition = direction.transform.position;

        // Move rest of the body
        Vector3 temp = transform.position;
        //if (direction.GetComponent<Rigidbody>().velocity != Vector3.zero)
        //{
            transform.position = Vector3.Lerp(transform.position, previousPosition, segmentDistance / (temp - previousPosition).magnitude);
        //}
            
        //previousPosition = temp;
    }
}
