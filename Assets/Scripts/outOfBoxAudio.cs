using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class outOfBoxAudio : MonoBehaviour
{
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        //AudioSource source = GetComponent<AudioSource>();
        audioSource.Play();
    }
}
