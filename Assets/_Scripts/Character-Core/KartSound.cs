using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartSound : MonoBehaviour
{
    
    public float maxEnginePitch = 2.0f;
    public float minEnginePitch = 0.5f;

    private AudioSource _kartSoundSource;
    [SerializeField]
    private AudioClip _kartSound;

    private Rigidbody _kartRB;

    private KartController _kartController;

    void Start()
    {
        _kartSoundSource = GetComponent<AudioSource>();
        _kartRB = GetComponent<Rigidbody>();
        _kartController = GetComponent<KartController>();

        _kartSoundSource.clip = _kartSound;
        _kartSoundSource.playOnAwake = true;
        _kartSoundSource.loop = true;
        _kartSoundSource.volume = 0.5f;
    }

    void Update()
    {
        _kartSoundSource.pitch = Mathf.Lerp(minEnginePitch, maxEnginePitch, _kartController.getSpeed / _kartController.speed_max);
    }
}
