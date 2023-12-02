using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocationUpdate : MonoBehaviour
{
    private GameObject player;
    private GameObject map3D;
    private GameObject map2D;

    public Vector3 realPos;
    public Vector2 localPos;
    public Vector3 worldPos;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        map3D = GameObject.Find("Map 3D");
        map2D = GameObject.Find("Map");

        Debug.Log(map2D.GetComponent<RectTransform>().sizeDelta.x);
        Debug.Log(map2D.GetComponent<RectTransform>().sizeDelta.y);
    }

    // Update is called once per frame
    void Update()
    {
        worldPos = player.transform.position - (map3D.transform.position + map3D.GetComponent<BoxCollider>().center);
        localPos.x = worldPos.x / (map3D.GetComponent<BoxCollider>().size.x);
        localPos.y = worldPos.z / (map3D.GetComponent<BoxCollider>().size.y);
        // Get position ratio

        realPos.x = localPos.x * (map2D.GetComponent<RectTransform>().sizeDelta.x);
        realPos.y = localPos.y * (map2D.GetComponent<RectTransform>().sizeDelta.y);
        // Normalized position to 2d map coordinates

        RectTransform playerIconRectTransform = GetComponent<RectTransform>();
        playerIconRectTransform.anchoredPosition = new Vector3(realPos.x, realPos.y, 13.6f);
    }
}
