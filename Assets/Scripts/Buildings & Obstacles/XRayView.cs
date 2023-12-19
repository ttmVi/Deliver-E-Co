using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class XRayView : MonoBehaviour
{
    public List<Material> originalMaterial;
    public List<Material> transparentMaterial;
    private Renderer objRenderer;

    public GameObject player;
    public GameObject cam;
    public GameObject[] buildings;
    public GameObject[] buildingsInWay;

    void Start()
    {
        // Get the Renderer component from the object
        objRenderer = GetComponent<MeshRenderer>();

        player = GameObject.Find("Player");
        cam = GameObject.Find("Main Camera");

        buildings = GameObject.FindGameObjectsWithTag("Buildings");
    }
    void Update()
    {
        RaycastHit[] hits;
        Vector3 direction = player.transform.position - cam.transform.position;
        float distance = Vector3.Distance(cam.transform.position, player.transform.position);

        hits = Physics.RaycastAll(cam.transform.position, direction, distance);
        buildingsInWay = hits.Select(hit => hit.collider.gameObject).ToArray();

        foreach (GameObject building in buildings)
        {
            if (!buildingsInWay.Contains(building))
            {
                building.GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                building.GetComponent<MeshRenderer>().enabled = false;
            }

            /*for (int i = 0; i < buildings.Length; i++)
            {
                //Color color = buildings[i].GetComponent<MeshRenderer>().material.color;

                if (hit.collider.gameObject == buildings[i])
                {
                    buildings[i].GetComponent<MeshRenderer>().enabled = false;
                    Debug.Log($"Transparent set for {buildings[i]}");

                    break;

                    for (int j = 0; j < buildings[i].GetComponent<MeshRenderer>().materials.Length; j++)
                    {
                        for (int k = 0; k < originalMaterial.Count; k++)
                        {
                            if (buildings[i].GetComponent<MeshRenderer>().materials[j].name.Contains(originalMaterial[k].name))
                            {
                                buildings[i].GetComponent<MeshRenderer>().materials[j] = transparentMaterial[k];
                            }
                        }
                    }
                }
                else if (hit.collider.gameObject != buildings[i])
                {
                    buildings[i].GetComponent<MeshRenderer>().enabled = true;
                    Debug.Log("Original set");
                    for (int j = 0; j < buildings[i].GetComponent<MeshRenderer>().materials.Length; j++)
                    {
                        for (int k = 0; k < transparentMaterial.Count; k++)
                        {
                            if (buildings[i].GetComponent<MeshRenderer>().materials[j].name.Contains(transparentMaterial[k].name))
                            {
                                buildings[i].GetComponent<MeshRenderer>().materials[j] = originalMaterial[k];
                            }
                        }
                    }
                }
            }*/
        }
    }

    // Method to enable X-Ray view
    public void EnableXRayView()
    {
        //objRenderer.material = transparentMaterial;
    }

    // Method to disable X-Ray view
    public void DisableXRayView()
    {
        //objRenderer.material = originalMaterial;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(cam.transform.position, player.transform.position);
    }
}