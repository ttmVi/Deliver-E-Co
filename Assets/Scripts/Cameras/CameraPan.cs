using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    public static Transform lookatObj;

    [SerializeField] Vector3 offset;
    [SerializeField] float rotationSpeed;
    [SerializeField] float minDistance;
    [SerializeField] float maxDistance;
    
    float distance;

    void Start()
    {
        transform.position = lookatObj.position + offset;
        distance = offset.magnitude;
    }

    void Update()
    {
        transform.LookAt(lookatObj);

        CameraOffsetUpdate(minDistance, maxDistance);
        if (Input.GetMouseButton(0))
        {
            CameraAngleUpdate();
        }
        CameraPosUpdate();
    }

    void CameraAngleUpdate()
    {
        float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;
        float vertical = -Input.GetAxis("Mouse Y") * rotationSpeed;

        transform.RotateAround(lookatObj.position, Vector3.up, horizontal);
        transform.RotateAround(lookatObj.position, transform.right, vertical);
    }

    void CameraPosUpdate()
    {
        Vector3 currentDistance = transform.position - lookatObj.position;

        if (currentDistance.magnitude > distance)
        {
            transform.position -= (currentDistance.normalized * Time.deltaTime * 10f);
        }
        else if (currentDistance.magnitude < (distance - 0.1f))
        {
            transform.position += (currentDistance.normalized * Time.deltaTime * 10f);
        }
    }

    void CameraOffsetUpdate(float minDistance, float maxDistance)
    {
        if (distance >= minDistance && distance <=  maxDistance)
        {
            distance = distance += -Input.mouseScrollDelta.y;
        }
        else if (distance > maxDistance)
        {
            distance = maxDistance;
        }
        else
        {
            distance = minDistance;
        }
    }
}
