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

    private AudioSource audioSource;
    [SerializeField] AudioClip buyingSound;
    [SerializeField] AudioClip upgradingSound;
    [SerializeField] AudioClip selectingSound;
    [SerializeField] AudioClip upgradeSwitchingSound;
    [SerializeField] AudioClip vehicleSwitchingSound;
    [SerializeField] AudioClip clickSound;
    [SerializeField] AudioClip otherClickingSound;

    public VehicleType[] vehicles = { VehicleType.Bicycle, VehicleType.Motorbike, VehicleType.Truck, VehicleType.Updating };
    public static bool[] vehicleIsUnlocked = { false, false, false, false };
    public int[] vehiclePrices = { 0, 100, 1000, 0 };
    [SerializeField] Sprite[] lockedSprites;
    [SerializeField] Sprite[] unlockedSprites;
    [SerializeField] Sprite[] upgradingSprites;
    [SerializeField] Sprite[] motorbikeComponentsSprites;
    [SerializeField] Sprite[] truckComponentsSprites;
    [SerializeField] Sprite[] componentsStatusSprites;
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
        audioSource = GetComponent<AudioSource>();

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
        audioSource.clip = otherClickingSound;
        audioSource.Play();
        playerVehicle = GetVehicleProperties(vehicleSelected);
    }

    public void Next()
    {
        if (vehicleIsUnlocked[currentVehicleIndex] && isInUpgradingUI)
        {
            if (currentUpgradableIndex != upgradableComponents.Length - 1)
            {
                currentUpgradableIndex++;
                audioSource.clip = upgradeSwitchingSound;
                audioSource.Play();
            }
            DisplayVehicleUpgradeComponent();
        }
        else
        {
            currentVehicleIndex = (currentVehicleIndex + 1) % vehicles.Length;
            DisplayVehicleProperties();
            audioSource.clip = vehicleSwitchingSound;
            audioSource.Play();
        }
    }

    public void Previous()
    {
        if (vehicleIsUnlocked[currentVehicleIndex] && isInUpgradingUI)
        {
            if (currentUpgradableIndex != 0)
            {
                currentUpgradableIndex--;
                audioSource.clip = upgradeSwitchingSound;
                audioSource.Play();
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
            audioSource.clip = vehicleSwitchingSound;
            audioSource.Play();
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
            vehicleImage.SetNativeSize();

            Image speed = GameObject.Find("Speed").GetComponent<Image>();
            Image mpg = GameObject.Find("MPG").GetComponent<Image>();
            Image fuel = GameObject.Find("Fuel").GetComponent<Image>();
            Image capacity = GameObject.Find("Package Capacity").GetComponent<Image>();

            if (currentVehicleType != VehicleType.Bicycle)
            {
                if (currentVehicleType == VehicleType.Motorbike)
                {
                    upgradableComponents = Motorbike.GetMotorbikeUpgradableComponent();
                }
                else if (currentVehicleType == VehicleType.Truck)
                {
                    upgradableComponents = Truck.GetTruckUpgradableComponent();
                }
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
            vehicleImage.SetNativeSize();
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
            vehicleImage.SetNativeSize();
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

            currentVehicle.sprite = upgradingSprites[1];
            currentVehicle.SetNativeSize();
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
                Image componentStatus = componentOptions.transform.GetChild(i).GetChild(3).GetComponent<Image>();
                TextMeshProUGUI button = componentOptions.transform.GetChild(i).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();

                icon.sprite = motorbikeComponentsSprites[currentUpgradableIndex * upgradableComponents[currentUpgradableIndex].Length + i];
                icon.SetNativeSize();
                name.text = upgradableComponents[currentUpgradableIndex][i].name;
                description.text = upgradableComponents[currentUpgradableIndex][i].description;

                if (!upgradableComponents[currentUpgradableIndex][i].isUnlocked)
                {
                    componentStatus.sprite = componentsStatusSprites[0];
                    componentStatus.SetNativeSize();
                    button.text = $"{upgradableComponents[currentUpgradableIndex][i].price}";
                }
                else if (!upgradableComponents[currentUpgradableIndex][i].isChosen)
                {
                    componentStatus.sprite = componentsStatusSprites[1];
                    componentStatus.SetNativeSize();
                    button.text = "Equip";
                }
                else
                {
                    componentStatus.sprite = componentsStatusSprites[2];
                    componentStatus.SetNativeSize();
                    button.text = "";
                }
            }
        }
        else if (currentVehicleType == VehicleType.Truck)
        {
            //upgradableComponents = Truck.GetTruckUpgradableComponent();
            currentVehicle.sprite = upgradingSprites[2];
            currentVehicle.SetNativeSize();
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
                Image componentStatus = componentOptions.transform.GetChild(i).GetChild(3).GetComponent<Image>();
                TextMeshProUGUI button = componentOptions.transform.GetChild(i).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();

                icon.sprite = truckComponentsSprites[currentUpgradableIndex * upgradableComponents[currentUpgradableIndex].Length + i];
                icon.SetNativeSize();
                name.text = upgradableComponents[currentUpgradableIndex][i].name;
                description.text = upgradableComponents[currentUpgradableIndex][i].description;

                if (!upgradableComponents[currentUpgradableIndex][i].isUnlocked)
                {
                    componentStatus.sprite = componentsStatusSprites[0];
                    componentStatus.SetNativeSize();
                    button.text = $"{upgradableComponents[currentUpgradableIndex][i].price}";
                }
                else if (!upgradableComponents[currentUpgradableIndex][i].isChosen)
                {
                    componentStatus.sprite = componentsStatusSprites[1];
                    componentStatus.SetNativeSize();
                    button.text = "Equip";
                }
                else
                {
                    componentStatus.sprite = componentsStatusSprites[2];
                    componentStatus.SetNativeSize();
                    button.text = "";
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
            if (MoneyManager.money >= upgradableComponents[currentUpgradableIndex][componentIndex].price)
            {
                ConfirmWindow($"Are you sure you want to unlock this component?");
            }
            else
            {
                ConfirmWindow($"You don't have enough money!");
            }
            //UpgradeVehicleComponent(GetVehicleProperties(vehicles[currentVehicleIndex]), componentIndex);
        }
        else
        {
            GetVehicleProperties(vehicles[currentVehicleIndex]).ChooseUpgradeComponent(upgradableComponents[currentUpgradableIndex][componentIndex].category, upgradableComponents[currentUpgradableIndex][componentIndex].name);
            DisplayVehicleUpgradeComponent();

            audioSource.clip = selectingSound;
            audioSource.Play();
        }
    }

    public void UpgradeVehicleComponent(Vehicle upgradedVehicle, int componentIndex)
    {
        upgradedVehicle.UnlockUpgradeComponent(upgradableComponents[currentUpgradableIndex][componentIndex].category, upgradableComponents[currentUpgradableIndex][componentIndex].name);
        upgradedVehicle.ChooseUpgradeComponent(upgradableComponents[currentUpgradableIndex][componentIndex].category, upgradableComponents[currentUpgradableIndex][componentIndex].name);

        MoneyManager.money -= upgradableComponents[currentUpgradableIndex][componentIndex].price;

        DisplayVehicleUpgradeComponent();

        audioSource.clip = upgradingSound;
        audioSource.Play();
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
                audioSource.clip = otherClickingSound;
                audioSource.Play();
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

            audioSource.clip = buyingSound;
            audioSource.Play();
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
            vehicle.SetNativeSize();

            name.fontSize = 65;
            nameShadow.fontSize = 65;
        }
        else
        {
            price.text = $"{upgradableComponents[currentUpgradableIndex][upgradeIndex].price}";
            name.text = $"{upgradableComponents[currentUpgradableIndex][upgradeIndex].name}";
            nameShadow.text = $"{upgradableComponents[currentUpgradableIndex][upgradeIndex].name}";
            vehicle.sprite = upgradingSprites[currentVehicleIndex];
            vehicle.SetNativeSize();

            name.fontSize = 44;
            nameShadow.fontSize = 44;
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
            if (MoneyManager.money >= upgradableComponents[currentUpgradableIndex][upgradeIndex].price)
            {
                vehicleCanvas.SetActive(true);
                UpgradeVehicleComponent(GetVehicleProperties(vehicles[currentVehicleIndex]), upgradeIndex);
            }
            else
            {
                vehicleCanvas.SetActive(true);
            }
        }

        confirmWindow.SetActive(false);
    }

    public void Cancel()
    {
        audioSource.clip = otherClickingSound;
        audioSource.time = 0.725f;
        audioSource.Play();

        if (!isInUpgradingUI)
        {
            vehicleCanvas.SetActive(true);
            AQIBar.SetActive(true);
        }
        else
        {
            upgradeCanvas.SetActive(true);
            AQIBar.SetActive(true);
        }
        confirmWindow.SetActive(false);
    }
}
