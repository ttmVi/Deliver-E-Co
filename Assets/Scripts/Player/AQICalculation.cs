using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AQICalculation : MonoBehaviour
{
    private float currentMPG;

    private RectTransform AQIBar;
    private RectTransform baseBar;
    public static float realAQI_Index = 5750f;
    public float timeAQIStays;

    [SerializeField] PathFinding vel;

    // Start is called before the first frame update
    void Start()
    {
        AQIBar = GameObject.Find("AQI Index").GetComponent<RectTransform>();
        baseBar = GameObject.Find("Base Bar").GetComponent<RectTransform>();

        if (SceneManager.GetActiveScene().name == "Vehicle Customize")
        {
            baseBar.gameObject.transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 192);
        }
        else if (SceneManager.GetActiveScene().name == "Main Moving Scene")
        {
            baseBar.gameObject.transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 130);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (VehicleManager.playerVehicle != null)
        {
            currentMPG = VehicleManager.playerVehicle.vehicleMPG;

            realAQI_Index = CalculateAQI();
        }

        UpdateAQIBar();
        //AQIBar.fillAmount = realAQI_Index / 7500f;
    }

    public float CalculateAQI()
    {
        if (PathFinding.isMoving && currentMPG != 100f)
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
                    realAQI_Index -= 20f * Time.deltaTime;
                }
                else { realAQI_Index = 0f; }
            }
        }

        return realAQI_Index;
    }

    public void UpdateAQIBar()
    {
        float fillAmount = realAQI_Index / 7500f;

        //Setting arrow's local position
        float xArrow = fillAmount * baseBar.sizeDelta.x - baseBar.sizeDelta.x / 2f;
        AQIBar.anchoredPosition = new Vector2(xArrow, AQIBar.anchoredPosition.y);
    }
}
