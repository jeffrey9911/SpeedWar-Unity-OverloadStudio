using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform _cameraTrans;

    public Transform _cameraRotator;

    public Camera _camera;

    // Modifier
    public float cameraFollowSensitivity = 3.0f;
    public float cameraRotateSensitivity = 3.0f;

    public float adaptiveCameraModifier = 100.0f;

    private bool cameraRot()
    {
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            if (!Input.GetKey(KeyCode.RightArrow))
            {
                _cameraRotator.localEulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
                return true;
            }
        }
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            _cameraRotator.localEulerAngles = new Vector3(0.0f, -90.0f, 0.0f);
            return true;
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {
            _cameraRotator.localEulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
            return true;
        }
        else
        {
            _cameraRotator.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }
        return false;
    }

    private void speedCam()
    {
        float kartSpeed = PunManager.instance._spawnedPlayer.GetComponent<KartController>().getSpeed();
        _cameraTrans.localEulerAngles = new Vector3(20.0f - (kartSpeed * 20.0f / adaptiveCameraModifier), 0.0f, 0.0f);
    }

    private void Awake()
    {
        if(_cameraTrans == null)
        {
            _cameraTrans = this.transform.Find("");
        }

        if(this == Camera.main)
        {
            _camera = this.GetComponent<Camera>();
        }
    }

    private void FixedUpdate()
    {
        float camRotMult = 1.0f;
        float camFolMult = 1.0f;
        if(cameraRot())
        {
            camRotMult = 3.0f;
            camFolMult = 3.0f;
        }

        speedCam();

        _camera.transform.position = 
            Vector3.Lerp(_camera.transform.position, _cameraTrans.position,
                        Vector3.Distance(_camera.transform.position, _cameraTrans.position) * Time.deltaTime * cameraFollowSensitivity * camFolMult);

        _camera.transform.rotation =
            Quaternion.Lerp(_camera.transform.rotation, Quaternion.LookRotation(_cameraTrans.forward),
                                (_camera.transform.forward - _cameraTrans.forward).magnitude * Time.deltaTime * cameraRotateSensitivity * camRotMult);
    }
}
