using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Data.Common;
using UnityEngine.InputSystem;

public class RewindController : MonoBehaviour
{
    #region SAVE PLUGIN
    [DllImport("SavePlugin")]
    private static extern void SetID(int id);

    [DllImport("SavePlugin")]
    private static extern void SetPosition(float x, float y, float z);

    [DllImport("SavePlugin")]
    private static extern void SaveToFile(int id, float x, float y, float z);

    [DllImport("SavePlugin")]
    private static extern void StartWriting(string fileName);

    [DllImport("SavePlugin")]
    private static extern void EndWriting();
    #endregion


    #region LOAD PLUGIN
    [DllImport("LoadPlugin")]
    private static extern float LoadFromFile(int j, string fileName);


    [DllImport("LoadPlugin")]
    private static extern int GetLines(string fileName);
    #endregion


    string fn;

    private float autoSaveTimer = 3;
    private float rewindTimer = 3;
    private bool isRewind = false;

    public float minDist = 0.0f;

    Vector3 loadedPos = Vector3.zero;
    Vector3 loadedRot = Vector3.zero;

    KartAction inputActions;

    private void Start()
    {
        fn = Application.dataPath + "/savedKartTransform.txt";

        inputActions = KartInputController.inst_controller.inputActions;

        inputActions.Player.Rewind.performed += context => RewindStart();
    }

    private void Update()
    {
        

        if(isRewind)
        {
            PunManager.instance._spawnedPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
            PunManager.instance._spawnedPlayer.transform.position = Vector3.Lerp(PunManager.instance._spawnedPlayer.transform.position, loadedPos, Time.deltaTime * 3);
            PunManager.instance._spawnedPlayer.transform.rotation = Quaternion.Lerp(PunManager.instance._spawnedPlayer.transform.rotation, Quaternion.Euler(loadedRot), Time.deltaTime * 3);

            Vector3 kartPos = PunManager.instance._spawnedPlayer.transform.position;
            if(Vector3.Distance(kartPos, loadedPos) <= minDist)
            {
                Debug.Log("STOP REWIND");
                isRewind = false;
            }
            else
            {
                Debug.Log("KEEP REWIND" + " [" + Vector3.Distance(kartPos, loadedPos));
            }
            rewindTimer -= Time.deltaTime;

            if(rewindTimer <= 0)
            {
                rewindTimer = 3.0f;
                isRewind = false;
            }
        }
    }

    private void FixedUpdate()
    {
        /*
        if(!isRewind)
        {
            if (autoSaveTimer <= 0)
            {
                StartWriting(fn);
                Transform _playerTransform = PunManager.instance._spawnedPlayer.transform;
                SaveToFile(0, _playerTransform.position.x, _playerTransform.position.y, _playerTransform.position.z);
                SaveToFile(0, _playerTransform.rotation.eulerAngles.x, _playerTransform.rotation.eulerAngles.y, _playerTransform.rotation.eulerAngles.z);
                EndWriting();
                autoSaveTimer = 3;
                Debug.Log("Transform has saved to: " + fn);
            }
            else
            {
                autoSaveTimer -= Time.deltaTime;
            }
        }
        */
    }

    private void RewindStart()
    {
        loadedPos.x = (float)LoadFromFile(1, fn);
        loadedPos.y = (float)LoadFromFile(2, fn);
        loadedPos.z = (float)LoadFromFile(3, fn);

        loadedRot.x = (float)LoadFromFile(5, fn);
        loadedRot.y = (float)LoadFromFile(6, fn);
        loadedRot.z = (float)LoadFromFile(7, fn);

        isRewind = true;

        Debug.Log("Save loaded!");
        Debug.Log("Kart is transform by: " + "[Position]: " + loadedPos + "[Rotation]: " + loadedRot);
    }
}
