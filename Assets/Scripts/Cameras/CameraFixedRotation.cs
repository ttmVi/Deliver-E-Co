using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFixedRotation : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float rotationSpeed;
    [SerializeField] Vector3 offset;

    private float targetAngle = 0f;

    public bool isRotating;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CameraRotate();
    }

    void CameraRotate()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isRotating)
        {
            targetAngle = 90f;
        }
        else if (Input.GetKeyDown(KeyCode.E) && !isRotating)
        {
            targetAngle = -90f;
        }

        if (Mathf.Abs(targetAngle) > 0.25f)
        {
            isRotating = true;
            float step = rotationSpeed * (targetAngle / Mathf.Abs(targetAngle)) * Time.deltaTime;
            transform.RotateAround(player.position, Vector3.up, step);
            targetAngle -= step;
        }
        else
        {
            transform.RotateAround(player.position, Vector3.up, targetAngle);
            targetAngle = 0;
            isRotating = false;
        }
    }
}
