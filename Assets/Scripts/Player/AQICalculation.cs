using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AQICalculation : MonoBehaviour
{
    private float currentMPG;
    private Image AQIBar;
    public float realAQI_Index;

    private PathFinding vel;
    private Rigidbody player;


    // Start is called before the first frame update
    void Start()
    {
        if (VehicleManager.playerVehicle != null)
        {
            vel = GameObject.Find("Direction").GetComponent<PathFinding>();
        }
        AQIBar = GameObject.Find("AQI Index").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (VehicleManager.playerVehicle != null)
        {
            currentMPG = VehicleManager.playerVehicle.vehicleMPG;

            realAQI_Index = CalculateAQI();
        }

        AQIBar.fillAmount = realAQI_Index / 1000f;
    }

    public float CalculateAQI()
    {
        if (vel.velDirection != Vector3.zero)
        {
            realAQI_Index += currentMPG * Time.deltaTime;
        }
        return realAQI_Index;
    }
}
