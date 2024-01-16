using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static VehicleFactory;

public class VehicleManager : MonoBehaviour
{
    public static VehicleManager vehicleManager;
    public static Vehicle playerVehicle;

    public VehicleType[] vehicles = { VehicleType.Bicycle, VehicleType.Motorbike, VehicleType.Car, VehicleType.Updating };
    public static bool[] vehicleIsUnlocked = { false, false, false, false };
    public int[] vehiclePrices = { 0, 100, 1000, 0 };
    [SerializeField] Sprite[] lockedSprites;
    [SerializeField] Sprite[] unlockedSprites;
    private int currentVehicleIndex = 0;

    public Vehicle.UpgradableComponent[][] upgradableComponents;
    private int currentUpgradableIndex = 0;

    private bool isInUpgradingUI = false;

    private GameObject vehicleCanvas;
    private GameObject selectButton;
    private TextMeshProUGUI selectButtonText;
    private GameObject upgradeButton;
    private TextMeshProUGUI upgradeButtonText;
    private TextMeshProUGUI displayedVehicleProperties;
    private GameObject locked;
    private Image vehicleImage;
    private GameObject confirmWindow;

    private void Awake()
    {
        vehicleCanvas = GameObject.Find("Vehicle Selection Canvas");

        selectButton = GameObject.Find("Select Vehicle Button");
        selectButtonText = GameObject.Find("Vehicle").GetComponent<TextMeshProUGUI>();

        upgradeButton = GameObject.Find("Upgrade Vehicle Button");
        upgradeButtonText = GameObject.Find("Upgrade").GetComponent<TextMeshProUGUI>();

        displayedVehicleProperties = GameObject.Find("Vehicle Properties").GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        locked = GameObject.Find("Locked?");
        vehicleImage = GameObject.Find("Vehicle Preview").GetComponent<Image>();

        confirmWindow = GameObject.Find("Confirm Window");
        confirmWindow.SetActive(false);
        upgradeButton.SetActive(false);

        DisplayVehicleProperties();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLevelWithVehicleProperties(VehicleType vehicleSelected)
    {
        playerVehicle = GetVehicleProperties(vehicleSelected);
        playerVehicle.StartVehicle();
    }

    public void Next()
    {
        if (vehicleIsUnlocked[currentVehicleIndex] && isInUpgradingUI)
        {
            if (currentUpgradableIndex != upgradableComponents.Length - 1)
            {
                currentUpgradableIndex++;
            }
            DisplayVehicleUpgradeComponent();
        }
        else
        {
            currentVehicleIndex = (currentVehicleIndex + 1) % vehicles.Length;
            DisplayVehicleProperties();
        }
    }

    public void Previous()
    {
        if (vehicleIsUnlocked[currentVehicleIndex] && isInUpgradingUI)
        {
            if (currentUpgradableIndex != 0)
            {
                currentUpgradableIndex--;
            }
            DisplayVehicleUpgradeComponent();
        }
        else
        {
            if (currentVehicleIndex == 0)
            {
                currentVehicleIndex = vehicles.Length - 1;
            }
            else
            {
                currentVehicleIndex--;
            }
            DisplayVehicleProperties();
        }
    }

    public void DisplayVehicleProperties()
    {
        //Vector3 tempPos = selectButton.GetComponent<RectTransform>().anchoredPosition;
        Image speed = GameObject.Find("Speed").GetComponent<Image>();
        Image mpg = GameObject.Find("MPG").GetComponent<Image>();
        Image fuel = GameObject.Find("Fuel").GetComponent<Image>();
        Image capacity = GameObject.Find("Package Capacity").GetComponent<Image>();

        VehicleType currentVehicleType = vehicles[currentVehicleIndex];
        Vehicle previewVehicle = GetVehicleProperties(currentVehicleType);
        if (vehicleIsUnlocked[currentVehicleIndex])
        {
            locked.SetActive(false);
            vehicleImage.sprite = unlockedSprites[currentVehicleIndex];
            displayedVehicleProperties.text = $"Vehicle: {currentVehicleType}\nSpeed: {previewVehicle.vehicleSpeed}\nMPG: {previewVehicle.vehicleMPG}\nFuel Capacity: {previewVehicle.vehicleFuel}\nPackage Capacity: {previewVehicle.vehicleCapacity}";
            selectButtonText.text = "Start";

            if (currentVehicleType != VehicleType.Bicycle)
            {
                //selectButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(275, tempPos.y, tempPos.z);
                upgradeButton.SetActive(true);
            }
            else { upgradeButton.SetActive(false); }

            speed.fillAmount = previewVehicle.vehicleSpeed / 10f;
            mpg.fillAmount = 1 - previewVehicle.vehicleMPG / 100f;
            fuel.fillAmount = previewVehicle.vehicleFuel / 100f;
            capacity.fillAmount = previewVehicle.vehicleCapacity / 10f;
        }
        else if (currentVehicleType == VehicleType.Updating)
        {
            locked.SetActive(true);
            displayedVehicleProperties.text = "Coming soon...";
            //selectButtonText.text = "Updating";

            speed.fillAmount = 0f;
            mpg.fillAmount = 0f;
            fuel.fillAmount = 0f;
            capacity.fillAmount = 0f;

            //selectButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, tempPos.y, tempPos.z);
            upgradeButton.SetActive(false);
        }
        else
        {
            locked.SetActive(true);
            vehicleImage.sprite = lockedSprites[currentVehicleIndex];
            displayedVehicleProperties.text = $"Vehicle: {currentVehicleType}\nSpeed: {previewVehicle.vehicleSpeed}\nMPG: {previewVehicle.vehicleMPG}\nFuel Capacity: {previewVehicle.vehicleFuel}\nPackage Capacity: {previewVehicle.vehicleCapacity}";
            //selectButtonText.text = $"Price: {vehiclePrices[currentVehicleIndex]}";

            //selectButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, tempPos.y, tempPos.z);
            upgradeButton.SetActive(false);

            speed.fillAmount = previewVehicle.vehicleSpeed / 10f;
            mpg.fillAmount = 1 - previewVehicle.vehicleMPG / 100f;
            fuel.fillAmount = previewVehicle.vehicleFuel / 25f;
            capacity.fillAmount = previewVehicle.vehicleCapacity / 10f;
        }
    }

    public void DisplayVehicleUpgradeComponent()
    {
        VehicleType currentVehicleType = vehicles[currentVehicleIndex];

        if (currentVehicleType == VehicleType.Motorbike)
        {
            //upgradableComponents = new string[] { "Engine", "Wheel", "Exhaust System" };

            upgradableComponents = Motorbike.upgradeOptions;

            if (upgradableComponents[currentUpgradableIndex][0].category == "Engine")
            {
                displayedVehicleProperties.text = $"Vehicle: {currentVehicleType}\n\n" +
                    $"Engine: {Motorbike.engineType}";
            }
            else if (upgradableComponents[currentUpgradableIndex][0].category == "Wheels")
            {
                displayedVehicleProperties.text = $"Vehicle: {currentVehicleType}\n\n" +
                    $"Wheel: {Motorbike.wheelType}";
            }
            else if (upgradableComponents[currentUpgradableIndex][0].category == "Exhaust System")
            {
                displayedVehicleProperties.text = $"Vehicle: {currentVehicleType}\n\n" +
                    $"Exhaust System: {Motorbike.exhaustSystem}";
            }
            
            for (int i = 0; i < upgradableComponents[currentUpgradableIndex].Length; i++)
            {
                if (upgradableComponents[currentUpgradableIndex][i].isChosen)
                {
                    if (i < upgradableComponents[currentUpgradableIndex].Length - 1)
                    {
                        upgradeButtonText.text = $"Upgrade: {upgradableComponents[currentUpgradableIndex][i + 1].price}";
                    }
                    else
                    {
                        upgradeButtonText.text = "Upgraded";
                    }
                }
            }
        }
        else if (currentVehicleType == VehicleType.Car)
        {
            upgradableComponents = Car.upgradeOptions;

            if (upgradableComponents[currentUpgradableIndex][0].category == "Engine")
            {
                displayedVehicleProperties.text = $"Vehicle: {currentVehicleType}\n\n" +
                    $"Engine: {Car.engineType}";
            }
            else if (upgradableComponents[currentUpgradableIndex][0].category == "Wheels")
            {
                displayedVehicleProperties.text = $"Vehicle: {currentVehicleType}\n\n" +
                    $"Wheel: {Car.wheelType}";
            }
            else if (upgradableComponents[currentUpgradableIndex][0].category == "Battery")
            {
                displayedVehicleProperties.text = $"Vehicle: {currentVehicleType}\n\n" +
                    $"Battery: {Car.batteryType}";
            }
            else if (upgradableComponents[currentUpgradableIndex][0].category == "Exhaust System")
            {
                displayedVehicleProperties.text = $"Vehicle: {currentVehicleType}\n\n" +
                    $"Exhaust System: {Car.exhaustSystem}";
            }

            for (int i = 0; i < upgradableComponents[currentUpgradableIndex].Length; i++)
            {
                if (upgradableComponents[currentUpgradableIndex][i].isChosen)
                {
                    if (i < upgradableComponents[currentUpgradableIndex].Length - 1)
                    {
                        upgradeButtonText.text = $"Upgrade: {upgradableComponents[currentUpgradableIndex][i + 1].price}";
                    }
                    else
                    {
                        upgradeButtonText.text = "Upgraded";
                    }
                }
            }
        }
    }

    public void UpgradeVehicle()
    {
        if (!isInUpgradingUI)
        {
            isInUpgradingUI = true;
            DisplayVehicleUpgradeComponent();

            selectButtonText.text = "Back";
        }
        else
        {
            ConfirmWindow($"Are you sure you want to upgrade {vehicles[currentVehicleIndex].ToString().ToLower()}?");
            UpgradeVehicleComponent(GetVehicleProperties(vehicles[currentVehicleIndex]));
        }
    }

    public void UpgradeVehicleComponent(Vehicle upgradedVehicle)
    {
        for (int i = 0; i < upgradableComponents[currentUpgradableIndex].Length - 1; i++)
        {
            if (upgradableComponents[currentUpgradableIndex][i].isChosen)
            {
                if (MoneyManager.money >= upgradableComponents[currentUpgradableIndex][i + 1].price)
                {
                    MoneyManager.money -= upgradableComponents[currentUpgradableIndex][i + 1].price;

                    upgradedVehicle.UnlockUpgradeComponent(upgradableComponents[currentUpgradableIndex][i + 1].category, upgradableComponents[currentUpgradableIndex][i + 1].name);
                    Debug.Log($"Unlocking {upgradableComponents[currentUpgradableIndex][i + 1].name} for {upgradedVehicle}");
                    upgradedVehicle.ChooseUpgradeComponent(upgradableComponents[currentUpgradableIndex][i + 1].category, upgradableComponents[currentUpgradableIndex][i + 1].name);
                    Debug.Log($"Choosing {upgradableComponents[currentUpgradableIndex][i + 1].name} for {upgradedVehicle}");
                }
                else
                {
                    Debug.Log("Not enough money");
                }
                break;
            }
        }

        DisplayVehicleUpgradeComponent();
    }

    public void SelectVehicle()
    {
        if (vehicleIsUnlocked[currentVehicleIndex])
        {
            if (!isInUpgradingUI)
            {
                StartLevelWithVehicleProperties(vehicles[currentVehicleIndex]);
            }
            else
            {
                isInUpgradingUI = false;
                DisplayVehicleProperties();

                //selectButtonText.text = "Start";
                //upgradeButtonText.text = "Upgrade";
            }
        }
    }

    public void BuyVehicle()
    {
        if (vehicles[currentVehicleIndex] != VehicleType.Updating)
        {
            if (MoneyManager.money >= vehiclePrices[currentVehicleIndex])
            {
                ConfirmWindow($"Are you sure you want to buy {vehicles[currentVehicleIndex].ToString().ToLower()} for {vehiclePrices[currentVehicleIndex]}?");
            }
            else
            {
                ConfirmWindow($"You don't have enough money to buy {vehicles[currentVehicleIndex].ToString().ToLower()}.");
            }
        }
        else { ConfirmWindow("We still haven't sold this product yet"); }
    }

    public void UnlockVehicle()
    {
        if (MoneyManager.money >= vehiclePrices[currentVehicleIndex] && vehicles[currentVehicleIndex] != VehicleType.Updating)
        {
            MoneyManager.money -= vehiclePrices[currentVehicleIndex];
            vehicleIsUnlocked[currentVehicleIndex] = true;
            DisplayVehicleProperties();
        }
        else
        {
            Debug.Log("You don't have enough money");
        }
    }

    public void ConfirmWindow(string message)
    {
        vehicleCanvas.SetActive(false);
        confirmWindow.SetActive(true);
        TextMeshProUGUI confirmMessage = GameObject.Find("Are you sure?").GetComponent<TextMeshProUGUI>();
        confirmMessage.text = message;
    }

    public void Confirm()
    {
        if (!isInUpgradingUI)
        {
            if (MoneyManager.money >= vehiclePrices[currentVehicleIndex])
            {
                vehicleCanvas.SetActive(true);
                UnlockVehicle();
            }
        }
        else
        {
            vehicleCanvas.SetActive(true);
            UpgradeVehicleComponent(GetVehicleProperties(vehicles[currentVehicleIndex]));
        }

        confirmWindow.SetActive(false);
    }

    public void Cancel()
    {
        vehicleCanvas.SetActive(true);
        confirmWindow.SetActive(false);
    }
}
