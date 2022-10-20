using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using System.Data.Common;

public class LoadPlugin : MonoBehaviour
{
    [DllImport("LoadPlugin")]
    private static extern float LoadFromFile(int j, string fileName);


    [DllImport("LoadPlugin")]
    private static extern int GetLines(string fileName);

    string fn;

    private void Start()
    {
        fn = Application.dataPath + "/savedKartTransform.txt";
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            Vector3 loadedPos = Vector3.zero;
            Vector3 loadedRot = Vector3.zero;

            loadedPos.x = (float)LoadFromFile(1, fn);
            loadedPos.y = (float)LoadFromFile(2, fn);
            loadedPos.z = (float)LoadFromFile(3, fn);

            loadedRot.x = (float)LoadFromFile(5, fn);
            loadedRot.y = (float)LoadFromFile(6, fn);
            loadedRot.z = (float)LoadFromFile(7, fn);

            PunManager.instance._spawnedPlayer.transform.position = loadedPos;
            PunManager.instance._spawnedPlayer.transform.rotation = Quaternion.Euler(loadedRot);

            Debug.Log("Save loaded!");
            Debug.Log("Kart is transform by: " + "[Position]: " + loadedPos + "[Rotation]: " + loadedRot);
        }
    }
}
