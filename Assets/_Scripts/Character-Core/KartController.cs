using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using UnityEditor.Build;

public class KartController : MonoBehaviour
{
    public static KartController instance;

    public KartAction inputActions;


    [Header("Manager Systems")]
    [SerializeField] public bool isOnDisplay = false;
    [SerializeField] public float onDisplayScale = 1.0f;
    public GameObject _gameManager;
    [SerializeField] private bool isOnNetwork = false;
    Publisher _publisher = new Publisher();

    [Space(10)]


    

    [Header("Vehicle Systems Setup")]
    [SerializeField] private Transform vehicle_centre;
    private Rigidbody _rigidbody;
    [SerializeField] private List<Transform> exhaustTransList = new List<Transform>();

    public enum Wheel_Location
    {
        Front_Left,
        Front_Right,
        Rear_Left,
        Rear_Right
    }

    [Serializable]
    public class Wheel
    {
        public WheelCollider _wCollider;
        public GameObject _wObject;
        public Wheel_Location _wLocation;
    }


    public static int numOfWheels = 4;
    [SerializeField] public Wheel[] wheels = new Wheel[numOfWheels];


    private itemCommand _itemCommandPack;

    private float kartSpeed = 0.0f;

    private Vector2 kartMoveVector = new Vector2(0.0f, 0.0f);
    private Vector2 moveActionVector;







    [SerializeField] private float hbWheelSideFric = 0.5f;
    [SerializeField] private float nmWheelSideFric = 1.0f;

    [Space(10)]

    [Header("Kart Properties")]
    [SerializeField] private float torque_max = 1000.0f;             // Torque
    [SerializeField] private float brakeTorque_max = 10000.0f;      // Brake Torque
    [SerializeField] private float steerAngle_max = 30.0f;          // Steer angle
    [SerializeField] private float kineticRecycleForce = 1.0f;      // Kineteic Recycle
    [SerializeField] private float gravMult = 100.0f;               // Down force
    [SerializeField] private float speed_min = -10;                 // Min Speed
    [SerializeField] public float speed_max = 100;                  // Max Speed
    // drift
    [SerializeField] private float steerSensitivity = 1.0f;         // Steer sensitivity
    [SerializeField] private float kartWeight;                      // Weight
    [SerializeField] private float antiRoll = 5000.0f;              // Anti roll force

    [Space]

    // Kart States
    [Header("Kart States")]
    public bool isHandbrake = false;
    public bool isBoosted = false;

    private float timeCounter = 0;
    [SerializeField] private float fireTimeInterval;

    [Space(10)]

    [Header("DEBUGGING")]
    [SerializeField] private bool isDebugging = false;
    public float wDampingRate = 0.25f;
    public float sDistance = 0.2f;
    public float sSpringConstant = 37500f;      // Normally = kart weight / num of wheels * number between 50 to 100
    public float sSpringDamper = 4500f;
    public bool isUsingAntiRoll = true;



    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    void Start()
    {
        //isOnNetwork = PunManager.instance.isOnNetWork;

        if(!isOnDisplay)
        {
            inputActions = KartInputController.inst_controller.inputActions;

            //inputActions
            inputActions.Player.Move.performed += context => moveActionVector = context.ReadValue<Vector2>();
            inputActions.Player.Move.canceled += context => moveActionVector = Vector2.zero;

            inputActions.Player.Handbrake.performed += context => isHandbrake = true;
            inputActions.Player.Handbrake.canceled += context => isHandbrake = false;



            GameObject.FindGameObjectWithTag("LocalCamera").GetComponent<CameraController>()._cameraTrans = this.transform.Find("camController").Find("camTrans").transform;
            GameObject.FindGameObjectWithTag("LocalCamera").GetComponent<CameraController>()._cameraRotator = this.transform.Find("camController").transform;

            _rigidbody = GetComponent<Rigidbody>();
            if (vehicle_centre != null && _rigidbody != null)
            {
                //_rigidbody.centerOfMass = vehicle_centre.localPosition;
                _rigidbody.mass = kartWeight;
            }

            AchievementObserver fastObserver = new AchievementObserver(this.gameObject, new fastPoints());
            _publisher.AddObserver(fastObserver);

            AchievementObserver driftObserver = new AchievementObserver(this.gameObject, new driftPoints());
            _publisher.AddObserver(driftObserver);
        }
        else
        {
            this.transform.localScale = new Vector3(onDisplayScale, onDisplayScale, onDisplayScale);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            itemEffectInvoker.AddItem(new itemUseCommand(other.GetComponent<Item>().ItemName));
            Destroy(other.gameObject);
        }

        if (LayerMask.LayerToName(other.gameObject.layer) == "Item")
        {
            scoreManager.instance.addScore(1000);
            Destroy(other.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("==== SHOW ITEMS IN THE PACK ====");
            for (int i = 0; i < itemEffectInvoker._itemCommands.Count; i++)
            {
                Debug.Log(itemEffectInvoker._itemCommands[i].getItemName());
            }
            Debug.Log("==== POINTS THE PLAYER HAS ====");
            Debug.Log(scoreManager.instance.getScore());
            Debug.Log("================================");
        }
    }

    private void FixedUpdate()
    {
        if(!isOnDisplay)
        {
            if (isUsingAntiRoll) AntiRoll();
            KartMove(moveActionVector);
            UpdateVelocity();
            UpdateGameplayUI();


            if (isBoosted)
            {
                if (timeCounter <= 0)
                {
                    for (int i = 0; i < exhaustTransList.Count; i++)
                    {
                        GameObject exFire;

                        exFire = ObjectPool.instance.SpawnFromPool("BoostFire", exhaustTransList[i].position, Quaternion.identity);


                        exFire.GetComponent<ExhaustFire>().SetTimedActive();
                        Rigidbody exFireRb = exFire.GetComponent<Rigidbody>();

                        exFireRb.velocity = Vector3.zero;

                        exFireRb.AddForce(exhaustTransList[i].forward.normalized, ForceMode.Impulse);
                    }
                    timeCounter += fireTimeInterval;

                }
                else
                {
                    timeCounter -= Time.deltaTime;
                }

            }

            if (kartSpeed > 100.0f || isHandbrake)
            {
                _publisher.Notify();
            }
        }

        
    }


    private void syncWheel(Wheel wheel)
    {
        if (wheel._wObject == null || wheel._wCollider == null)
        {
            return;
        }
            

        Vector3 wPos;
        Quaternion wRot;
        
        wheel._wCollider.GetWorldPose(out wPos, out wRot);

        wheel._wObject.transform.position = wPos;
        wheel._wObject.transform.rotation = wRot;
    }

    private void debugKart(Wheel wheel)
    {
        if (vehicle_centre != null && _rigidbody != null)
        {
            _rigidbody.centerOfMass = vehicle_centre.localPosition;
            _rigidbody.mass = kartWeight;
        }

        if (wheel._wObject != null || wheel._wCollider != null)
        {
            wheel._wCollider.wheelDampingRate = wDampingRate;
            wheel._wCollider.suspensionDistance = sDistance;
            JointSpring spring = new JointSpring();
            spring.spring = sSpringConstant;
            spring.damper = sSpringDamper;
            spring.targetPosition = 0.5f;
            wheel._wCollider.suspensionSpring = spring;
        }
        
    }

    private void AntiRoll()
    {
        WheelCollider wcFL = new();
        WheelCollider wcFR = new();
        WheelCollider wcRL = new();
        WheelCollider wcRR = new();

        foreach (Wheel wheel in wheels)
        {
            if (wheel._wLocation == Wheel_Location.Front_Left) wcFL = wheel._wCollider;
            if (wheel._wLocation == Wheel_Location.Front_Right) wcFR = wheel._wCollider;
            if (wheel._wLocation == Wheel_Location.Rear_Left) wcRL = wheel._wCollider;
            if (wheel._wLocation == Wheel_Location.Rear_Right) wcRR = wheel._wCollider;
        }

        if (wcFL == null || wcFR == null || wcRL == null || wcRR == null) return;

        WheelBalance(wcFL, wcFR);
        WheelBalance(wcRL, wcRR);
    }

    private void WheelBalance(WheelCollider wcLeft, WheelCollider wcRight)
    {
        WheelHit wheelHit;

        float travelLeft = 1.0f;
        float travelRight = 1.0f;

        bool isGroundedLeft = wcLeft.GetGroundHit(out wheelHit);
        if (isGroundedLeft)
        {
            travelLeft = (-wcLeft.transform.InverseTransformPoint(wheelHit.point).y - wcLeft.radius) / wcLeft.suspensionDistance;
        }

        bool isGroundedRight = wcRight.GetGroundHit(out wheelHit);
        if (isGroundedRight)
        {
            travelRight = (-wcRight.transform.InverseTransformPoint(wheelHit.point).y - wcRight.radius) / wcRight.suspensionDistance;
        }

        float balanceForce = (travelLeft - travelRight) * antiRoll;

        if (isGroundedLeft)
        {
            _rigidbody.AddForceAtPosition(wcLeft.transform.up * -balanceForce, wcLeft.transform.position);
        }

        if (isGroundedRight)
        {
            _rigidbody.AddForceAtPosition(wcRight.transform.up * balanceForce, wcRight.transform.position);
        }
    }

    private void KineticRecycle()
    {
        this._rigidbody.AddForce(-this.transform.forward * kineticRecycleForce);
        /*
        if (wheels[0]._wCollider == null)
            return;

        for(int i = 0; i < wheels.Length; i++)
        {
            wheels[i]._wCollider.attachedRigidbody.AddForce(-transform.up * gravMult * wheels[i]._wCollider.attachedRigidbody.velocity.magnitude);
        }*/

    }

    public float getSpeed { get { return kartSpeed; } }

    private void UpdateVelocity()
    {
        kartSpeed = _rigidbody.velocity.magnitude * 3.6f;

        kartSpeed *= Vector3.Dot(_rigidbody.velocity, this.transform.forward) > 0 ? 1.0f : -1.0f;

        if (kartSpeed > speed_max && !isBoosted)
        {
            _rigidbody.velocity = (speed_max / 3.6f) * _rigidbody.velocity.normalized;
            
        }
        else if(kartSpeed < speed_min)
        {
            _rigidbody.velocity = -(speed_min / 3.6f) * _rigidbody.velocity.normalized;
        }
    }


    private void KartMove(Vector2 moveVec)
    {
        //Debug.Log(moveVec);

        float steerAng = kartMoveVector.x = Mathf.Lerp(kartMoveVector.x, moveVec.x * steerAngle_max, Time.deltaTime * steerSensitivity);

        float torqueForce = kartMoveVector.y = Mathf.Lerp(kartMoveVector.y, moveVec.y * torque_max, Time.deltaTime);

        if(moveVec.y == 0)
        {
            torqueForce = kartMoveVector.y = 0;
        }

        for(int i = 0; i < wheels.Length; i++)
        {
            if (wheels[i]._wCollider == null)
                break;

            /// Wheels tunring
            if (wheels[i]._wLocation == Wheel_Location.Front_Left
                || wheels[i]._wLocation == Wheel_Location.Front_Right)
            {
                wheels[i]._wCollider.steerAngle = steerAng;
            }

            /// Not Handbraking
            if (!isHandbrake)
            {
                SetSideFriction(wheels[i], nmWheelSideFric);
                /// Speeding Up by W
                if (moveVec.y > 0)
                {
                    //Debug.Log("SPEEDING!!!" + torqueForce);
                    wheels[i]._wCollider.motorTorque = torqueForce;
                    wheels[i]._wCollider.brakeTorque = 0.0f;
                }
                /// Slowing Down by S
                else if (moveVec.y < 0)
                {
                    
                    if (kartSpeed > 0)
                    {
                        //Debug.Log("BRAKING!!!" + torqueForce);
                        if (wheels[i]._wLocation == Wheel_Location.Front_Left || wheels[i]._wLocation == Wheel_Location.Front_Right)
                            wheels[i]._wCollider.brakeTorque = -moveVec.y * brakeTorque_max;
                        else
                            wheels[i]._wCollider.brakeTorque = -moveVec.y * brakeTorque_max / 2;

                        wheels[i]._wCollider.motorTorque = 0.0f;
                    }
                    else if(kartSpeed < 0 || Mathf.Approximately(kartSpeed, 0.0f))
                    {
                        //Debug.Log("BACKING!!!" + torqueForce);
                        wheels[i]._wCollider.motorTorque = torqueForce * 0.1f;
                        wheels[i]._wCollider.brakeTorque = 0.0f;
                    }
                }
                else if(moveVec.y == 0)
                {
                    //Debug.Log("STILL!!!" + torqueForce);
                    wheels[i]._wCollider.motorTorque = 0.0f;
                    wheels[i]._wCollider.brakeTorque = kineticRecycleForce;

                }
            }
            /// IS Handbraking
            else
            {
                if (wheels[i]._wLocation == Wheel_Location.Rear_Left || wheels[i]._wLocation == Wheel_Location.Rear_Right)
                {
                    wheels[i]._wCollider.brakeTorque = brakeTorque_max;
                    SetSideFriction(wheels[i], hbWheelSideFric);
                }
                else
                    wheels[i]._wCollider.brakeTorque = kineticRecycleForce;

                wheels[i]._wCollider.motorTorque = 0.0f;
            }

            syncWheel(wheels[i]);

            if (isDebugging)
            {
                debugKart(wheels[i]);
            }
        }
    }

    private void SetSideFriction(Wheel wheel, float frictionValue)
    {
        WheelFrictionCurve sFriction = wheel._wCollider.sidewaysFriction;
        sFriction.stiffness = frictionValue;
        wheel._wCollider.sidewaysFriction = sFriction;
    }

    private void UpdateGameplayUI()
    {
        GameplayUIManager.instance.UpdateSpeedometer(kartSpeed / speed_max);
        GameplayUIManager.instance.UpdateTorqueBar(moveActionVector.y > 0 ? moveActionVector.y : 0);
        GameplayUIManager.instance.UpdateSpeedValue((int)kartSpeed);
    }

    public void KartSetup(float acceleration, float maxSpeed, float drift, float control, float weight)
    {
        torque_max = acceleration;
        speed_max = maxSpeed;

        steerSensitivity = control;
        kartWeight = weight;
    }
}
