using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    private float inputTime = 0;
    //private bool inputting = false;

    private Vector3 initialDirection;
    public Vector3 velDirection;
    private float direction;
    public float lastVelocityDirection;
    private float targetRotatingAngle = 0f;
    public float speed;
    public static float energy;
    public static int currentCapacity;

    private bool changingLane = false;
    private bool turning = false;
    private float rotatingDuration = 1.25f;
    public static bool isMoving;

    VehicleAnimation vehicleAnimation;
    VehicleSFX vehicleSFX;

    public GameObject player;
    public GameSceneManager sceneManager;
    public LaneState laneState;
    public Vector3[] LaneDirection;

    public AudioSource audioSource;
    public AudioClip[] soundEffects;

    private void Awake()
    {
        vehicleAnimation = GetComponent<VehicleAnimation>();
        vehicleSFX = GetComponent<VehicleSFX>();

        isMoving = false;
        initialDirection = transform.position - player.transform.position;
        // Define initial direction
        if (initialDirection.z > 0)
        {
            lastVelocityDirection = 1;
        }
        else if (initialDirection.z < 0)
        {
            lastVelocityDirection = 3;
        }
        else if (initialDirection.x > 0)
        {
                lastVelocityDirection = 2;               
        }
        else if (initialDirection.x < 0)
        {
                lastVelocityDirection = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        energy = VehicleManager.playerVehicle.vehicleFuel;
        speed = VehicleManager.playerVehicle.vehicleSpeed;
        currentCapacity = 0;

        player = GameObject.Find("Player");
        laneState = GameObject.Find("LaneIndicator").GetComponent<LaneState>();
        audioSource = player.GetComponent<AudioSource>();

        direction = 1.5f;
        velDirection = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (velDirection != Vector3.zero)
        {
            energy -= Time.deltaTime * 0.05f;
        }

        if (energy <= 0)
        {
            StartCoroutine(GameSceneManager.LoseLevel("Where's all the energy..."));
        }

        LaneDirection = laneState.laneDirection;

        // Determine if the player is pressing the key or holding the key
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && !turning)
        {
            //inputting = true;
            inputTime += Time.deltaTime;
        }
        else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && !turning)
        {
            //inputting = true;
            inputTime += Time.deltaTime;
        }

        // Turning or Changing lanes when the player release the key
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
                //inputting = false;

                if (inputTime > 0.5f) //&& LaneDirection.Length >= 2) // Turning
                {
                    vehicleAnimation.SetTurningLeft(true);
                    Debug.Log(inputTime.ToString());

                    inputTime = 0f;

                    targetRotatingAngle = -90f;
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

                else if (inputTime <= 0.5f) // Changing lanes
                {
                    Debug.Log(inputTime.ToString());
                    inputTime = 0f;

                    if (velDirection != Vector3.zero && !changingLane)
                    {
                        StartCoroutine(ChangingLanes(-1f));
                        Debug.Log("Changing lanes");
                    }
                }
        }
        else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
                //inputting = false;

                if (inputTime > 0.5f)// && LaneDirection.Length >= 2) // Turning
                {
                    vehicleAnimation.SetTurningRight(true);
                    Debug.Log(inputTime.ToString());

                    inputTime = 0f;

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

                else if (inputTime <= 0.5f) // Changing lanes
                {
                    Debug.Log(inputTime.ToString());
                    inputTime = 0f;
                    
                    if (velDirection != Vector3.zero && !changingLane)
                    {
                        StartCoroutine(ChangingLanes(1f));
                        Debug.Log("Changing lanes");
                    }
                }
        }

        // Define velocity direction
        switch (direction % 4)
        {
            case -3: velDirection = new Vector3(0, 0, 1); break;
            case -2: velDirection = new Vector3(1, 0, 0); break;
            case -1: velDirection = new Vector3(0, 0, -1); break;
            case 0: velDirection = new Vector3(-1, 0, 0); break;
            case 1: velDirection = new Vector3(0, 0, 1); break;
            case 2: velDirection = new Vector3(1, 0, 0); break;
            case 3: velDirection = new Vector3(0, 0, -1); break;
            default: velDirection = new Vector3(0, 0, 0); break;
        }

        // Stop and Go
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || GameSceneManager.isPausing)
        {
            if (velDirection != Vector3.zero)
            {
                lastVelocityDirection = direction;
                direction += 0.5f;

                //StopVehicleSound();
                vehicleAnimation.SetIsMoving(false);
                vehicleSFX.PlayBrakingSound();
                vehicleSFX.PlayIdleSound();

                isMoving = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (velDirection == Vector3.zero)
            {
                direction = lastVelocityDirection;

                //PlayVehicleSound();
                vehicleAnimation.SetIsMoving(true);
                vehicleSFX.PlayStartingSound();
                vehicleSFX.PlayDrivingSound();

                isMoving = true;
            }
        }

        // Check if the player is in the correct lane direction
        if (!changingLane && !turning)
        {
            IsInCorrectLaneDirection();
        }

        transform.Translate(velDirection * speed * Time.deltaTime, Space.World);
    }

    IEnumerator SmoothRotating(GameObject rotatingObject, float targetAngle, GameObject rotationCenter)
    {
        float rotationSpeed = 165f;

        //yield return new WaitForSeconds(0.5f);

        float rotatingObjectStartAngle = rotatingObject.transform.rotation.eulerAngles.y;
        float rotationCenterStartAngle = rotationCenter.transform.rotation.eulerAngles.y;
        float time = 0f;

        while (time < rotatingDuration)
        {
            turning = true;
            //rotatingObject.transform.RotateAround(rotationCenter.transform.position, Vector3.up, time / rotatingDuration);
            //rotationCenter.transform.RotateAround(rotationCenter.transform.position, Vector3.up, time / rotatingDuration);
            rotationCenter.transform.localEulerAngles = new Vector3(rotationCenter.transform.localEulerAngles.x, Mathf.Lerp(rotationCenterStartAngle, rotationCenterStartAngle + targetAngle, time / rotatingDuration), rotationCenter.transform.localEulerAngles.z);
            time += Time.deltaTime;
            yield return null;
        }
        turning = false;
        vehicleAnimation.SetTurningLeft(false);
        vehicleAnimation.SetTurningRight(false);
        rotationCenter.transform.localEulerAngles = new Vector3(rotationCenter.transform.localEulerAngles.x, rotationCenterStartAngle + targetAngle, rotationCenter.transform.localEulerAngles.z);
        //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, startAngle + targetAngle, transform.localEulerAngles.z);

        while (turning)
        {
            if (Mathf.Abs(targetAngle) > 0.25f)
            {
                float step = rotationSpeed * (targetAngle / Mathf.Abs(targetAngle)) * Time.deltaTime;
                rotatingObject.transform.RotateAround(rotationCenter.transform.position, Vector3.up, step);
                //rotationCenter.transform.RotateAround(rotatingObject.transform.position, Vector3.up, step);
                targetAngle -= step;
            }
            else
            {
                rotatingObject.transform.RotateAround(rotationCenter.transform.position, Vector3.up, targetAngle);
                //rotationCenter.transform.RotateAround(rotatingObject.transform.position, Vector3.up, targetAngle);
                targetAngle = 0;
                Debug.Log("finish rotation");
                turning = false;
            }
            yield return null;
        }
    }

    IEnumerator ChangingLanes(float changeLane)
    {
        changingLane = true;
        float stepleft = 2f;

        if (velDirection.x != 0)
        {
            while (changingLane)
            {
                if (stepleft > 0.01f)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Lerp(transform.position.z, transform.position.z + -velDirection.x * changeLane * stepleft, Time.deltaTime * speed));
                    stepleft -= Time.deltaTime * speed;
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Lerp(transform.position.z, transform.position.z + -velDirection.x * changeLane * stepleft, stepleft));
                    changingLane = false;
                }
                yield return null;
            }
        }
        else if (velDirection.z != 0)
        {
            while (changingLane)
            {
                if (stepleft > 0.01f)
                {
                    transform.position = new Vector3(Mathf.Lerp(transform.position.x, transform.position.x + velDirection.z * changeLane * stepleft, Time.deltaTime * speed), transform.position.y, transform.position.z);
                    stepleft -= Time.deltaTime * speed;
                }
                else
                {
                    transform.position = new Vector3(Mathf.Lerp(transform.position.x, transform.position.x + velDirection.z * changeLane * stepleft, stepleft), transform.position.y, transform.position.z);
                    changingLane = false;
                }
                yield return null;
            }
        }
    }

    void IsInCorrectLaneDirection()
    {
        for (int i = 0; i < LaneDirection.Length; i++)
        {
            if ((velDirection.x * LaneDirection[i].x == -1 || velDirection.z * LaneDirection[i].z == -1) && LaneDirection.Length == 1)
            {
                StartCoroutine(GameSceneManager.LoseLevel("And follow traffic rules!"));
                //return;
            }
            else { continue; }
        }
    }

    public void RefillEnergy()
    {
        int refillingCost = Mathf.RoundToInt((1 - energy / VehicleManager.playerVehicle.vehicleFuel) * 150);
        Debug.Log(refillingCost);
        MoneyManager.money -= refillingCost;
        energy = VehicleManager.playerVehicle.vehicleFuel;
        vehicleSFX.PlayRefuelingSound();
    }

    public void PlayVehicleSound()
    {
        if (VehicleManager.playerVehicle.vehicleSpeed == 2f)
        {
            Debug.Log("Bicycle Moving");
            audioSource.clip = soundEffects[0];
        }
        else if (VehicleManager.playerVehicle.vehicleSpeed == 5f)
        {
            audioSource.clip = soundEffects[1];
        }

        isMoving = true;
        audioSource.loop = true;
        audioSource.Play();        
    }

    public void StopVehicleSound()
    {
        isMoving = false;
        audioSource.loop = false;
        audioSource.Stop();
    }
}
