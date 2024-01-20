using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    [SerializeField] AudioClip hoverSound;
    [SerializeField] AudioClip clickSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayHoverSound()
    {
        AudioSource.PlayClipAtPoint(hoverSound, Camera.main.transform.position);
    }

    public void PlayClickSound()
    {
        AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position);
    }
}
