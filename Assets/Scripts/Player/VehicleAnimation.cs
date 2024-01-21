using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAnimation : MonoBehaviour
{
    private GameObject bicycle;
    private GameObject motorbike;
    private GameObject truck;

    private Animator vehicleAnimator;

    // Start is called before the first frame update
    void Start()
    {
        bicycle = GameObject.Find("Bicycle");
        motorbike = GameObject.Find("Motorbike");
        truck = GameObject.Find("Truck");

        if (VehicleManager.playerVehicle.vehicleCapacity == 2) //Bicycle
        {
            vehicleAnimator = bicycle.GetComponent<Animator>();
            motorbike.SetActive(false);
            truck.SetActive(false);
        }
        else if (VehicleManager.playerVehicle.vehicleCapacity == 4) //Motorbike
        {
            vehicleAnimator = motorbike.GetComponent<Animator>();
            bicycle.SetActive(false);
            truck.SetActive(false);
        }
        else if (VehicleManager.playerVehicle.vehicleCapacity == 10) //truck
        {
            vehicleAnimator = truck.GetComponent<Animator>();
            bicycle.SetActive(false);
            motorbike.SetActive(false);
        }

        vehicleAnimator.SetBool("isMoving", false);
        vehicleAnimator.SetBool("turningLeft", false);
        vehicleAnimator.SetBool("turningRight", false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIsMoving(bool isMoving)
    {
        vehicleAnimator.SetBool("isMoving", isMoving);
    }

    public void SetTurningLeft(bool turningLeft)
    {
        vehicleAnimator.SetBool("turningLeft", turningLeft);
    }

    public void SetTurningRight(bool turningRight)
    {
        vehicleAnimator.SetBool("turningRight", turningRight);
    }
}
