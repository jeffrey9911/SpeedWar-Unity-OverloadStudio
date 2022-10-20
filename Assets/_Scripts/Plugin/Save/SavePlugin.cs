using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;

public class SavePlugin : MonoBehaviour
{
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

    string fn;

    private void Start()
    {
        fn = Application.dataPath + "/savedKartTransform.txt";
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            StartWriting(fn);
            Transform _playerTransform = PunManager.instance._spawnedPlayer.transform;
            SaveToFile(0, _playerTransform.position.x, _playerTransform.position.y, _playerTransform.position.z);
            SaveToFile(0, _playerTransform.rotation.x, _playerTransform.rotation.y, _playerTransform.rotation.z);
            EndWriting();

            Debug.Log("Transform has saved to: " + fn);
        }
    }
}
