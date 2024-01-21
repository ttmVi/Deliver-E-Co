using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManager;
    private AudioSource audioSource;

    [SerializeField] AudioClip hoverSound;
    [SerializeField] AudioClip clickSound;

    [SerializeField] AudioClip popUpOrderSound;
    [SerializeField] AudioClip acceptOrderSound;
    [SerializeField] AudioClip pickUpOrderSound;
    [SerializeField] AudioClip dropOffOrderSound;

    [SerializeField] AudioClip openMapSound;
    [SerializeField] AudioClip closeMapSound;

    [SerializeField] AudioClip AQIWarningSound;

    [SerializeField] AudioClip time20SecondsLeftSound;
    [SerializeField] AudioClip time10SecondsLeftSound;

    [SerializeField] AudioClip losingSound;
    [SerializeField] AudioClip winningSound;

    void Awake()
    {
        audioManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayHoverSound()
    {
        AudioSource.PlayClipAtPoint(hoverSound, Camera.main.transform.position);
    }

    public void PlayClickSound()
    {
        AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position);
    }

    public void PlayPopUpOrderSound()
    {
        AudioSource.PlayClipAtPoint(popUpOrderSound, Camera.main.transform.position);
    }

    public void PlayAcceptOrderSound()
    {
        AudioSource.PlayClipAtPoint(acceptOrderSound, Camera.main.transform.position);
    }

    public void PlayPickUpOrderSound()
    {
        AudioSource.PlayClipAtPoint(pickUpOrderSound, GameObject.Find("Player").transform.position);
    }

    public void PlayDropOffOrderSound()
    {
        AudioSource.PlayClipAtPoint(dropOffOrderSound, GameObject.Find("Player").transform.position);
    }

    public void PlayAQIWarningSound()
    {
        AudioSource.PlayClipAtPoint(AQIWarningSound, GameObject.Find("Player").transform.position);
    }

    public void PlayTime20SecondsLeftSound()
    {
        AudioSource.PlayClipAtPoint(time20SecondsLeftSound, GameObject.Find("Player").transform.position);
    }

    public void PlayTime10SecondsLeftSound()
    {
        AudioSource.PlayClipAtPoint(time10SecondsLeftSound, GameObject.Find("Player").transform.position);
    }

    public void PlayOpenMapSound()
    {
        AudioSource.PlayClipAtPoint(openMapSound, GameObject.Find("Player").transform.position);
    }

    public void PlayCloseMapSound()
    {
        AudioSource.PlayClipAtPoint(closeMapSound, GameObject.Find("Player").transform.position);
    }

    public void PlayLosingSound()
    {
        AudioSource.PlayClipAtPoint(losingSound, Camera.main.transform.position);
    }

    public void PlayWinningSound()
    {
        AudioSource.PlayClipAtPoint(winningSound, Camera.main.transform.position);
    }
}
