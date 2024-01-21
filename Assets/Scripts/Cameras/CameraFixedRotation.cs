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
    private float beforeRotation;
    private float rotatingDuration;

    public bool isRotating;

    void Start()
    {
        rotatingDuration = 0.5f;
        isRotating = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isRotating)
        {
            beforeRotation = transform.rotation.eulerAngles.y;
            targetAngle = 90f;
            StartCoroutine(CameraRotate());
        }
        else if (Input.GetKeyDown(KeyCode.E) && !isRotating)
        {
            beforeRotation = transform.rotation.eulerAngles.y;
            targetAngle = -90f;
            StartCoroutine(CameraRotate());
        }
    }

    IEnumerator CameraRotate()
    {
        //if (Mathf.Abs(targetAngle) > 0.25f)
        //{
            float startAngle = transform.rotation.eulerAngles.y;
            float time = 0f;
            while (time < rotatingDuration)
            {
                isRotating = true;
                transform.localEulerAngles = new Vector3 (transform.localEulerAngles.x, Mathf.Lerp(startAngle, startAngle + targetAngle, time / rotatingDuration), transform.localEulerAngles.z);
                time += Time.deltaTime;
                yield return null;
            }
            isRotating = false;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, startAngle + targetAngle, transform.localEulerAngles.z);
            //float step = rotationSpeed * (targetAngle / Mathf.Abs(targetAngle)) * Time.deltaTime;
            //transform.RotateAround(player.position, Vector3.up, step);
            //targetAngle -= step;
        //}
        //else //camera getting shaky might be because of this
        //{
        //    transform.RotateAround(player.position, Vector3.up, targetAngle);
        //    targetAngle = 0;
        //    isRotating = false;
        //}
    }
}
