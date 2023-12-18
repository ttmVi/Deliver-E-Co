using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleFactory
{
    public enum VehicleType
    {
        Bicycle,
        Motorbike,
        Updating
    }

    public static Vehicle GetVehicleProperties(VehicleType vehicleType)
    {
        switch (vehicleType)
        {
            case VehicleType.Bicycle:
                return new Bicycle(2f, 0f, -1f, "package", 3); //(speed, MPG, fuel, feature, capacity)
            case VehicleType.Motorbike:
                return new Motorbike(5f, 10f, 10f, "package", 10); //(speed, MPG, fuel, feature, capacity)
            case VehicleType.Updating:
                return null;
            default:
                return null;
        }
    }
}
