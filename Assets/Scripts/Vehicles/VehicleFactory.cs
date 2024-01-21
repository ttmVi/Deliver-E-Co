using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class VehicleFactory
{
    public enum VehicleType
    {
        Bicycle,
        Motorbike,
        Truck,
        Updating
    }

    private static Dictionary<VehicleType, Vehicle> vehicleCache = new Dictionary<VehicleType, Vehicle>();

    public static Vehicle GetVehicleProperties(VehicleType vehicleType)
    {
        if (!vehicleCache.TryGetValue(vehicleType, out Vehicle vehicle))
        {
            switch (vehicleType)
            {
                case VehicleType.Bicycle:
                    vehicle = new Bicycle(2.25f, 100f, -1f, "package", 2); // (Speed, MPG, fuel, feature, capacity)
                    break;
                case VehicleType.Motorbike:
                    vehicle = new Motorbike(4f, 55f, 10f, "package", 4); // (Speed, MPG, fuel, feature, capacity)
                    break;
                case VehicleType.Truck:
                    vehicle = new Truck(7f, 35f, 20f, "package", 10); // (Speed, MPG, fuel, feature, capacity)
                    break;
                case VehicleType.Updating:
                    vehicle = new Updating(0f, 0f, 0f, "none", 0);
                    break;
                default:
                    return null;
            }
            vehicleCache[vehicleType] = vehicle;
        }
        else
        {
            switch (vehicleType)
            {
                case VehicleType.Bicycle: break;
                case VehicleType.Motorbike:
                    vehicle.UpgradeVehicle("Engine");
                    vehicle.UpgradeVehicle("Wheels");
                    vehicle.UpgradeVehicle("Exhaust");
                    break;
                case VehicleType.Truck:
                    vehicle.UpgradeVehicle("Engine");
                    vehicle.UpgradeVehicle("Wheels");
                    vehicle.UpgradeVehicle("Battery");
                    vehicle.UpgradeVehicle("Exhaust");
                    break;
                case VehicleType.Updating: break;
                default: break;
            }
        }
        return vehicle;
    }

}
