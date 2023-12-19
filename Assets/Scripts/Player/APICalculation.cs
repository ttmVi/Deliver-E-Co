using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class APICalculation : MonoBehaviour
{
    private float currentMPG;
    private Slider APISlider;
    public float realAPI_Index;
    
    private Rigidbody player;


    // Start is called before the first frame update
    void Start()
    {
        APISlider = GameObject.Find("API Slider").GetComponent<Slider>();
        player = GameObject.Find("Direction").GetComponent<Rigidbody>();
        APISlider.highValue = 1000f;
    }

    // Update is called once per frame
    void Update()
    {
        currentMPG = VehicleManager.playerVehicle.vehicleMPG;

        APISlider.value = CalculateAPI();
    }

    public float CalculateAPI()
    {
        if (SceneManager.GetActiveScene().name == "Main Moving Scene")
        {
            if (player.velocity != Vector3.zero)
            {
                realAPI_Index += player.velocity.magnitude * currentMPG;
            }
        }
        else if (SceneManager.GetActiveScene().name == "Vehicle Customize")
        {
            realAPI_Index = 50f;
        }

        return realAPI_Index;
    }
}
