using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSFX : MonoBehaviour
{
    private AudioSource audioSource;
    private int vehicleToIndex;
    [SerializeField] private AudioClip[] idleSounds;
    [SerializeField] private AudioClip[] drivingSounds;
    [SerializeField] private AudioClip[] startingSounds;
    [SerializeField] private AudioClip[] brakingSounds;
    [SerializeField] private AudioClip[] honkingSounds;
    [SerializeField] private AudioClip refuelingSound;

    private void Awake()
    {
        audioSource = GameObject.Find("Player").GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (VehicleManager.playerVehicle.vehicleCapacity == 2) //bicycle
        {
            vehicleToIndex = 0;
        }
        else if (VehicleManager.playerVehicle.vehicleCapacity == 4) //motorbike
        {
            vehicleToIndex = 1;
        }
        else if (VehicleManager.playerVehicle.vehicleCapacity == 10) //truck
        {
            vehicleToIndex = 2;
        }
    }

    public void PlayIdleSound()
    {
        if (idleSounds[vehicleToIndex] == null)
        {
            audioSource.Stop();
        }
        else
        {
            audioSource.clip = idleSounds[vehicleToIndex];
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void PlayDrivingSound()
    {
        audioSource.clip = drivingSounds[vehicleToIndex];
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlayStartingSound()
    {
        AudioSource.PlayClipAtPoint(startingSounds[vehicleToIndex], GameObject.Find("Player").transform.position);
    }

    public void PlayBrakingSound()
    {
        AudioSource.PlayClipAtPoint(brakingSounds[vehicleToIndex], GameObject.Find("Player").transform.position);
    }

    public void PlayHonkingSound() 
    {
        AudioSource.PlayClipAtPoint(honkingSounds[vehicleToIndex], GameObject.Find("Player").transform.position);
    }

    public void PlayRefuelingSound()
    {
        AudioSource.PlayClipAtPoint(refuelingSound, GameObject.Find("Player").transform.position);
    }
}
