using UnityEngine;

public class PlayerLocationUpdate : MonoBehaviour
{
    public GameObject player;
    public GameObject map3D;
    public RectTransform map2D;

    private BoxCollider map3DCollider;

    void Start()
    {
        player = GameObject.Find("Player");
        map3D = GameObject.Find("Map 3D");
        map2D = GameObject.Find("Map").GetComponent<RectTransform>();

        // Check if the necessary GameObjects are assigned
        if (!player)
        {
            Debug.LogError("Player GameObject is not assigned.");
            return;
        }

        if (!map3D)
        {
            Debug.LogError("3D Map GameObject is not assigned.");
            return;
        }

        if (!map2D)
        {
            Debug.LogError("2D Map RectTransform is not assigned.");
            return;
        }

        map3DCollider = map3D.GetComponent<BoxCollider>();
        if (!map3DCollider)
        {
            Debug.LogError("BoxCollider component missing on the 3D map GameObject.");
            return;
        }
    }

    void Update()
    {
        if (!player || !map3DCollider || !map2D)
        {
            return; // Essential components are not set
        }

        // Calculate the player's position relative to the 3D map's bounds
        Vector3 relativePosition = player.transform.position - map3D.transform.position;
        Debug.Log("Relative position (before center adjustment): " + relativePosition);

        relativePosition.x += map3DCollider.center.x * map3D.transform.localScale.x;
        relativePosition.z += map3DCollider.center.y * map3D.transform.localScale.y;
        Debug.Log("Relative position (after center adjustment): " + relativePosition);

        // Normalize the position to a range of [0, 1]
        Vector2 normalizedPos;
        normalizedPos.x = (relativePosition.x / (map3DCollider.size.x * map3D.transform.localScale.x)) + 0.5f;
        normalizedPos.y = (relativePosition.z / (map3DCollider.size.y * map3D.transform.localScale.y)) + 0.5f;

        // Clamp values to ensure they are within [0, 1]
        normalizedPos.x = Mathf.Clamp(normalizedPos.x, 0, 1);
        normalizedPos.y = Mathf.Clamp(normalizedPos.y, 0, 1);
        Debug.Log("Normalized position: " + normalizedPos);

        // Convert normalized position to 2D map coordinates
        Vector2 mapPosition;
        mapPosition.x = normalizedPos.x * map2D.sizeDelta.x - (map2D.sizeDelta.x * 0.5f);
        mapPosition.y = normalizedPos.y * map2D.sizeDelta.y - (map2D.sizeDelta.y * 0.5f);

        // Update player icon's position on the 2D map
        RectTransform playerIconRectTransform = GetComponent<RectTransform>();
        if (playerIconRectTransform)
        {
            playerIconRectTransform.anchoredPosition = -mapPosition;
            Debug.Log("Updated player icon position: " + mapPosition);
        }
        else
        {
            Debug.LogError("Missing RectTransform on the player icon.");
        }

        // Debugging player movement
        Debug.Log("Player world position: " + player.transform.position);
    }
}
