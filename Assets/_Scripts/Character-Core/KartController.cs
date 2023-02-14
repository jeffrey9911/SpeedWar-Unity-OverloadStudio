using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using TMPro;
using UnityEditor.Build;

public class KartController : MonoBehaviour
{
    public static KartController instance;

    public bool isUsingOBJPOOL = true;

    public GameObject _gameManager;

    public KartAction inputActions;

    [Header("Managers")]
    private bool isOnNetwork = false;
    public bool isUsingSway = false;

    
    PhotonView _punView;

    Publisher _publisher = new Publisher();

    [Header("Vehicle Setup")]
    [SerializeField] private Transform vehicle_centre;

    private Rigidbody _rigidbody;

    private float kartSpeed = 0.0f;

    [SerializeField] private TMP_Text _sText; // later link to UI

    /// <summary>
    /// Setup for wheels. class of variables of wheels
    /// </summary>
    #region wheels variables
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
        public GameObject _wObject;
        public Wheel_Location _wLocation;
    }
    

    [Header("Wheels Setup")]
    [SerializeField] private Wheel[] wheels = new Wheel[numOfWheels];
    [SerializeField] private float hbWheelSideFric = 0.5f;
    [SerializeField] private float nmWheelSideFric = 1.0f;
    #endregion

    #region vehicle variables
    [Header("Kart Properties")]

    // Torque
    [SerializeField] private float torque_max = 1000.0f;


    [SerializeField] private float brakeTorque_max = 10000.0f;

    // Steering
    [SerializeField] private float steerAngle_max = 30.0f;


    [SerializeField] private float kineticRecycleForce = 1.0f;
    [SerializeField] private float gravMult = 100.0f;



    [SerializeField] private float speed_min = -10;

    //[SerializeField] private static int numberOfExhaust;
    //[SerializeField] private Transform[] exhaustTrans = new Transform[numberOfExhaust];
    [SerializeField] private List<Transform> exhaustTransList = new List<Transform>();

    [Header("Kart Stats")]
    [SerializeField] private float torqueSensitivity = 1.0f;                     // Torque index
    [SerializeField] public float speed_max = 100;                              // Max Speed
    // drift

    [SerializeField] private float steerSensitivity = 1.0f;                      // Steer index
    [SerializeField] private float kartWeight;                              //Weight

    [SerializeField] private float antiRoll = 5000.0f;


    #endregion



    private bool isHandbrake = false;

    private Vector2 kartMoveVector = new Vector2(0.0f, 0.0f);
    private Vector2 moveActionVector;

    private itemCommand _itemCommandPack;


    public bool isBoosted = false;

    private float timeCounter = 0;
    [SerializeField] private float fireTimeInterval;


    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    void Start()
    {
        isOnNetwork = PunManager.instance.isOnNetWork;
        if (isOnNetwork)
        {
            _punView = this.transform.GetParentComponent<PhotonView>();
        }
            

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
            _rigidbody.centerOfMass = vehicle_centre.localPosition;
            _rigidbody.mass = kartWeight;
        }
            

        _sText = GameObject.FindGameObjectWithTag("GUI").transform.Find("PNL_UI").Find("TXT_SpeedMeter").GetComponent<TMP_Text>();

        AchievementObserver fastObserver = new AchievementObserver(this.gameObject, new fastPoints());
        _publisher.AddObserver(fastObserver);

        AchievementObserver driftObserver = new AchievementObserver(this.gameObject, new driftPoints());
        _publisher.AddObserver(driftObserver);
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
        if (isOnNetwork)
        {
            if (_punView.IsMine)
            {
                KartMove(moveActionVector);
                UpdateVelocity();
                UpdateGameplayUI();
            }
        }
        else
        {
            KartMove(moveActionVector);
            if(isUsingSway)
            {
                AntiRoll();
            }
            
            UpdateVelocity();
            UpdateGameplayUI();
        }

        

        if (_sText != null)
            _sText.text = ((int)kartSpeed).ToString() + " KPH";

        if(isBoosted)
        {
            if(timeCounter <= 0)
            {
                for (int i = 0; i < exhaustTransList.Count; i++)
                {
                    GameObject exFire;

                    if(isUsingOBJPOOL)
                    {
                        exFire = ObjectPool.instance.SpawnFromPool("BoostFire", exhaustTransList[i].position, Quaternion.identity);
                    }
                    else
                    {
                        exFire = Instantiate(ObjectPool.instance.pools[0]._prefab, exhaustTransList[i].position, Quaternion.identity);
                    }
                    
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

        if(kartSpeed > 100.0f || isHandbrake)
        {
            _publisher.Notify();
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

    
    /// <summary>
    /// Apply aerodynamics force onto the wheels
    /// </summary>
    private void AntiRoll()
    {
        /*
        this._rigidbody.AddForce(-this.transform.forward * gravMult);
        
        if (wheels[0]._wCollider == null)
            return;

        for(int i = 0; i < wheels.Length; i++)
        {
            wheels[i]._wCollider.attachedRigidbody.AddForce(-transform.up * gravMult * wheels[i]._wCollider.attachedRigidbody.velocity.magnitude);
        }*/
        float travelLeft = 1.0f;
        float travelRight = 1.0f;

        var groundHit = new WheelHit();

        foreach (Wheel wheel in wheels)
        {
            if(wheel._wLocation == Wheel_Location.Front_Left || wheel._wLocation == Wheel_Location.Rear_Left)
            {
                var groundedLeft = wheel._wCollider.GetGroundHit(out groundHit);
                if (groundedLeft)
                {
                    travelLeft = (-wheel._wCollider.transform.InverseTransformPoint(groundHit.point).y - wheel._wCollider.radius) / wheel._wCollider.suspensionDistance;

                    float antiRollForce = (travelLeft - travelRight) * antiRoll;

                    _rigidbody.AddForceAtPosition(wheel._wCollider.transform.up * antiRollForce, wheel._wCollider.transform.position);
                }
            }
            else if(wheel._wLocation == Wheel_Location.Front_Right || wheel._wLocation == Wheel_Location.Rear_Right)
            {
                var groundedRight = wheel._wCollider.GetGroundHit(out groundHit);
                if (groundedRight)
                {
                    travelRight = (-wheel._wCollider.transform.InverseTransformPoint(groundHit.point).y - wheel._wCollider.radius) / wheel._wCollider.suspensionDistance;

                    float antiRollForce = (travelLeft - travelRight) * antiRoll;

                    _rigidbody.AddForceAtPosition(wheel._wCollider.transform.up * antiRollForce, wheel._wCollider.transform.position);
                }
            }
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

        float torqueForce = kartMoveVector.y = Mathf.Lerp(kartMoveVector.y, moveVec.y * torque_max, Time.deltaTime * torqueSensitivity);

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
    }

    public void KartSetup(float acceleration, float maxSpeed, float drift, float control, float weight)
    {
        torque_max = acceleration;
        speed_max = maxSpeed;

        steerSensitivity = control;
        kartWeight = weight;
    }
}
