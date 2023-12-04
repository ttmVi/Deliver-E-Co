using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    //private float inputTime = 0;
    //private bool pathFinder = false;
    //private float laneWidth = 2f;

    private float direction = 50;
    private float lastVelocityDirection;
    private float targetRotatingAngle = 0f;

    public bool turning = false;
    public Vector3 velDirection;
    public GameObject player;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        lastVelocityDirection = 0;
        velDirection = new Vector3(1, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && !turning)
        {
            targetRotatingAngle = -90f;
            //transform.RotateAround(player.transform.position, Vector3.up, targetRotatingAngle);
            //player.transform.RotateAround(player.transform.position, Vector3.up, targetRotatingAngle);
            StartCoroutine(SmoothRotating(gameObject, targetRotatingAngle, player));

            if (velDirection != Vector3.zero)
            {
                direction -= 1;
            }
            else
            {
                lastVelocityDirection -= 1;
            }
        }
        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && !turning)
        {
            targetRotatingAngle = 90f;
            //transform.RotateAround(player.transform.position, Vector3.up, targetRotatingAngle);
            //player.transform.RotateAround(player.transform.position, Vector3.up, targetRotatingAngle);
            StartCoroutine(SmoothRotating(gameObject, targetRotatingAngle, player));

            if (velDirection != Vector3.zero)
            {
                direction += 1;
            }
            else
            {
                lastVelocityDirection += 1;
            }
        }

        switch (direction % 4)
        {
            case 0: velDirection = new Vector3(-1, 0, 0); break;
            case 1: velDirection = new Vector3(0, 0, 1); break;
            case 2: velDirection = new Vector3(1, 0, 0); break;
            case 3: velDirection = new Vector3(0, 0, -1); break;
            default: velDirection = new Vector3(0, 0, 0); break;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (velDirection != Vector3.zero)
            {
                lastVelocityDirection = direction;
                direction += 0.5f;
            }
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (velDirection == Vector3.zero)
            {
                direction = lastVelocityDirection;
            }
        }

        transform.Translate(velDirection * speed * Time.deltaTime, Space.World);
    }

    IEnumerator SmoothRotating(GameObject rotatingObject, float targetAngle, GameObject rotationCenter)
    {
        turning = true;
        float rotationSpeed = 100f;

        //yield return new WaitForSeconds(0.5f);

        while (turning)
        {
            if (Mathf.Abs(targetAngle) > 0.25f)
            {
                float step = rotationSpeed * (targetAngle / Mathf.Abs(targetAngle)) * Time.deltaTime;
                rotatingObject.transform.RotateAround(rotationCenter.transform.position, Vector3.up, step);
                rotationCenter.transform.RotateAround(rotatingObject.transform.position, Vector3.up, step);
                targetAngle -= step;
            }
            else
            {
                rotatingObject.transform.RotateAround(rotationCenter.transform.position, Vector3.up, targetAngle);
                rotationCenter.transform.RotateAround(rotatingObject.transform.position, Vector3.up, targetAngle);
                targetAngle = 0;
                Debug.Log("finish rotation");
                turning = false;
            }
            yield return null;
        }
    }

    /*    void Turning()
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                inputTime += Time.deltaTime;
                pathFinder = true;

                if ((Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow)) && inputTime <= 0.5f)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + laneWidth);
                    inputTime = 0;
                    pathFinder= false;
                }
                else if ((Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow)) && inputTime <= 0.5f)
                {

                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - laneWidth);
                    inputTime = 0;
                    pathFinder = false;
                }

                else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && inputTime > 0.5f)
                {
                    direction -= 1;
                    transform.RotateAround(transform.position, Vector3.up, -90f);
                    player.transform.RotateAround(player.transform.position, Vector3.up, -90f);
                    inputTime = 0;
                }
                else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && inputTime > 0.5f)
                {
                    direction += 1;
                    transform.RotateAround(transform.position, Vector3.up, 90f);
                    player.transform.RotateAround(player.transform.position, Vector3.up, 90f);
                    inputTime = 0;
                }
            }

            inputTime = 0;
        }
    */
}
