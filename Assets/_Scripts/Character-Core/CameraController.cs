using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform _cameraTrans;
    private float defaultHeight;

    public Transform _cameraRotator;

    private Vector2 cameraRotVec = Vector2.zero;

    public Camera _camera;

    // Modifier
    public float cameraFollowSensitivity = 10.0f;
    public float cameraRotateSensitivity = 15.0f;

    private float cFS = 0.0f;
    private float cRS = 0.0f;

    public float adaptiveCameraModifier = 100.0f;

    private bool isInitializing = true;

    private bool isViewing = false;

    private void cameraRot(Vector2 rotVec)
    {
        if(rotVec.x < 0)
        {
            _cameraRotator.localEulerAngles = new Vector3(0.0f, 90f * (rotVec.y + 1.0f), 0.0f);
        }

        if(rotVec.x >= 0)
        {
            _cameraRotator.localEulerAngles = new Vector3(0.0f, -90f * (rotVec.y + 1.0f), 0.0f);
        }

        

        if (rotVec.y == 0 && rotVec.x == 0)
        {
            _cameraRotator.localEulerAngles = Vector3.zero;
        }
        
    }

    private void speedCam()
    {
        float kartSpeedIndex = NetworkManager.localPlayer.playerTransform.GetComponent<KartController>().getSpeed / 230f;

        float targetView = 70.0f + (kartSpeedIndex * 30);
        if (isViewing) targetView = 70.0f + (kartSpeedIndex * 50);

        _camera.fieldOfView = targetView;
        if (!isViewing)
        {
            _cameraRotator.localPosition = new Vector3(_cameraRotator.localPosition.x, defaultHeight + kartSpeedIndex * 3, _cameraRotator.localPosition.z);
        }
        else
        {
            _cameraRotator.localPosition = new Vector3(_cameraRotator.localPosition.x, defaultHeight, _cameraRotator.localPosition.z);
        }
        
    }

    private void Awake()
    {
        
    }

    private void Start()
    {
        if (_cameraTrans == null)
        {
            //_cameraTrans = this.transform.Find("");
        }

        if (this == Camera.main)
        {
            _camera = this.GetComponent<Camera>();
        }

        cFS = cameraFollowSensitivity;
        cRS = cameraRotateSensitivity;
        defaultHeight = _cameraRotator.localPosition.y;
    }

    private void FixedUpdate()
    {
        if (isInitializing && KartController.instance != null)
        {
            Debug.Log("SETUP CAMERA!");
            KartController.instance.inputActions.Camera.CameraRotation.performed += context => cameraRot(context.ReadValue<Vector2>());
            KartController.instance.inputActions.Camera.CameraRotation.performed += context => cFS = 30f;
            KartController.instance.inputActions.Camera.CameraRotation.performed += context => cRS = 40f;
            KartController.instance.inputActions.Camera.CameraRotation.performed += context => isViewing = true;
            KartController.instance.inputActions.Camera.CameraRotation.canceled += context => cameraRot(Vector2.zero);
            KartController.instance.inputActions.Camera.CameraRotation.canceled += context => cFS = cameraFollowSensitivity;
            KartController.instance.inputActions.Camera.CameraRotation.canceled += context => cRS = cameraRotateSensitivity;
            KartController.instance.inputActions.Camera.CameraRotation.canceled += context => isViewing = false;
            isInitializing = false;
        }

        speedCam();

        _camera.transform.position =
            Vector3.Lerp(_camera.transform.position, _cameraTrans.position,
                        Vector3.Distance(_camera.transform.position, _cameraTrans.position) * Time.deltaTime * cFS);

        _camera.transform.rotation =
            Quaternion.Lerp(_camera.transform.rotation, Quaternion.LookRotation(_cameraTrans.forward),
                                (_camera.transform.forward - _cameraTrans.forward).magnitude * Time.deltaTime * cRS);
    }

    private void Update()
    {
        //cameraRot(Vector2.zero);
        
        //speedCam();

        
    }
}
