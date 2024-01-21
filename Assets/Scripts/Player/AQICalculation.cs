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

    private bool isChangingColor = false;
    private bool isPlayingWarning = false;

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
            if (realAQI_Index >= 0 && realAQI_Index < 1250)
            {
                StartCoroutine(UpdateEnvironmentLights(HexToColor("82cd7b"), 0.1f)); //green
            }
            else if (realAQI_Index >= 1250 && realAQI_Index < 2500)
            {
                StartCoroutine(UpdateEnvironmentLights(HexToColor("e4e781"), 0.1f)); //yellow
            }
            else if (realAQI_Index >= 2500 && realAQI_Index < 3750)
            {
                StartCoroutine(UpdateEnvironmentLights(HexToColor("f48a65"), 0.1f)); //orange
            }
            else if (realAQI_Index >= 3750 && realAQI_Index < 5000)
            {
                StartCoroutine(UpdateEnvironmentLights(HexToColor("cd7b82"), 0.1f)); //light red
            }
            else if (realAQI_Index >= 5000 && realAQI_Index < 6250)
            {
                StartCoroutine(UpdateEnvironmentLights(HexToColor("983abc"), 0.1f)); //purple
            }
            else if (realAQI_Index >= 6250)
            {
                StartCoroutine(UpdateEnvironmentLights(HexToColor("b0414a"), 0.1f)); //dark red
            }
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

        if (realAQI_Index >= 1249f && realAQI_Index <= 1250f && !isChangingColor)
        {
            StartCoroutine(UpdateEnvironmentLights(HexToColor("82cd7b"), 5f)); //green
        }
        else if (((realAQI_Index > 1250f && realAQI_Index <= 1251f) || (realAQI_Index >= 2499f&& realAQI_Index <= 2500f)) && !isChangingColor) 
        {
            StartCoroutine(UpdateEnvironmentLights(HexToColor("e4e781"), 5f)); //yellow
        }
        else if (((realAQI_Index > 2500f && realAQI_Index <= 2501f) || (realAQI_Index >= 3749f&& realAQI_Index <= 3750f)) && !isChangingColor)
        {
            StartCoroutine(UpdateEnvironmentLights(HexToColor("f48a65"), 5f)); //orange
        }
        else if (((realAQI_Index > 3750f && realAQI_Index <= 3751f) || (realAQI_Index >= 4999f && realAQI_Index <= 5000f)) && !isChangingColor)
        {
            StartCoroutine(UpdateEnvironmentLights(HexToColor("cd7b82"), 5f)); //light red
        }
        else if (((realAQI_Index > 5000f && realAQI_Index <= 5001f) || (realAQI_Index >= 6249f && realAQI_Index <= 6250f)) && !isChangingColor)
        {
            StartCoroutine(UpdateEnvironmentLights(HexToColor("983abc"), 5f)); //purple
        }
        else if ((realAQI_Index > 6250f && realAQI_Index <= 6251f) && !isChangingColor)
        {
            StartCoroutine(UpdateEnvironmentLights(HexToColor("b0414a"), 5f)); //dark red
        }

        if (!isPlayingWarning && realAQI_Index >= 6250f)
        {
            StartCoroutine(WarningAQI());
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

    public IEnumerator UpdateEnvironmentLights(Color targetColor, float duration)
    {
        GameObject environmentLights = GameObject.Find("Environment Lights");
        float time = 0f;

        for (int i = 0; i < environmentLights.transform.childCount; i++)
        {
            Light light = environmentLights.transform.GetChild(i).gameObject.GetComponent<Light>();
            Color startColor = light.color;

            if (startColor == targetColor)
            {
                break;
            }

            while (time < duration)
            {
                isChangingColor = true;
                light.color = Color.Lerp(startColor, targetColor, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            isChangingColor = false;
            light.color = targetColor;
            time = 0f;
            Debug.Log($"Updated Environment Lights {i}");
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

    public IEnumerator WarningAQI()
    {
        isPlayingWarning = true;
        AudioManager.audioManager.PlayAQIWarningSound();
        yield return new WaitForSeconds(1f);
        isPlayingWarning = false;
    }
}
