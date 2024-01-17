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
            StartCoroutine(GameSceneManager.LoseLevel($"Oh no! You crashed..."));
        }
    }
}
