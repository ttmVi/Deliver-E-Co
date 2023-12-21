using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Vehicle
{
    public float vehicleSpeed {  get; set; }
    public float vehicleMPG { get; set; }
    public float vehicleFuel { get; set; }
    public string vehicleFeature { get; set; }
    public int vehicleCapacity { get; set; }

    public virtual void StartVehicle()
    {
        Debug.Log("Vehicle started");
    }
}

public class Bicycle : Vehicle
{
    public Bicycle(float speed, float MPG, float fuel, string feature, int capacity)
    {
        vehicleSpeed = speed;
        vehicleMPG = MPG;
        vehicleFuel = fuel;
        vehicleFeature = feature;
        vehicleCapacity = capacity;
    }

    public override void StartVehicle()
    {
        Debug.Log("Bicycle started");
    }
}

public class Motorbike : Vehicle
{
    public Motorbike(float speed, float MPG, float fuel, string feature, int capacity)
    {
        vehicleSpeed = speed;
        vehicleMPG = MPG;
        vehicleFuel = fuel;
        vehicleFeature = feature;
        vehicleCapacity = capacity;
    }

    public override void StartVehicle()
    {
        Debug.Log("Motorbike started");
    }
}

public class Updating : Vehicle
{
    public Updating(float speed, float MPG, float fuel, string feature, int capacity)
    {
        vehicleSpeed = speed;
        vehicleMPG = MPG;
        vehicleFuel = fuel;
        vehicleFeature = feature;
        vehicleCapacity = capacity;
    }

    public override void StartVehicle()
    {
        Debug.Log("Still updating");
    }
}
