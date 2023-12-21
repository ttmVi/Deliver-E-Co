using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static VehicleFactory;

public class VehicleManager : MonoBehaviour
{
    public GameSceneManager gameSceneManager;

    public static VehicleManager vehicleManager;
    public static Vehicle playerVehicle;
    public VehicleType[] vehicles = { VehicleType.Bicycle, VehicleType.Motorbike, VehicleType.Updating };
    public bool[] vehicleIsUnlocked = { false, false, false };
    public int[] vehiclePrices = { 0, 100, 0 };
    private int currentVehicleIndex = 0;

    private Canvas vehicleCanvas;
    private TextMeshProUGUI selectButton;
    private TextMeshProUGUI displayedVehicleProperties;

    private void Awake()
    {
        vehicleCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        selectButton = GameObject.Find("Vehicle").GetComponent<TextMeshProUGUI>();
        displayedVehicleProperties = GameObject.Find("Vehicle Properties").GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        DisplayVehicleProperties(GetVehicleProperties(vehicles[currentVehicleIndex]));
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Return))
        // {
        //     StartLevelWithVehicleProperties(VehicleFactory.VehicleType."some kind of input");
        // }
    }

    public void StartLevelWithVehicleProperties(VehicleType vehicleSelected)
    {
        playerVehicle = GetVehicleProperties(vehicleSelected);
        playerVehicle.StartVehicle();
    }

    public void NextVehicle()
    {
        currentVehicleIndex = (currentVehicleIndex + 1) % vehicles.Length;
        DisplayVehicleProperties(GetVehicleProperties(vehicles[currentVehicleIndex]));
    }

    public void PreviousVehicle()
    {
        if (currentVehicleIndex == 0)
        {
            currentVehicleIndex = vehicles.Length - 1;
        }
        else
        {
            currentVehicleIndex--;
        }
        DisplayVehicleProperties(GetVehicleProperties(vehicles[currentVehicleIndex]));
    }

    public void DisplayVehicleProperties(Vehicle vehicle)
    {
        VehicleType currentVehicleType = vehicles[currentVehicleIndex];
        Vehicle previewVehicle = GetVehicleProperties(currentVehicleType);
        if (vehicleIsUnlocked[currentVehicleIndex])
        {
            displayedVehicleProperties.text = $"Vehicle: {currentVehicleType}\nSpeed: {previewVehicle.vehicleSpeed}\nMPG: {previewVehicle.vehicleMPG}\nFuel Capacity: {previewVehicle.vehicleFuel}\nPackage Capacity: {previewVehicle.vehicleCapacity}";
            selectButton.text = "Start";
        }
        else if (currentVehicleType == VehicleType.Updating)
        {
            displayedVehicleProperties.text = "Coming soon...";
            selectButton.text = "Updating";
        }
        else
        {
            displayedVehicleProperties.text = $"Vehicle: {currentVehicleType}\nSpeed: {previewVehicle.vehicleSpeed}\nMPG: {previewVehicle.vehicleMPG}\nFuel Capacity: {previewVehicle.vehicleFuel}\nPackage Capacity: {previewVehicle.vehicleCapacity}";
            selectButton.text = $"Price: {vehiclePrices[currentVehicleIndex]}";
        }
    }

    public void SelectVehicle()
    {
        if (vehicleIsUnlocked[currentVehicleIndex])
        {
            StartLevelWithVehicleProperties(vehicles[currentVehicleIndex]);
        }
        else
        {
            UnlockVehicle();
        }
    }

    public void UnlockVehicle()
    {
        if (MoneyManager.money >= vehiclePrices[currentVehicleIndex] && vehicles[currentVehicleIndex] != VehicleType.Updating)
        {
            MoneyManager.money -= vehiclePrices[currentVehicleIndex];
            vehicleIsUnlocked[currentVehicleIndex] = true;
            DisplayVehicleProperties(GetVehicleProperties(vehicles[currentVehicleIndex]));
        }
    }
}
