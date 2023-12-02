using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
    private Rigidbody player;
    
    public GameObject direction;

    // Start is called before the first frame update
    void Start()
    {
        direction = GameObject.Find("Direction");
        player = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        player.velocity = (direction.transform.position - transform.position) * 2;
    }
       
}
