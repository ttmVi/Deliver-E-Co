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

    public VehicleType[] vehicles = { VehicleType.Bicycle, VehicleType.Motorbike, VehicleType.Truck, VehicleType.Updating };
    public static bool[] vehicleIsUnlocked = { false, false, false, false };
    public int[] vehiclePrices = { 0, 100, 1000, 0 };
    [SerializeField] Sprite[] lockedSprites;
    [SerializeField] Sprite[] unlockedSprites;
    [SerializeField] Sprite[] upgradingSprites;
    [SerializeField] Sprite[][] motorbikeComponentsSprites;
    [SerializeField] Sprite[][] truckComponentsSprites;
    private int currentVehicleIndex = 0;

    public Vehicle.UpgradableComponent[][] upgradableComponents;
    private int currentUpgradableIndex = 0;
    private int upgradeIndex;

    private bool isInUpgradingUI = false;

    private GameObject vehicleCanvas;
    private GameObject upgradeCanvas;
    private GameObject selectButton;
    //private TextMeshProUGUI selectButtonText;
    private GameObject upgradeButton;
    private TextMeshProUGUI upgradeButtonText;
    //private TextMeshProUGUI displayedVehicleProperties;
    private GameObject locked;
    private Image vehicleImage;
    private GameObject moneyBar;
    private GameObject AQIBar;
    private GameObject properties;

    private GameObject confirmWindow;

    private void Awake()
    {
        vehicleCanvas = GameObject.Find("Vehicle Selection Canvas");
        upgradeCanvas = GameObject.Find("Vehicle Upgrade Canvas");

        selectButton = GameObject.Find("Select Vehicle Button");
        //selectButtonText = GameObject.Find("Vehicle").GetComponent<TextMeshProUGUI>();

        upgradeButton = GameObject.Find("Upgrade Vehicle Button");
        upgradeButtonText = GameObject.Find("Upgrade").GetComponent<TextMeshProUGUI>();

        moneyBar = GameObject.Find("Money Bar");
        AQIBar = GameObject.Find("AQI Bar Frame");
        properties = GameObject.Find("Properties");

        //displayedVehicleProperties = GameObject.Find("Vehicle Properties").GetComponent<TextMeshProUGUI>();
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
        vehicleCanvas.SetActive(true);
        upgradeCanvas.SetActive(false);
        //Vector3 tempPos = selectButton.GetComponent<RectTransform>().anchoredPosition;

        VehicleType currentVehicleType = vehicles[currentVehicleIndex];
        Vehicle previewVehicle = GetVehicleProperties(currentVehicleType);
        if (vehicleIsUnlocked[currentVehicleIndex])
        {
            locked.SetActive(false);
            AQIBar.SetActive(true);
            AQIBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 192);
            AQIBar.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
            moneyBar.GetComponent<RectTransform>().anchoredPosition = new Vector3(-732, 289, 0);
            properties.SetActive(true);
            selectButton.SetActive(true);
            vehicleImage.sprite = unlockedSprites[currentVehicleIndex];

            Image speed = GameObject.Find("Speed").GetComponent<Image>();
            Image mpg = GameObject.Find("MPG").GetComponent<Image>();
            Image fuel = GameObject.Find("Fuel").GetComponent<Image>();
            Image capacity = GameObject.Find("Package Capacity").GetComponent<Image>();

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
            selectButton.SetActive(false);
            moneyBar.GetComponent<RectTransform>().anchoredPosition = new Vector3(-285, 289, 0);
            AQIBar.SetActive(false);
            locked.SetActive(false);
            properties.SetActive(false);

            vehicleImage.sprite = lockedSprites[currentVehicleIndex];
            upgradeButton.SetActive(false);
        }
        else
        {
            selectButton.SetActive(false);
            locked.SetActive(true);
            AQIBar.SetActive(true);
            AQIBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 192);
            AQIBar.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
            properties.SetActive(false);
            moneyBar.GetComponent<RectTransform>().anchoredPosition = new Vector3(-732, 289, 0);
            vehicleImage.sprite = lockedSprites[currentVehicleIndex];
            //displayedVehicleProperties.text = $"Vehicle: {currentVehicleType}\nSpeed: {previewVehicle.vehicleSpeed}\nMPG: {previewVehicle.vehicleMPG}\nFuel Capacity: {previewVehicle.vehicleFuel}\nPackage Capacity: {previewVehicle.vehicleCapacity}";
            //selectButtonText.text = $"Price: {vehiclePrices[currentVehicleIndex]}";

            //selectButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, tempPos.y, tempPos.z);
            upgradeButton.SetActive(false);

            //speed.fillAmount = previewVehicle.vehicleSpeed / 10f;
            //mpg.fillAmount = 1 - previewVehicle.vehicleMPG / 100f;
            //fuel.fillAmount = previewVehicle.vehicleFuel / 25f;
            //capacity.fillAmount = previewVehicle.vehicleCapacity / 10f;
        }
    }

    public void DisplayVehicleUpgradeComponent()
    {
        vehicleCanvas.SetActive(false);
        upgradeCanvas.SetActive(true);
        AQIBar.SetActive(true);

        moneyBar.GetComponent<RectTransform>().anchoredPosition = new Vector3(-825, 411, 0);
        AQIBar.GetComponent<RectTransform>().anchoredPosition = new Vector3(-340, -425, 0);
        AQIBar.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 4);

        Image currentVehicle = GameObject.Find("Current Vehicle").GetComponent<Image>();
        TextMeshProUGUI currentVehicleName = GameObject.Find("Current Vehicle Name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI currentComponent = GameObject.Find("Current Component").GetComponent<TextMeshProUGUI>();
        GameObject componentOptions = GameObject.Find("Component Options");

        VehicleType currentVehicleType = vehicles[currentVehicleIndex];

        if (currentVehicleType == VehicleType.Motorbike)
        {
            //upgradableComponents = new string[] { "Engine", "Wheel", "Exhaust System" };

            upgradableComponents = Motorbike.GetMotorbikeUpgradableComponent();
            currentVehicle.sprite = upgradingSprites[1];
            currentVehicleName.text = "Motorbike";

            if (upgradableComponents[currentUpgradableIndex][0].category == "Engine")
            {
                currentComponent.text = "Engine";
            }
            else if (upgradableComponents[currentUpgradableIndex][0].category == "Wheels")
            {
                currentComponent.text = "Wheels";
            }
            else if (upgradableComponents[currentUpgradableIndex][0].category == "Exhaust System")
            {
                currentComponent.text = "Exhaust System";
            }

            for (int i = 0; i < componentOptions.transform.childCount; i++)
            {
                Image icon = componentOptions.transform.GetChild(i).GetChild(0).GetComponent<Image>();
                TextMeshProUGUI name = componentOptions.transform.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI description = componentOptions.transform.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI button = componentOptions.transform.GetChild(i).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();

                //icon.sprite = motorbikeComponentsSprites[currentUpgradableIndex][i];
                name.text = upgradableComponents[currentUpgradableIndex][i].name;
                //description.text = upgradableComponents[currentUpgradableIndex][i].description;

                if (!upgradableComponents[currentUpgradableIndex][i].isUnlocked)
                {
                    button.text = "Unlock";
                }
                else if (!upgradableComponents[currentUpgradableIndex][i].isChosen)
                {
                    button.text = "Choose";
                }
                else
                {
                    button.text = "Chosen";
                }
            }
        }
        else if (currentVehicleType == VehicleType.Truck)
        {
            upgradableComponents = Truck.GetTruckUpgradableComponent();
            currentVehicle.sprite = upgradingSprites[2];
            currentVehicleName.text = "Truck";

            if (upgradableComponents[currentUpgradableIndex][0].category == "Engine")
            {
                currentComponent.text = "Engine";
            }
            else if (upgradableComponents[currentUpgradableIndex][0].category == "Wheels")
            {
                currentComponent.text = "Wheels";
            }
            else if (upgradableComponents[currentUpgradableIndex][0].category == "Battery")
            {
                currentComponent.text = "Battery";
            }
            else if (upgradableComponents[currentUpgradableIndex][0].category == "Exhaust System")
            {
                currentComponent.text = "Exhaust System";
            }

            for (int i = 0; i < componentOptions.transform.childCount; i++)
            {
                Image icon = componentOptions.transform.GetChild(i).GetChild(0).GetComponent<Image>();
                TextMeshProUGUI name = componentOptions.transform.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI description = componentOptions.transform.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI button = componentOptions.transform.GetChild(i).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();

                //icon.sprite = truckComponentsSprites[currentUpgradableIndex][i];
                name.text = upgradableComponents[currentUpgradableIndex][i].name;
                //description.text = upgradableComponents[currentUpgradableIndex][i].description;

                if (!upgradableComponents[currentUpgradableIndex][i].isUnlocked)
                {
                    button.text = "Unlock";
                }
                else if (!upgradableComponents[currentUpgradableIndex][i].isChosen)
                {
                    button.text = "Choose";
                }
                else
                {
                    button.text = "Chosen";
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

            //selectButtonText.text = "Back";
        }
        else
        {
            //ConfirmWindow($"Are you sure you want to upgrade {vehicles[currentVehicleIndex].ToString().ToLower()}?");
            //UpgradeVehicleComponent(GetVehicleProperties(vehicles[currentVehicleIndex]));
        }
    }

    public void ChooseComponent(int componentIndex)
    {
        Debug.Log("Choosing component");
        upgradeIndex = componentIndex;

        if (!upgradableComponents[currentUpgradableIndex][componentIndex].isUnlocked)
        {
            UpgradeVehicleComponent(GetVehicleProperties(vehicles[currentVehicleIndex]), componentIndex);
        }
        else
        {
            GetVehicleProperties(vehicles[currentVehicleIndex]).ChooseUpgradeComponent(upgradableComponents[currentUpgradableIndex][componentIndex].category, upgradableComponents[currentUpgradableIndex][componentIndex].name);
            DisplayVehicleUpgradeComponent();
        }
    }

    public void UpgradeVehicleComponent(Vehicle upgradedVehicle, int componentIndex)
    {
        upgradedVehicle.UnlockUpgradeComponent(upgradableComponents[currentUpgradableIndex][componentIndex].category, upgradableComponents[currentUpgradableIndex][componentIndex].name);
        upgradedVehicle.ChooseUpgradeComponent(upgradableComponents[currentUpgradableIndex][componentIndex].category, upgradableComponents[currentUpgradableIndex][componentIndex].name);

        if (vehicleCanvas.activeSelf)
        {
            Debug.Log("Vehicle canvas is active before refreshing");
        }
        else
        {
            Debug.Log("Vehicle canvas is not active before refreshing");
        }
        /*for (int i = 0; i < upgradableComponents[currentUpgradableIndex].Length - 1; i++)
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
        }*/

        DisplayVehicleUpgradeComponent();
        Debug.Log("Upgraded vehicle component");
        if (vehicleCanvas.activeSelf)
        {
            Debug.Log("Vehicle canvas is active after refreshing");
        }
        else
        {
            Debug.Log("Vehicle canvas is not active after refreshing");
        }
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
                ConfirmWindow($"Are you sure you want to buy this vehicle?");
            }
            else
            {
                ConfirmWindow($"You don't have enough money!");
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
        upgradeCanvas.SetActive(false);
        confirmWindow.SetActive(true);
        AQIBar.SetActive(false);
        moneyBar.GetComponent<RectTransform>().anchoredPosition = new Vector3(-825, 411, 0);

        TextMeshProUGUI confirmMessage = GameObject.Find("Are you sure?").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI price = GameObject.Find("Price").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI name = GameObject.Find("Name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI nameShadow = GameObject.Find("Name Shadow").GetComponent<TextMeshProUGUI>();
        Image vehicle = GameObject.Find("Vehicle Confirmation").GetComponent<Image>();

        confirmMessage.text = message;
        if (!isInUpgradingUI)
        {
            price.text = $"{vehiclePrices[currentVehicleIndex]}";
            name.text = $"{vehicles[currentVehicleIndex]}";
            nameShadow.text = $"{vehicles[currentVehicleIndex]}";
            vehicle.sprite = upgradingSprites[currentVehicleIndex];
        }
        else
        {
            price.text = $"{upgradableComponents[currentUpgradableIndex][upgradeIndex].price}";
            name.text = $"{upgradableComponents[currentUpgradableIndex][upgradeIndex].name}";
            nameShadow.text = $"{upgradableComponents[currentUpgradableIndex][upgradeIndex].name}";
            vehicle.sprite = upgradingSprites[currentVehicleIndex];
        }
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
            else
            {
                vehicleCanvas.SetActive(true);
            }
        }
        else
        {
            //vehicleCanvas.SetActive(true);
            //UpgradeVehicleComponent(GetVehicleProperties(vehicles[currentVehicleIndex]));
        }

        confirmWindow.SetActive(false);
    }

    public void Cancel()
    {
        vehicleCanvas.SetActive(true);
        confirmWindow.SetActive(false);
    }
}
