using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerColliding : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided");

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Gotohell");
            StartCoroutine(GameSceneManager.LoseLevel($"you crashed into the {tag}"));
        }
    }
}
