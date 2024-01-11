using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyUI : MonoBehaviour
{
    private Image energyBar;

    // Start is called before the first frame update
    void Start()
    {
        energyBar = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        energyBar.fillAmount = PathFinding.energy / VehicleManager.playerVehicle.vehicleFuel;
    }
}
