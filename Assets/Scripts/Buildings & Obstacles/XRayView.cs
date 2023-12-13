using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRayView : MonoBehaviour
{
    public Material originalMaterial;
    public Material transparentMaterial;
    private Renderer objRenderer;

    public GameObject player;
    public GameObject cam;

    void Start()
    {
        // Get the Renderer component from the object
        objRenderer = GetComponent<MeshRenderer>();

        originalMaterial = objRenderer.material;

        player = GameObject.Find("Player");
        cam = GameObject.Find("Main Camera");
    }

    void Update()
    {
        RaycastHit[] hits;
        Vector3 direction = player.transform.position - cam.transform.position;
        float distance = Vector3.Distance(cam.transform.position, player.transform.position);

        hits = Physics.RaycastAll(cam.transform.position, direction, distance);
        bool buildingInWay = false;

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                buildingInWay = true;
                break;
            }
        }

        if (buildingInWay)
        {
            EnableXRayView();
        }
        else
        {
            DisableXRayView();
        }
    }

    // Method to enable X-Ray view
    public void EnableXRayView()
    {
        objRenderer.material = transparentMaterial;
    }

    // Method to disable X-Ray view
    public void DisableXRayView()
    {
        objRenderer.material = originalMaterial;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(cam.transform.position, player.transform.position);
    }
}

