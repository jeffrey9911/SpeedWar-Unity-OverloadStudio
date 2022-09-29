using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform _cameraController;

    public Camera _camera;

    // Modifier
    public float cameraFollowSpeedModifier = 3.0f;
    public float cameraRotateSpeedModifier = 3.0f;

    private void Awake()
    {
        if(_cameraController == null)
        {
            _cameraController = this.transform.Find("");
        }
        
        if(this == Camera.main)
        {
            _camera = this.GetComponent<Camera>();
        }
    }

    private void FixedUpdate()
    {
        _camera.transform.position = 
            Vector3.Lerp(_camera.transform.position, _cameraController.position,
                        Vector3.Distance(_camera.transform.position, _cameraController.position) * Time.deltaTime * cameraFollowSpeedModifier);

        _camera.transform.rotation =
            Quaternion.Lerp(_camera.transform.rotation, Quaternion.LookRotation(_cameraController.forward),
                                (_camera.transform.forward - _cameraController.forward).magnitude * Time.deltaTime * cameraRotateSpeedModifier);
    }
}
