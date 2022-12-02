using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SceneConnect : MonoBehaviour
{
    public static SceneConnect instance;


    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
            

        DontDestroyOnLoad(this.gameObject);
    }
}
