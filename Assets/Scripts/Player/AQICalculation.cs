using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AQICalculation : MonoBehaviour
{
    private float currentMPG;
    private float startingAQI;

    private RectTransform AQIBar;
    private RectTransform baseBar;
    public static float realAQI_Index = 5500f;
    public float timeAQIStays;
    private float duration = 2f;

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
            StartCoroutine(UpdateEnvironmentLights(HexToColor("983abc")));
        }

        startingAQI = realAQI_Index;
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

        if (realAQI_Index == 1250f)
        {
            StartCoroutine(UpdateEnvironmentLights(HexToColor("82cd7b"))); //green
        }
        else if (realAQI_Index == 1251f || realAQI_Index == 2500f) 
        {
            StartCoroutine(UpdateEnvironmentLights(HexToColor("e4e781"))); //yellow
        }
        else if (realAQI_Index == 2501f || realAQI_Index == 3750f)
        {
            StartCoroutine(UpdateEnvironmentLights(HexToColor("48a65"))); //orange
        }
        else if (realAQI_Index == 3751f || realAQI_Index == 5000f)
        {
            StartCoroutine(UpdateEnvironmentLights(HexToColor("cd7b82"))); //light red
        }
        else if (realAQI_Index == 5001f || realAQI_Index == 6250f)
        {
            StartCoroutine(UpdateEnvironmentLights(HexToColor("983abc"))); //purple
        }
        else if (realAQI_Index == 6251f || realAQI_Index == 7500f)
        {
            StartCoroutine(UpdateEnvironmentLights(HexToColor("b0414A"))); //dark red
        }

        if (realAQI_Index >= 7000f)
        {
            //Lose game
        }
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

    public void EndDayCheck()
    {
        if (realAQI_Index < startingAQI && !(realAQI_Index <= 0))
        {
            MoneyManager.money += Mathf.RoundToInt((startingAQI - realAQI_Index) * 0.1f);
        }
    }

    public IEnumerator UpdateEnvironmentLights(Color targetColor)
    {
        GameObject environmentLights = GameObject.Find("Environment Lights");
        float time = 0f;

        for (int i = 0; i < environmentLights.transform.childCount; i++)
        {
            Light light = environmentLights.transform.GetChild(i).gameObject.GetComponent<Light>();
            Color startColor = light.color;

            while (time < duration)
            {
                light.color = Color.Lerp(startColor, targetColor, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            light.color = targetColor;
        }
    }

    public Color HexToColor(string hex)
    {
        byte a = 255;
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        // Check if alpha value is provided
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }

        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }
}
