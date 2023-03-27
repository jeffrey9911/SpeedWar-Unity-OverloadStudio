using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMSourceControl : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip firstClip;
    public AudioClip secondClip;

    private bool isOnSecond = false;

    private void Start()
    {
        audioSource.clip = firstClip;
        audioSource.Play();
    }

    private void Update()
    {
        if(!audioSource.isPlaying && !isOnSecond)
        {
            isOnSecond = true;
            audioSource.loop = true;
            audioSource.clip = secondClip;
            audioSource.Play();
        }
    }

}
