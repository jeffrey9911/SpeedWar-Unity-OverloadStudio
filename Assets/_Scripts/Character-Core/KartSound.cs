using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartSound : MonoBehaviour
{
    struct ShiftSpeed
    {
        public float minSpeed {  get; set; }
        public float maxSpeed { get; set; }

        public ShiftSpeed(float cMin, float cMax)
        {
            minSpeed = cMin;
            maxSpeed = cMax;
        }
    }

    private List<ShiftSpeed> shiftSpeedList = new List<ShiftSpeed>();

    private float minPitch = 0.6f;
    private float maxPitch = 2.0f;
    private float midPitch = 1.2f;

    private AudioSource _kartSoundSource;

    private KartController _kartController;

    void Start()
    {
        _kartSoundSource = GetComponent<AudioSource>();
        _kartController = GetComponent<KartController>();

        _kartSoundSource.playOnAwake = true;
        _kartSoundSource.loop = true;

        if(_kartController.spawnMode != 0)
        {
            _kartSoundSource.mute = true;
        }

        if(_kartController.spawnMode == 0)
        {
            shiftSpeedList.Add(new ShiftSpeed(-11, 0)); // revers
            shiftSpeedList.Add(new ShiftSpeed(0, 25));  //1
            shiftSpeedList.Add(new ShiftSpeed(25, 40));//2
            shiftSpeedList.Add(new ShiftSpeed(40, 60));//3
            shiftSpeedList.Add(new ShiftSpeed(60, 80));//4
            shiftSpeedList.Add(new ShiftSpeed(80, 100));//5
            shiftSpeedList.Add(new ShiftSpeed(100, 130));//6
            shiftSpeedList.Add(new ShiftSpeed(130, 170));//7
            //8
            _kartSoundSource.Play();
        }
    }

    void Update()
    {
        if(_kartController.spawnMode == 0)
        {
            CheckSpeed();
        }
    }

    void CheckSpeed()
    {
        int speedShift = CheckShift();
        float level = 0;
        float targetPitch = 0;
        switch(speedShift)
        {
            case -1:
                break;

            case 0:
                level = Mathf.Abs(_kartController.getSpeed) / 10f;
                targetPitch = Mathf.Lerp(minPitch, 0.9f, level);
                break;


            default:
                float kartSpeed = _kartController.getSpeed;
                level = (kartSpeed - shiftSpeedList[speedShift].minSpeed) / (shiftSpeedList[speedShift].maxSpeed - shiftSpeedList[speedShift].minSpeed);

                if(speedShift == 1)
                {
                    targetPitch = Mathf.Lerp(minPitch, maxPitch, level);
                    _kartSoundSource.pitch = Mathf.Lerp(_kartSoundSource.pitch, targetPitch, Time.deltaTime * 100);
                }
                else
                {
                    targetPitch = Mathf.Lerp(midPitch, maxPitch, level);
                }

                break;

        }
        _kartSoundSource.pitch = Mathf.Lerp(_kartSoundSource.pitch, targetPitch, Time.deltaTime * 200);



    }

    private int CheckShift()
    {
        float kartSpeed = _kartController.getSpeed;
        for (int i = 0; i < shiftSpeedList.Count; i++)
        {
            if(kartSpeed >= shiftSpeedList[i].minSpeed && kartSpeed < shiftSpeedList[i].maxSpeed)
            {
                return i;
            }
        }

        if(kartSpeed >= shiftSpeedList[shiftSpeedList.Count - 1].maxSpeed)
        {
            return 8;
        }

        return -1;
    }
}
