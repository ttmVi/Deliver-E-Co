using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliding : MonoBehaviour
{
    private GameObject playerCollider;
    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GameObject.Find("Player & Camera (Intermediate Parent)");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerCollider)
        {
            Debug.Log("Gotohell");
            Destroy(other.gameObject);
        }
    }
}
