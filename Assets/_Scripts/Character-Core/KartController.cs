using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class KartController : MonoBehaviour
{
    public static int numOfWheels = 4;
    public enum Wheel_Location
    {
        Front_Left,
        Front_Right,
        Rear_Left,
        Rear_Right
    }


    [Serializable]
    class Wheel
    {
        public WheelCollider _wCollider;
        public GameObject _gameObject;
        public Wheel_Location _wLocation;
    }

    [SerializeField] private Transform vehicle_centre;

    private Rigidbody _rigidbody;

    [SerializeField] private Text _sText; // later link to UI

    [SerializeField] private Wheel[] wheels = new Wheel[numOfWheels];
    [SerializeField]
    private float torque_max = 1000.0f,
                  brakeTorque_max = 10000.0f,
                  steerAngle_max = 30.0f;
    [SerializeField] private static int NumOfGears = 5;

    [SerializeField] private float gravMult = 100.0f;
    [SerializeField] private float speed_max = 200, speed_min = -10;

    private bool isSpeedingUp = false, isSlowingDown = false, isHandbrake = false;

    public bool isBoost = false;

    private bool isMoveingForward = true;
   
    public float calc_speed
    {
        get
        {
            return _rigidbody.velocity.magnitude * 3.6f;
        }
    }

    private void syncWheel(Wheel wheel)
    {
        if(wheel._gameObject == null || wheel._wCollider == null)
            return;

        Vector3 wPos;
        Quaternion wRot;
        
        wheel._wCollider.GetWorldPose(out wPos, out wRot);

        wheel._gameObject.transform.position = wPos;
        wheel._gameObject.transform.rotation = wRot;
    }
    
    private void vehicleMove(float torqueForce, float steerInput, bool handBrake, bool boost)
    {
        isHandbrake = handBrake;

        torqueForce = Mathf.Clamp(torqueForce, -1.0f, 1.0f);

        steerInput = Mathf.Clamp(steerInput, -1.0f, 1.0f);
        float steerAng = steerInput * steerAngle_max;

        if(torqueForce > 0.0f)
        {
            isSpeedingUp = true;
            isSlowingDown = false;
        }
        else if(torqueForce < 0.0f)
        {
            isSpeedingUp = false;
            isSlowingDown = true;
        }
        else
        {
            isSpeedingUp = isSlowingDown = false;
        }

        isBoost = boost;

        for(int i = 0; i < wheels.Length; ++i)
        {
            if (wheels[i]._wCollider == null)
                break;

            if (wheels[i]._wLocation == Wheel_Location.Front_Left
                || wheels[i]._wLocation == Wheel_Location.Front_Right)
            {
                wheels[i]._wCollider.steerAngle = steerAng;
            }

            if(!isHandbrake) // not handbraking
            {
                wheels[i]._wCollider.brakeTorque = 0.0f;
                if (isSpeedingUp)
                {
                    wheels[i]._wCollider.motorTorque = torqueForce * torque_max / 4.0f;
                    if (isBoost)
                    {
                        //wheels[i]._wCollider.motorTorque = torqueForce * torque_max * 4.0f;
                        _rigidbody.velocity = (speed_max / 1.0f) * _rigidbody.velocity.normalized;
                    }
                }
                else if (isSlowingDown)
                {
                    if(isMoveingForward)
                    {
                        if (wheels[i]._wLocation == Wheel_Location.Front_Left || wheels[i]._wLocation == Wheel_Location.Front_Right)
                        {
                            wheels[i]._wCollider.motorTorque = torqueForce * brakeTorque_max;
                            //wheels[i]._wCollider.brakeTorque = brakeTorque_max * 40.0f;
                        }
                        else
                        {
                            wheels[i]._wCollider.motorTorque = 0.0f;
                        }
                        
                    }
                    else
                    {
                        wheels[i]._wCollider.motorTorque = torqueForce * brakeTorque_max / 10.0f;
                    }
                }
                else
                {
                    if(Mathf.Approximately(_rigidbody.velocity.magnitude, 0.0f))
                    {

                    }
                    wheels[i]._wCollider.motorTorque = 0.0f;
                }
                    
            }
            else //is handbraking
            {
                if (wheels[i]._wLocation == Wheel_Location.Rear_Left
                       || wheels[i]._wLocation == Wheel_Location.Rear_Right)
                {
                    wheels[i]._wCollider.brakeTorque = brakeTorque_max * 50.0f;
                }
                wheels[i]._wCollider.motorTorque = 0.0f;
            }

            if (wheels[i]._gameObject != null)
                syncWheel(wheels[i]);

           
        }

        applyDownForce();
        ApplySpeed();
    }

    private void applyDownForce()
    {
        if (wheels[0]._wCollider == null)
            return;
        wheels[0]._wCollider.attachedRigidbody.AddForce(-transform.up * gravMult
                                                            * wheels[0]._wCollider.attachedRigidbody.velocity.magnitude);
    }

    private void ApplySpeed()
    {
        float speed = _rigidbody.velocity.magnitude;
        speed *= 3.6f;



        float direcIdentifier = Vector3.Dot(_rigidbody.velocity, this.transform.forward);

        if (isMoveingForward && speed > speed_max && !isBoost)
        {
            _rigidbody.velocity = (speed_max / 3.6f) * _rigidbody.velocity.normalized;
            
        }
        else if(!isMoveingForward && -speed < speed_min)
        {
            _rigidbody.velocity = -(speed_min / 3.6f) * _rigidbody.velocity.normalized;
        }

        Debug.Log(isMoveingForward ? speed : -speed + "KM/H");
    }

    private void updateMovingDirection()
    {
        isMoveingForward = Vector3.Dot(_rigidbody.velocity, this.transform.forward) > 0 ? true : false;
    }

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (vehicle_centre != null && _rigidbody != null)
            _rigidbody.centerOfMass = vehicle_centre.localPosition;
        for(int i = 0; i < wheels.Length; ++i)
        {
            syncWheel(wheels[i]);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_sText != null)
            _sText.text = ((int)calc_speed).ToString() + " KPH";
    }

    private void FixedUpdate()
    {
        float motorInput, steerInput;
        bool handBrakeInput;

        bool boostInput;

        motorInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
        handBrakeInput = Input.GetButton("Jump");

        boostInput = Input.GetKey(KeyCode.LeftShift);

        vehicleMove(motorInput, steerInput, handBrakeInput, boostInput);
        updateMovingDirection();
    }
}
