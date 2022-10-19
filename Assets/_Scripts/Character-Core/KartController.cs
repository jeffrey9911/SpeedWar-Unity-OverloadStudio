using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class KartController : MonoBehaviour
{
    //public static KartController instance;
   
    public GameObject _gameManager;

    public KartAction inputActions;

    [Header("Photon Network")]
    private bool isOnNetwork = false;

    
    PhotonView _punView;

    

    [Header("Vehicle Setup")]
    [SerializeField] private Transform vehicle_centre;

    private Rigidbody _rigidbody;

    private float kartSpeed = 0.0f;

    [SerializeField] private Text _sText; // later link to UI

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
        public GameObject _gameObject;
        public Wheel_Location _wLocation;
    }
    

    [Header("Wheels Setup")]
    [SerializeField] private Wheel[] wheels = new Wheel[numOfWheels];
    #endregion

    [Header("Vehicle Parameters")]
    [SerializeField] private float torque_max = 1000.0f;
    [SerializeField] private float brakeTorque_max = 10000.0f;
    [SerializeField] private float steerAngle_max = 30.0f;
    [SerializeField] private float steerSensitivity = 1.0f;
    [SerializeField] private float torqueSensitivity = 1.0f;
    [SerializeField] private float kineticRecycleForce = 1.0f;
    [SerializeField] private float wheelStiffness = 3.0f;
    //[SerializeField] private static int NumOfGears = 5;

    [SerializeField] private float gravMult = 100.0f;
    [SerializeField] private float speed_max = 200, speed_min = -10;

    private bool isHandbrake = false;

    private Vector2 kartMoveVector = new Vector2(0.0f, 0.0f);

    //private bool isMoveingForward = true;

    public bool speedEffected = false;

    itemCommand _itemCommand;
   
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

    /*
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

            

            /// Not Handbraking - WS calculation
            if(!isHandbrake)
            {
                wheels[i]._wCollider.brakeTorque = 0.0f;

                /// Speeding Up by W
                if (isSpeedingUp)
                {
                    
                    if (isBoost)
                    {
                        //wheels[i]._wCollider.motorTorque = torqueForce * torque_max * 4.0f;
                        _rigidbody.velocity = (speed_max / 1.0f) * _rigidbody.velocity.normalized;
                    }
                }
                /// Slowing Down by S
                else if (isSlowingDown)
                {
                    if(isMoveingForward)
                    {

                        if(!Mathf.Approximately(kartSpeed, 0.0f))
                        {
                            wheels[i]._wCollider.motorTorque = torqueForce * torque_max;
                        }
                        else
                        {
                            if (wheels[i]._wLocation == Wheel_Location.Front_Left || wheels[i]._wLocation == Wheel_Location.Front_Right)
                            {
                                wheels[i]._wCollider.brakeTorque = torqueForce * brakeTorque_max;
                                //wheels[i]._wCollider.brakeTorque = brakeTorque_max * 40.0f;
                                
                            }
                            wheels[i]._wCollider.motorTorque = 0.0f;
                        }

                    }
                    else
                    {
                        
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

            

           
        }

        
    }
*/
    /// <summary>
    /// Apply aerodynamics force onto the wheels
    /// </summary>
    private void applyDownForce()
    {
        if (wheels[0]._wCollider == null)
            return;

        for(int i = 0; i < wheels.Length; i++)
        {
            wheels[i]._wCollider.attachedRigidbody.AddForce(-transform.up * gravMult * wheels[i]._wCollider.attachedRigidbody.velocity.magnitude);
        }
        
    }

    private void UpdateVelocithy()
    {
        kartSpeed = _rigidbody.velocity.magnitude * 3.6f;

        kartSpeed *= Vector3.Dot(_rigidbody.velocity, this.transform.forward) > 0 ? 1.0f : -1.0f;

        if (kartSpeed > speed_max && !speedEffected)
        {
            _rigidbody.velocity = (speed_max / 3.6f) * _rigidbody.velocity.normalized;
            
        }
        else if(kartSpeed < speed_min)
        {
            _rigidbody.velocity = -(speed_min / 3.6f) * _rigidbody.velocity.normalized;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        isOnNetwork = PunManager.instance.isOnNetWork;
        if(isOnNetwork)
            _punView = this.transform.GetParentComponent<PhotonView>();

        inputActions = KartInputController.inst_controller.inputActions;

        //inputActions.Player.Move.performed += context => KartMove(context.ReadValue<Vector2>());

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>()._cameraController = this.transform.Find("cameraController").transform;
        
        _rigidbody = GetComponent<Rigidbody>();
        if (vehicle_centre != null && _rigidbody != null)
            _rigidbody.centerOfMass = vehicle_centre.localPosition;
        /*
        for(int i = 0; i < wheels.Length; ++i)
        {
            syncWheel(wheels[i]);
        }*/
        
    }

    private void KartMove(Vector2 moveVec)
    {
        //Debug.Log(kartMoveVector);

        float steerAng = kartMoveVector.x = Mathf.Lerp(kartMoveVector.x, moveVec.x * steerAngle_max, Time.deltaTime * steerSensitivity);

        float torqueForce = kartMoveVector.y = Mathf.Lerp(kartMoveVector.y, moveVec.y * torque_max, Time.deltaTime * torqueSensitivity);


        for(int i = 0; i < wheels.Length; i++)
        {
            if (wheels[i]._wCollider == null)
                break;

            /// Not Handbraking
            if(!isHandbrake)
            {
                /// Wheels tunring
                if (wheels[i]._wLocation == Wheel_Location.Front_Left
                    || wheels[i]._wLocation == Wheel_Location.Front_Right)
                {
                    wheels[i]._wCollider.steerAngle = steerAng;
                }

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
                            wheels[i]._wCollider.brakeTorque = kineticRecycleForce;

                        wheels[i]._wCollider.motorTorque = 0.0f;
                    }
                    else if(kartSpeed < 0 || Mathf.Approximately(kartSpeed, 0.0f))
                    {
                        //Debug.Log("BACKING!!!" + torqueForce);
                        wheels[i]._wCollider.motorTorque = torqueForce * 0.1f;
                        wheels[i]._wCollider.brakeTorque = 0.0f;
                    }
                }
                else
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
                    wheels[i]._wCollider.brakeTorque = brakeTorque_max;
                else
                    wheels[i]._wCollider.brakeTorque = kineticRecycleForce;

                wheels[i]._wCollider.motorTorque = 0.0f;
            }


            

            if (wheels[i]._gameObject != null)
                syncWheel(wheels[i]);
        }
    }

    private void KartDrive()
    {
        isHandbrake = Input.GetKey(KeyCode.Space);

        Vector2 moveVector = new Vector2();

        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.S))
                moveVector.y = 0.0f;
            else
                moveVector.y = 1.0f;
        }
        else if (Input.GetKey(KeyCode.S))
            moveVector.y = -1.0f;
        else
            moveVector.y = 0.0f;



        if (Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A))
                moveVector.x = 0.0f;
            else
                moveVector.x = 1.0f;
        }
        else if (Input.GetKey(KeyCode.A))
            moveVector.x = -1.0f;
        else
            moveVector.x = 0.0f;
        


        KartMove(moveVector);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Item")
        {
            _itemCommand = new itemUseCommand(other.gameObject.tag);
            itemEffectInvoker.AddItem(_itemCommand);
            Destroy(other.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnNetwork)
        {
            if (_punView.IsMine)
            {
                KartDrive();
            }
        }
        else
        {
            KartDrive();
            UpdateVelocithy();
            //applyDownForce();
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("==== SHOW ITEMS IN THE PACK ====");
            for(int i = 0; i < itemEffectInvoker._itemCommands.Count; i++)
            {
                Debug.Log(itemEffectInvoker._itemCommands[i].getItemName());
            }
            Debug.Log("================================");
        }

        if (_sText != null)
            _sText.text = ((int)calc_speed).ToString() + " KPH";
    }

    private void FixedUpdate()
    {
        
    }
}
