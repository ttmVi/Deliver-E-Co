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
        objRenderer = GetComponent<Renderer>();
        // Assume the object starts with its original material
        objRenderer.material = originalMaterial;

        player = GameObject.Find("Player");
        cam = GameObject.Find("Main Camera");
    }

    void Update()
    {
        RaycastHit hit;
        Vector3 direction = player.transform.position - cam.transform.position;

        if (Physics.Raycast(cam.transform.position, direction, out hit))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                EnableXRayView();
            }
            else
            {
                DisableXRayView();
            }
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
}

