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
    
    public Vehicle (float speed, float MPG, float fuel, string feature, int capacity)
    {
        vehicleSpeed = speed;
        vehicleMPG = MPG;
        vehicleFuel = fuel;
        vehicleFeature = feature;
        vehicleCapacity = capacity;
    }

    public virtual void StartVehicle()
    {
        Debug.Log("Vehicle started");
    }

    public virtual void UpgradeVehicle(string upgradeType)
    {
        Debug.Log($"Vehicle {upgradeType} upgraded");
    }
}

public class Bicycle : Vehicle
{
    public Bicycle(float speed, float MPG, float fuel, string feature, int capacity)
        : base(speed, MPG, fuel, feature, capacity) { }

    public override void StartVehicle()
    {
        Debug.Log("Bicycle started");
    }

    public override void UpgradeVehicle(string upgradeType)
    {
        Debug.Log("Bicycle cannot be upgraded");
    }
}

public class Motorbike : Vehicle
{
    public string engineType { get; set; }
    public string wheelType { get; set; }
    public string batteryType { get; set; }
    public string exhaustSystem { get; set; }

    public Motorbike(float speed, float MPG, float fuel, string feature, int capacity)
        : base(speed, MPG, fuel, feature, capacity)
    {
        engineType = "Combustion";
        wheelType = "Steel";
        batteryType = "None";
        exhaustSystem = "";
    }

    public override void StartVehicle()
    {
        Debug.Log("Motorbike started");
    }

    public override void UpgradeVehicle(string upgradeType)
    {
        switch (upgradeType)
        {
            case "Engine":
                if (engineType == "Combustion")
                {
                    vehicleMPG = 55f;
                    vehicleFuel = 10f;
                }
                else if (engineType == "Hybrid")
                {
                    vehicleMPG = 20f;
                    vehicleFuel = 8f;
                }
                else if (engineType == "Electric")
                {
                    vehicleMPG = 0f;
                    vehicleFuel = 5f;
                }
                break;
            case "Wheels":
                if (wheelType == "Steel")
                {
                    vehicleSpeed = 5f;
                }
                else if (wheelType == "Alloy")
                {
                    vehicleSpeed = 6f;
                }
                else if (wheelType == "Magnesium Alloy")
                {
                    vehicleSpeed = 7f;
                }
                break;
            case "Battery":
                break;
            case "Exhaust":
                if (exhaustSystem == "Standard")
                {

                }
                else if (exhaustSystem == "Exhaust Tuning")
                {
                    vehicleMPG -= 10f;
                    vehicleFuel += 2f;
                }
                else if (exhaustSystem == "Remove")
                {
                    vehicleMPG = 0;
                }
                break;
            default: break;
        }

        Debug.Log($"Motorbike {upgradeType} upgraded");
    }
}

public class Car : Vehicle
{
    public string engineType { get; set; }
    public string wheelType { get; set; }
    public string batteryType { get; set; }
    public string exhaustSystem { get; set; }

    public Car(float speed, float MPG, float fuel, string feature, int capacity)
        : base(speed, MPG, fuel, feature, capacity)
    {
        engineType = "Combustion";
        wheelType = "Steel";
        batteryType = "None";
        exhaustSystem = "Standard";
    }

    public override void StartVehicle()
    {
        Debug.Log("Car started");
    }

    public override void UpgradeVehicle(string upgradeType)
    {
        switch (upgradeType)
        {
            case "Engine":
                if (engineType == "Combustion")
                {
                    vehicleMPG = 35f;
                    vehicleFuel = 20f;
                }
                else if (engineType == "Hybrid")
                {
                    vehicleMPG = 20f;
                    vehicleFuel = 16f;
                }
                else if (engineType == "Electric")
                {
                    vehicleMPG = 0f;
                    vehicleFuel = 12f;
                }
                break;
            case "Wheels":
                if (wheelType == "Steel")
                {
                    vehicleSpeed = 5f;
                }
                else if (wheelType == "Alloy")
                {
                    vehicleSpeed = 6f;
                }
                else if (wheelType == "Magnesium Alloy")
                {
                    vehicleSpeed = 7f;
                }
                break;
            case "Battery":
                break;
            case "Exhaust":
                if (exhaustSystem == "Standard")
                {

                }
                else if (exhaustSystem == "Exhaust Tuning")
                {
                    vehicleMPG -= 10f;
                    vehicleFuel += 5f;
                }
                else if (exhaustSystem == "Remove")
                {
                    vehicleMPG = 0;
                }
                break;
            default: break;
        }

        Debug.Log($"Motorbike {upgradeType} upgraded");
    }
}

public class Updating : Vehicle
{
    public Updating(float speed, float MPG, float fuel, string feature, int capacity)
        : base(speed, MPG, fuel, feature, capacity)
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
