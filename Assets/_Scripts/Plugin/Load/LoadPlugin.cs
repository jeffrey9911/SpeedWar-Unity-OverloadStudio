using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;

public class LoadPlugin : MonoBehaviour
{
    

    string fn;

    private void Start()
    {
        fn = Application.dataPath + "/savedKartTransform.txt";
    }

    private void Update()
    {
        
    }
}
