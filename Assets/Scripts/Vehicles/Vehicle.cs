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

        public UpgradableComponent(string category, string name, int price, bool isUnlocked, bool isChosen, string previousComponent)
        {
            this.category = category;
            this.name = name;
            this.price = price;
            this.isUnlocked = isUnlocked;
            this.isChosen = isChosen;
            this.previousComponent = previousComponent;
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

    public virtual string[] GetEquippedAndUpgradableComponent(string category)
    {
        string[] currentlyEquippedComponent = {"none", ""};
        return currentlyEquippedComponent;
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
            new UpgradableComponent("Engine", "Combustion Engine", 0, true, true, "none"),
            new UpgradableComponent("Engine", "Hybrid Engine", 200, false, false, "none"),
            new UpgradableComponent("Engine", "Electric Motor", 300, false, false, "none")
        };

        // Wheels upgrade options
        upgradeOptions[(int)MotorbikeUpgradableComponents.Wheels] = new UpgradableComponent[]
        {
            new UpgradableComponent("Wheels", "Steel Rims", 0, true, true, "none"),
            new UpgradableComponent("Wheels", "Alloy Wheels", 200, false, false, "none"),
            new UpgradableComponent("Wheels", "Magnesium Alloy Rims", 300, false, false, "none")
        };

        // Exhaust System upgrade options
        upgradeOptions[(int)MotorbikeUpgradableComponents.ExhaustSystem] = new UpgradableComponent[]
        {
            new UpgradableComponent("Exhaust System", "Standard", 0, true, true, "none"),
            new UpgradableComponent("Exhaust System", "Exhaust Tuning", 100, false, false, "none"),
            new UpgradableComponent("Exhaust System", "Remove", 0, false, false, "Electric Motor")
        };
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
                for (int i = 0; i < upgradeOptions[(int)MotorbikeUpgradableComponents.Engine].Length; i++)
                {
                    if (upgradeOptions[(int)MotorbikeUpgradableComponents.Engine][i].isChosen)
                    {
                        engineType = upgradeOptions[(int)MotorbikeUpgradableComponents.Engine][i].name;
                        break;
                    }
                }

                if (engineType.Contains("Combustion"))
                {
                    vehicleMPG = 55f;
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
                for (int i = 0; i < upgradeOptions[(int)MotorbikeUpgradableComponents.Wheels].Length; i++)
                {
                    if (upgradeOptions[(int)MotorbikeUpgradableComponents.Wheels][i].isChosen)
                    {
                        wheelType = upgradeOptions[(int)MotorbikeUpgradableComponents.Wheels][i].name;
                        break;
                    }
                }

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

            case "Exhaust":
                for (int i = 0; i < upgradeOptions[(int)MotorbikeUpgradableComponents.ExhaustSystem].Length; i++)
                {
                    if (upgradeOptions[(int)MotorbikeUpgradableComponents.ExhaustSystem][i].isChosen)
                    {
                        exhaustSystem = upgradeOptions[(int)MotorbikeUpgradableComponents.ExhaustSystem][i].name;
                    }
                    else { continue; }
                }

                if (exhaustSystem.Contains("Standard"))
                {
                }
                else if (exhaustSystem.Contains("Exhaust Tuning"))
                {
                    vehicleMPG += 10f;
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
        for (int i = 0; i < upgradeOptions.Length; i++)
        {
            if (upgradeOptions[i][0].category.Contains(upgradeCategory))
            {
                for (int j = 0; j < upgradeOptions[i].Length; j++)
                {
                    if (upgradeOptions[i][j].name.Contains(upgradeComponent) && upgradeOptions[i][j].previousComponent.Contains("none"))
                    {
                        upgradeOptions[i][j].isUnlocked = true;
                        break;
                    }
                    else if (upgradeOptions[i][j].name.Contains(upgradeComponent) && !upgradeOptions[i][j].previousComponent.Contains("none"))
                    {
                        //Checking if previous component is unlocked
                        //...

                        //For now just checking if it's chosen
                        if (upgradeOptions[i][j].previousComponent == engineType || upgradeOptions[i][j].previousComponent == wheelType || upgradeOptions[i][j].previousComponent == exhaustSystem)
                        {
                            upgradeOptions[i][j].isUnlocked = true;
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
        for (int i = 0; i < upgradeOptions.Length; i++)
        {
            if (upgradeOptions[i][0].category.Contains(upgradeCategory))
            {
                for (int j = 0; j < upgradeOptions[i].Length; j++)
                {
                    if (upgradeOptions[i][j].name.Contains(upgradeComponent) && upgradeOptions[i][j].isUnlocked)
                    {
                        upgradeOptions[i][j].isChosen = true;
                        upgradeOptions[i][j - 1].isChosen = false;
                    }
                    else
                    {
                        upgradeOptions[i][j].isChosen = false;
                    }

                    Debug.Log($"Choosing {upgradeOptions[i][j].name}: {upgradeOptions[i][j].isChosen}");
                }
            }
            else { continue; }
        }
    }

    public override string[] GetEquippedAndUpgradableComponent(string category) //For now it does nothing
    {
        string[] currentlyEquippedComponent = { "...", "" };

        for (int i = 0; i < upgradeOptions.Length; i++)
        {
            if (upgradeOptions[i][0].category.Contains(category))
            {
                for (int j = 0; j < upgradeOptions[i].Length; j++)
                {
                    if (upgradeOptions[i][j].isChosen)
                    {
                        currentlyEquippedComponent[0] = upgradeOptions[i][j].name;
                        if (j + 1 < upgradeOptions[i].Length)
                        {
                            currentlyEquippedComponent[1] = upgradeOptions[i][j + 1].price.ToString();
                        }
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else { continue; }
        }

        return currentlyEquippedComponent;
    }
}

public class Truck : Vehicle
{
    public static string engineType = "Combustion";
    public static string wheelType = "Steel";
    public static string batteryType = "None";
    public static string exhaustSystem = "Standard";

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
            new UpgradableComponent("Engine", "Combustion", 0, true, true, "none"),
            new UpgradableComponent("Engine", "Hybrid", 200, false, false, "none"),
            new UpgradableComponent("Engine", "Electric", 300, false, false, "none")
        };

        // Wheels upgrade options
        upgradeOptions[(int)TruckUpgradableComponents.Wheels] = new UpgradableComponent[]
        {
            new UpgradableComponent("Wheels", "Steel", 0, true, true, "none"),
            new UpgradableComponent("Wheels", "Carbon Fiber", 200, false, false, "none"),
            new UpgradableComponent("Wheels", "Titanium", 300, false, false, "none")
        };

        // Battery upgrade options
        upgradeOptions[(int)TruckUpgradableComponents.Battery] = new UpgradableComponent[]
        {
            new UpgradableComponent("Battery", "None", 0, true, true, "none"),
            new UpgradableComponent("Battery", "Lithium", 200, false, false, "none"),
            new UpgradableComponent("Battery", "Sodium", 300, false, false, "none")
        };

        // Exhaust System upgrade options
        upgradeOptions[(int)TruckUpgradableComponents.ExhaustSystem] = new UpgradableComponent[]
        {
            new UpgradableComponent("Exhaust System", "Standard", 0, true, true, "none"),
            new UpgradableComponent("Exhaust System", "Exhaust Tuning", 100, false, false, "none"),
            new UpgradableComponent("Exhaust System", "Remove", 0, false, false, "Electric")
        };
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
                    vehicleMPG = 45f;
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
        for (int i = 0; i < upgradeOptions.Length; i++)
        {
            if (upgradeOptions[i][0].category.Contains(upgradeCategory))
            {
                for (int j = 0; j < upgradeOptions[i].Length; j++)
                {
                    if (upgradeOptions[i][j].name.Contains(upgradeComponent) && upgradeOptions[i][j].previousComponent.Contains("none"))
                    {
                        upgradeOptions[i][j].isUnlocked = true;
                        break;
                    }
                    else if (upgradeOptions[i][j].name.Contains(upgradeComponent) && !upgradeOptions[i][j].previousComponent.Contains("none"))
                    {
                        //Checking if previous component is unlocked
                        //...

                        //For now just checking if it's chosen
                        if (upgradeOptions[i][j].previousComponent == engineType || upgradeOptions[i][j].previousComponent == wheelType || upgradeOptions[i][j].previousComponent == exhaustSystem)
                        {
                            upgradeOptions[i][j].isUnlocked = true;
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
        for (int i = 0; i < upgradeOptions.Length; i++)
        {
            if (upgradeOptions[i][0].category.Contains(upgradeCategory))
            {
                for (int j = 0; j < upgradeOptions[i].Length; j++)
                {
                    if (upgradeOptions[i][j].name.Contains(upgradeComponent) && upgradeOptions[i][j].isUnlocked)
                    {
                        upgradeOptions[i][j].isChosen = true;
                        upgradeOptions[i][j - 1].isChosen = false;
                    }
                    else
                    {
                        upgradeOptions[i][j].isChosen = false;
                    }

                    Debug.Log($"Choosing {upgradeOptions[i][j].name}: {upgradeOptions[i][j].isChosen}");
                }
            }
            else { continue; }
        }
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
