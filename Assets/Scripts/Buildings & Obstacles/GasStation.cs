using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GasStation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (RefillingEnergy(GetComponent<BoxCollider>()))
            {
                PathFinding.RefillEnergy();
                Debug.Log("Refilling energy");
            }
        }
    }
    public bool RefillingEnergy(BoxCollider location)
    {
        bool isRefilling = false;
        Vector3 triggerArea = new Vector3(location.size.x * location.transform.localScale.x + 2f * 2, location.size.y * location.transform.localScale.y + 2f * 2, location.size.z * location.transform.localScale.z + 2f * 2);
        Collider[] shipper = Physics.OverlapBox(location.transform.position, triggerArea / 2, Quaternion.identity, LayerMask.GetMask("Player"));

        if (shipper.Length > 0)
        {
            Debug.Log("Player detected");
            for (int i = 0; i < shipper.Length; i++)
            {
                if (shipper[i].gameObject.name == "Player")
                {
                    isRefilling = true;
                }
            }
        }
        else
        {
            isRefilling = false;
            Debug.Log("Player not detected");
        }

        Debug.Log("Finish checking for player");
        return isRefilling;
    }

}
