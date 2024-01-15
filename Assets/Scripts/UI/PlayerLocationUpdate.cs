using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLocationUpdate : MonoBehaviour
{
    private GameObject player;
    private GameObject map3D;
    private RectTransform map2D;

    private BoxCollider map3DCollider;

    private Animator animator;
    [SerializeField] RuntimeAnimatorController bicycle;
    [SerializeField] RuntimeAnimatorController motorbike;
    [SerializeField] RuntimeAnimatorController car;

    void Start()
    {
        player = GameObject.Find("Player");
        map3D = GameObject.Find("Map 3D");
        map2D = GameObject.Find("Map").GetComponent<RectTransform>();

        map3DCollider = map3D.GetComponent<BoxCollider>();

        animator = GetComponent<Animator>();

        if (VehicleManager.playerVehicle.vehicleCapacity <= 2)
        {
            animator.runtimeAnimatorController = bicycle;
        }
        else if (VehicleManager.playerVehicle.vehicleCapacity <= 4)
        {
            animator.runtimeAnimatorController = motorbike;
        }
        else if (VehicleManager.playerVehicle.vehicleCapacity <= 10)
        {
            animator.runtimeAnimatorController = car;
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
        //Debug.Log("Relative position (before center adjustment): " + relativePosition);

        relativePosition.x -= map3DCollider.center.x * map3D.transform.localScale.x;
        relativePosition.z -= map3DCollider.center.z * map3D.transform.localScale.z;
        //Debug.Log("Relative position (after center adjustment): " + relativePosition);

        // Normalize the position to a range of [0, 1]
        Vector2 normalizedPos;
        normalizedPos.x = (relativePosition.x / (map3DCollider.size.x * map3D.transform.localScale.x)) + 0.5f;
        normalizedPos.y = (relativePosition.z / (map3DCollider.size.z * map3D.transform.localScale.z)) + 0.5f;

        // Clamp values to ensure they are within [0, 1]
        normalizedPos.x = Mathf.Clamp(normalizedPos.x, 0, 1);
        normalizedPos.y = Mathf.Clamp(normalizedPos.y, 0, 1);
        //Debug.Log("Normalized position: " + normalizedPos);

        // Convert normalized position to 2D map coordinates
        Vector2 mapPosition;
        mapPosition.x = normalizedPos.x * map2D.sizeDelta.x - (map2D.sizeDelta.x * 0.5f) + map2D.anchoredPosition.x;
        mapPosition.y = normalizedPos.y * map2D.sizeDelta.y - (map2D.sizeDelta.y * 0.5f) + map2D.anchoredPosition.y;

        // Update player icon's position on the 2D map
        RectTransform playerIconRectTransform = GetComponent<RectTransform>();
        if (playerIconRectTransform)
        {
            playerIconRectTransform.anchoredPosition = mapPosition;
        }
        else
        {
            Debug.LogError("Missing RectTransform on the player icon.");
        }
    }
}
