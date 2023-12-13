using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class VehiclesProperties
{
    public float vehicleSpeed {  get; protected set; }
    public float vehicleMPG { get; protected set; }
    public string[] vehicleFeature { get; protected set; }
}

public class Bicycle : VehiclesProperties
{
    public Bicycle()
    {
        vehicleSpeed = 2f;
        vehicleMPG = 0f;
        vehicleFeature[0] = "Package";
    }
}

public class Motorbike : VehiclesProperties
{
    public Motorbike()
    {
        vehicleSpeed = 4f;
        vehicleMPG = 2f;
        vehicleFeature[0] = "Package";
    }
}