using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class VehiclesProperties
{
    public float vehicleSpeed {  get; protected set; }
    public float vehicleMPG { get; protected set; }
    public float vehicleFuel { get; protected set; }
    public string[] vehicleFeature { get; protected set; }
    public int vehicleCapacity { get; protected set; }
}

public class Bicycle : VehiclesProperties
{
    public Bicycle()
    {
        vehicleSpeed = 2f;
        vehicleMPG = 0f;
        vehicleFuel = -1f;
        vehicleFeature[0] = "Package";
        vehicleCapacity = 2;
    }
}

public class Motorbike : VehiclesProperties
{
    public Motorbike()
    {
        vehicleSpeed = 4f;
        vehicleMPG = 2f;
        vehicleFuel = 10f;
        vehicleFeature[0] = "Package";
        vehicleCapacity = 5;
    }
}
