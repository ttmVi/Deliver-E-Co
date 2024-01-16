using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AQICalculation : MonoBehaviour
{
    private float currentMPG;

    private Image AQIBar;
    public static float realAQI_Index;
    public float timeAQIStays;

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

        AQIBar.fillAmount = realAQI_Index / 7500f;
    }

    public float CalculateAQI()
    {
        if (vel.velDirection != Vector3.zero && currentMPG != 100f)
        {
            realAQI_Index += (1000f / currentMPG) * Time.deltaTime;
            timeAQIStays = 0f;
        }
        else if (SceneManager.GetActiveScene().name != "Vehicle Customize") //reduce AQI if it doesn't increase for 20 seconds
        {
            timeAQIStays += Time.deltaTime;
            if (timeAQIStays >= 20f && realAQI_Index > 0)
            {
                if (realAQI_Index > 0f)
                {
                    realAQI_Index -= 100f * Time.deltaTime;
                }
                else { realAQI_Index = 0f; }
            }
        }

        return realAQI_Index;
    }
}
