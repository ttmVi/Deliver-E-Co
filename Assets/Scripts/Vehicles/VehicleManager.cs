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
    public static VehicleManager vehicleManager;
    public static Vehicle playerVehicle;
    public VehicleType[] availableVehicles = { VehicleType.Bicycle, VehicleType.Motorbike, VehicleType.Updating };
    private int currentVehicleIndex = 0;

    private Canvas vehicleCanvas;
    private TextMeshProUGUI displayedVehicleName;
    private TextMeshProUGUI displayedVehicleProperties;

    private void Awake()
    {
        vehicleCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        displayedVehicleName = GameObject.Find("Vehicle").GetComponent<TextMeshProUGUI>();
        displayedVehicleProperties = GameObject.Find("Vehicle Properties").GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        DisplayVehicleProperties(GetVehicleProperties(availableVehicles[currentVehicleIndex]));
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
        currentVehicleIndex = (currentVehicleIndex + 1) % availableVehicles.Length;
        DisplayVehicleProperties(GetVehicleProperties(availableVehicles[currentVehicleIndex]));
    }

    public void PreviousVehicle()
    {
        if (currentVehicleIndex == 0)
        {
            currentVehicleIndex = availableVehicles.Length - 1;
        }
        else
        {
            currentVehicleIndex--;
        }
        DisplayVehicleProperties(GetVehicleProperties(availableVehicles[currentVehicleIndex]));
    }

    public void DisplayVehicleProperties(Vehicle vehicle)
    {
        VehicleType currentVehicleType = availableVehicles[currentVehicleIndex];
        Vehicle previewVehicle = GetVehicleProperties(currentVehicleType);
        displayedVehicleName.text = currentVehicleType.ToString();
        if (currentVehicleType != VehicleType.Updating)
        {
            displayedVehicleProperties.text = $"Vehicle: {currentVehicleType}\nSpeed: {previewVehicle.vehicleSpeed}\nMPG: {previewVehicle.vehicleMPG}\nFuel Capacity: {previewVehicle.vehicleFuel}\nPackage Capacity: {previewVehicle.vehicleCapacity}";
        }
        else
        {
            displayedVehicleProperties.text = "Not yet unlocked";
        }
    }

    public void SelectVehicle()
    {
        if (availableVehicles[currentVehicleIndex] != VehicleType.Updating)
        {
            StartLevelWithVehicleProperties(availableVehicles[currentVehicleIndex]);
        }
    }
}
