using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PicknDrop : MonoBehaviour
{
    private GameObject pickUpLocation;
    private GameObject dropOffLocation;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        pickUpLocation = gameObject.transform.GetChild(0).gameObject;
        dropOffLocation = GameObject.Find("DropOffLocation");
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && Vector3.Distance(player.transform.position, pickUpLocation.transform.position) <= 2.5f)
        {
            Debug.Log("Picking up");
            StartCoroutine(PickingUp(pickUpLocation, player));
        }

        if (Input.GetKeyDown(KeyCode.F) && Vector3.Distance(player.transform.position, dropOffLocation.transform.position) <= 2.5f)
        {
            StartCoroutine(DroppingOff(dropOffLocation, player));
        }
    }

    IEnumerator PickingUp(GameObject PickupLocation, GameObject player)
    {
        float pickingUp = 0.0f;

        while (Vector3.Distance(player.transform.position, PickupLocation.transform.position) <= 2.5f)
        {
            pickingUp += Time.deltaTime;
            if (pickingUp >= 2.0f)
            {
                Debug.Log("Picked up");
                gameObject.transform.SetParent(player.gameObject.transform);
                transform.localPosition = new Vector3(1, 1, 0);
                break;
            }
        }

        yield return null;
    }

    IEnumerator DroppingOff(GameObject DropoffLocation, GameObject player)
    {
        float droppingOff = 0.0f;

        while (Vector3.Distance(player.transform.position, DropoffLocation.transform.position) <= 3f)
        {
            droppingOff += Time.deltaTime;
            if (droppingOff >= 2.0f)
            {
                Debug.Log("Dropped off");
                gameObject.transform.SetParent(null);
                transform.localPosition = DropoffLocation.transform.position;
                break;
            }
        }

        yield return null;
    }
}

/*public class PickUpDropOffCollider : MonoBehaviour
{
    public Transform holdPoint; // The point where the object will be held
    private GameObject heldObject;
    private bool isHolding = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && heldObject != null)
        {
            if (!isHolding)
            {
                PickUpObject();
            }
            else
            {
                DropObject();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickupObject") && !isHolding) // Make sure your object has the tag "PickupObject"
        {
            heldObject = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == heldObject)
        {
            heldObject = null;
        }
    }

    void PickUpObject()
    {
        heldObject.GetComponent<Rigidbody>().useGravity = false;
        heldObject.GetComponent<Rigidbody>().isKinematic = true;
        heldObject.transform.position = holdPoint.position;
        heldObject.transform.parent = holdPoint;
        isHolding = true;
    }

    void DropObject()
    {
        heldObject.GetComponent<Rigidbody>().useGravity = true;
        heldObject.GetComponent<Rigidbody>().isKinematic = false;
        heldObject.transform.parent = null;
        heldObject = null;
        isHolding = false;
    }
}*/

