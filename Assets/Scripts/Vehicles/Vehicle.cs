using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    public struct UpgradableComponent
    {
        public string category;
        public string name;
        public int price;
        public bool isUnlocked;
        public bool isChosen;
        public string previousComponent;
        public string description;

        public UpgradableComponent(string category, string name, int price, bool isUnlocked, bool isChosen, string previousComponent, string description)
        {
            this.category = category;
            this.name = name;
            this.price = price;
            this.isUnlocked = isUnlocked;
            this.isChosen = isChosen;
            this.previousComponent = previousComponent;
            this.description = description;
        }
    }

    public static UpgradableComponent[][] upgradeOptions;

    public virtual void StartVehicle()
    {
        Debug.Log("Vehicle started");
    }

    public virtual void UpgradeVehicle(string upgradeType)
    {
        Debug.Log($"Vehicle {upgradeType} upgraded");
    }

    public virtual void UnlockUpgradeComponent(string upgradeCategory, string upgradeComponent) { }

    public virtual void ChooseUpgradeComponent(string upgradeCategory, string upgradeComponent) { }

    public virtual UpgradableComponent[][] GetUpgradableComponent()
    {
        return upgradeOptions;
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
    public static string engineType = "Combustion Engine";
    public static string wheelType = "Steel Rims";
    public static string exhaustSystem = "Standard";
    public static UpgradableComponent[][] motorbikeUpgradeOptions;

    public enum MotorbikeUpgradableComponents
    {
        Engine,
        Wheels,
        ExhaustSystem
    }

    public Motorbike(float speed, float MPG, float fuel, string feature, int capacity)
        : base(speed, MPG, fuel, feature, capacity)
    {
        upgradeOptions = new UpgradableComponent[3][];

        // Engine upgrade options
        upgradeOptions[(int)MotorbikeUpgradableComponents.Engine] = new UpgradableComponent[]
        {
            new UpgradableComponent("Engine", "Combustion Engine", 0, true, true, "none", "The conventional engine, provides a balance between power and fuel consumption"),
            new UpgradableComponent("Engine", "Hybrid Engine", 200, false, false, "none", "Combination of internal combustion engine with electric motor"),
            new UpgradableComponent("Engine", "Electric Motor", 300, false, false, "none", "Releases zero carbondioxide emission with high efficiency torque")
        };

        // Wheels upgrade options
        upgradeOptions[(int)MotorbikeUpgradableComponents.Wheels] = new UpgradableComponent[]
        {
            new UpgradableComponent("Wheels", "Steel Rims", 0, true, true, "none", "Standard rim, nothing special"),
            new UpgradableComponent("Wheels", "Alloy Wheels", 200, false, false, "none", "Higher speed and torque, and higher fuel efficiency"),
            new UpgradableComponent("Wheels", "Magnesium Alloy Rims", 300, false, false, "none", "Lighter with better acceleration")
        };

        // Exhaust System upgrade options
        upgradeOptions[(int)MotorbikeUpgradableComponents.ExhaustSystem] = new UpgradableComponent[]
        {
            new UpgradableComponent("Exhaust System", "Standard", 0, true, true, "none", "Basic exhaust system with nothing special instead of air pollution"),
            new UpgradableComponent("Exhaust System", "Exhaust Tuning", 100, false, false, "none", "Airflow optimization and better engine performance"),
            new UpgradableComponent("Exhaust System", "Remove", 0, false, false, "Electric Motor", "Can only be removed after upgrading engine to Electric Motor")
        };

        motorbikeUpgradeOptions = upgradeOptions;
    }

    public override void StartVehicle()
    {
        Debug.Log("Motorbike started");
    }

    public override void UpgradeVehicle(string upgradeType)
    {
        Debug.Log($"Motorbike {upgradeType} upgraded");
        //Get vehicle's properties based on chosen engine
        for (int i = 0; i < motorbikeUpgradeOptions[(int)MotorbikeUpgradableComponents.Engine].Length; i++)
        {
            if (motorbikeUpgradeOptions[(int)MotorbikeUpgradableComponents.Engine][i].isChosen)
            {
                engineType = motorbikeUpgradeOptions[(int)MotorbikeUpgradableComponents.Engine][i].name;
                break;
            }
        }

        switch (upgradeType)
        {
            case "Engine":
                for (int i = 0; i < motorbikeUpgradeOptions[(int)MotorbikeUpgradableComponents.Engine].Length; i++)
                {
                    if (motorbikeUpgradeOptions[(int)MotorbikeUpgradableComponents.Engine][i].isChosen)
                    {
                        engineType = motorbikeUpgradeOptions[(int)MotorbikeUpgradableComponents.Engine][i].name;
                        break;
                    }
                }

                if (engineType.Contains("Combustion"))
                {
                    vehicleMPG = 50f;
                    vehicleFuel = 10f;
                }
                else if (engineType.Contains("Hybrid"))
                {
                    vehicleMPG = 70f;
                    vehicleFuel = 8f;
                }
                else if (engineType.Contains("Electric"))
                {
                    vehicleMPG = 100f;
                    vehicleFuel = 5f;
                }
                break;

            case "Wheels":
                for (int i = 0; i < motorbikeUpgradeOptions[(int)MotorbikeUpgradableComponents.Wheels].Length; i++)
                {
                    if (motorbikeUpgradeOptions[(int)MotorbikeUpgradableComponents.Wheels][i].isChosen)
                    {
                        wheelType = motorbikeUpgradeOptions[(int)MotorbikeUpgradableComponents.Wheels][i].name;
                        break;
                    }
                }

                if (wheelType == "Steel")
                {
                    vehicleSpeed = 4f;
                }
                else if (wheelType == "Alloy")
                {
                    vehicleSpeed = 5f;
                }
                else if (wheelType == "Magnesium Alloy")
                {
                    vehicleSpeed = 6f;
                }
                break;

            case "Exhaust":
                for (int i = 0; i < motorbikeUpgradeOptions[(int)MotorbikeUpgradableComponents.ExhaustSystem].Length; i++)
                {
                    if (motorbikeUpgradeOptions[(int)MotorbikeUpgradableComponents.ExhaustSystem][i].isChosen)
                    {
                        exhaustSystem = motorbikeUpgradeOptions[(int)MotorbikeUpgradableComponents.ExhaustSystem][i].name;
                    }
                    else { continue; }
                }

                if (exhaustSystem.Contains("Standard"))
                {
                }
                else if (exhaustSystem.Contains("Exhaust Tuning"))
                {
                    vehicleMPG += 20f;
                    vehicleFuel += 2f; ;
                }
                else if (exhaustSystem.Contains("Remove"))
                {
                    vehicleMPG = 100f;
                }
                break;
            default: break;
        }
    } //not really compatible with the new upgrade system yet

    public override void UnlockUpgradeComponent(string upgradeCategory, string upgradeComponent)
    {
        for (int i = 0; i < motorbikeUpgradeOptions.Length; i++)
        {
            if (motorbikeUpgradeOptions[i][0].category.Contains(upgradeCategory))
            {
                for (int j = 0; j < motorbikeUpgradeOptions[i].Length; j++)
                {
                    if (motorbikeUpgradeOptions[i][j].name.Contains(upgradeComponent) && motorbikeUpgradeOptions[i][j].previousComponent.Contains("none"))
                    {
                        motorbikeUpgradeOptions[i][j].isUnlocked = true;
                        break;
                    }
                    else if (motorbikeUpgradeOptions[i][j].name.Contains(upgradeComponent) && !motorbikeUpgradeOptions[i][j].previousComponent.Contains("none"))
                    {
                        //Checking if previous component is unlocked
                        //...

                        //For now just checking if it's chosen
                        if (motorbikeUpgradeOptions[i][j].previousComponent == engineType || motorbikeUpgradeOptions[i][j].previousComponent == wheelType || motorbikeUpgradeOptions[i][j].previousComponent == exhaustSystem)
                        {
                            motorbikeUpgradeOptions[i][j].isUnlocked = true;
                            break;
                        }
                    }
                    else { continue;}
                }
            }
            else { continue; }
        }
    }

    public override void ChooseUpgradeComponent(string upgradeCategory, string upgradeComponent)
    {
        for (int i = 0; i < motorbikeUpgradeOptions.Length; i++)
        {
            if (motorbikeUpgradeOptions[i][0].category.Contains(upgradeCategory))
            {
                for (int j = 0; j < motorbikeUpgradeOptions[i].Length; j++)
                {
                    if (motorbikeUpgradeOptions[i][j].name.Contains(upgradeComponent) && motorbikeUpgradeOptions[i][j].isUnlocked)
                    {
                        motorbikeUpgradeOptions[i][j].isChosen = true;
                    }
                    else
                    {
                        motorbikeUpgradeOptions[i][j].isChosen = false;
                    }

                    Debug.Log($"Choosing {motorbikeUpgradeOptions[i][j].name}: {motorbikeUpgradeOptions[i][j].isChosen}");
                }
            }
            else { continue; }
        }
    }

    public static UpgradableComponent[][] GetMotorbikeUpgradableComponent()
    {
        return motorbikeUpgradeOptions;
    }
}

public class Truck : Vehicle
{
    public static string engineType = "Combustion";
    public static string wheelType = "Steel";
    public static string batteryType = "None";
    public static string exhaustSystem = "Standard";
    public static UpgradableComponent[][] truckUpgradeOptions;

    public enum TruckUpgradableComponents
    {
        Engine,
        Wheels,
        Battery,
        ExhaustSystem
    }

    public Truck(float speed, float MPG, float fuel, string feature, int capacity)
        : base(speed, MPG, fuel, feature, capacity)
    {
        upgradeOptions = new UpgradableComponent[4][];

        // Engine upgrade options
        upgradeOptions[(int)TruckUpgradableComponents.Engine] = new UpgradableComponent[]
        {
            new UpgradableComponent("Engine", "Combustion", 0, true, true, "none", "Standard engine, releases a lot of carbon dioxide"),
            new UpgradableComponent("Engine", "Hybrid", 200, false, false, "none", "The combination between gastroline and electric motor, reduces emissions"),
            new UpgradableComponent("Engine", "Electric", 300, false, false, "none", "Vinfast")
        };

        // Wheels upgrade options
        upgradeOptions[(int)TruckUpgradableComponents.Wheels] = new UpgradableComponent[]
        {
            new UpgradableComponent("Wheels", "Steel", 0, true, true, "none", "The heavier it is, the more impact on fuel efficiency and handling"),
            new UpgradableComponent("Wheels", "Carbon Fiber", 200, false, false, "none", "Lighter and offer high strength"),
            new UpgradableComponent("Wheels", "Titanium", 300, false, false, "none", "Teen Titans")
        };

        // Battery upgrade options
        upgradeOptions[(int)TruckUpgradableComponents.Battery] = new UpgradableComponent[]
        {
            new UpgradableComponent("Battery", "None", 0, true, true, "none", "Battery can be installed after upgrading to electric motor"),
            new UpgradableComponent("Battery", "Lithium", 200, false, false, "Electric", "High energy storage density and lightweight, has good longevity"),
            new UpgradableComponent("Battery", "Sodium", 300, false, false, "Electric", "More abundant and sustainable")
        };

        // Exhaust System upgrade options
        upgradeOptions[(int)TruckUpgradableComponents.ExhaustSystem] = new UpgradableComponent[]
        {
            new UpgradableComponent("Exhaust System", "Standard", 0, true, true, "none", "Emission cannon"),
            new UpgradableComponent("Exhaust System", "Exhaust Tuning", 100, false, false, "none", "Reduces and filters out harmful emissions for a cleaner ride"),
            new UpgradableComponent("Exhaust System", "Remove", 0, false, false, "Electric", "Automatically removed after upgrading engine to Electric Moto")
        };

        truckUpgradeOptions = upgradeOptions;
    }

    public override void StartVehicle()
    {
        Debug.Log("Truck started");
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
                    vehicleMPG = 55f;
                    vehicleFuel = 16f;
                }
                else if (engineType == "Electric")
                {
                    vehicleMPG = 100f;
                    vehicleFuel = 12f;
                }
                break;
            case "Wheels":
                if (wheelType == "Steel")
                {
                    vehicleSpeed = 7f;
                }
                else if (wheelType == "Carbon Fiber")
                {
                    vehicleSpeed = 6f;
                }
                else if (wheelType == "Titanium")
                {
                    vehicleSpeed = 7f;
                }
                break;
            case "Battery":
                if (batteryType == "None")
                {

                }
                else if (batteryType == "Lithium")
                {
                    
                }
                else if (batteryType == "Sodium")
                {
                    vehicleMPG += 10f;
                    vehicleFuel += 4f;
                }
                break;
            case "Exhaust":
                if (exhaustSystem == "Standard")
                {

                }
                else if (exhaustSystem == "Exhaust Tuning")
                {
                    vehicleMPG += 10f;
                    vehicleFuel += 5f;
                }
                else if (exhaustSystem == "Remove")
                {
                    vehicleMPG = 100f;
                }
                break;
            default: break;
        }

        Debug.Log($"Truck {upgradeType} upgraded");
    }

    public override void UnlockUpgradeComponent(string upgradeCategory, string upgradeComponent)
    {
        for (int i = 0; i < truckUpgradeOptions.Length; i++)
        {
            if (truckUpgradeOptions[i][0].category.Contains(upgradeCategory))
            {
                for (int j = 0; j < truckUpgradeOptions[i].Length; j++)
                {
                    if (truckUpgradeOptions[i][j].name.Contains(upgradeComponent) && truckUpgradeOptions[i][j].previousComponent.Contains("none"))
                    {
                        truckUpgradeOptions[i][j].isUnlocked = true;
                        break;
                    }
                    else if (truckUpgradeOptions[i][j].name.Contains(upgradeComponent) && !truckUpgradeOptions[i][j].previousComponent.Contains("none"))
                    {
                        //Checking if previous component is unlocked
                        //...

                        //For now just checking if it's chosen
                        if (truckUpgradeOptions[i][j].previousComponent == engineType || truckUpgradeOptions[i][j].previousComponent == wheelType || truckUpgradeOptions[i][j].previousComponent == exhaustSystem)
                        {
                            truckUpgradeOptions[i][j].isUnlocked = true;
                            break;
                        }
                    }
                    else { continue; }
                }
            }
            else { continue; }
        }
    }

    public override void ChooseUpgradeComponent(string upgradeCategory, string upgradeComponent)
    {
        for (int i = 0; i < truckUpgradeOptions.Length; i++)
        {
            if (truckUpgradeOptions[i][0].category.Contains(upgradeCategory))
            {
                for (int j = 0; j < truckUpgradeOptions[i].Length; j++)
                {
                    if (truckUpgradeOptions[i][j].name.Contains(upgradeComponent) && truckUpgradeOptions[i][j].isUnlocked)
                    {
                        truckUpgradeOptions[i][j].isChosen = true;
                    }
                    else
                    {
                        truckUpgradeOptions[i][j].isChosen = false;
                    }

                    Debug.Log($"Choosing {truckUpgradeOptions[i][j].name}: {truckUpgradeOptions[i][j].isChosen}");
                }
            }
            else { continue; }
        }
    }
    public static UpgradableComponent[][] GetTruckUpgradableComponent() //For now it does nothing
    {
        return truckUpgradeOptions;
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
