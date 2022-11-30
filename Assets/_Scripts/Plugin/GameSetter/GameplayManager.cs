using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using System;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance;

    public string audioVolume { get { return "audioVolume"; } }
    public string selectedMode { get { return "selectedMode"; } }
    public string selectedCar { get { return "selectedCar"; } }

    [DllImport("GameSetter")]
    private static extern bool readDataSet(string filePathName);

    [DllImport("GameSetter")]
    private static extern void writeDataSet(string filePathName);

    [DllImport("GameSetter")]
    private static extern void endRWDataSet();

    [DllImport("GameSetter")]
    private static extern int searchDataByTag(string filePathName, string tagToSearch);

    [DllImport("GameSetter")]
    private static extern float getDataByTag(string filePathName, string tagToSearch);

    [DllImport("GameSetter")]
    private static extern bool setDataByTag(string filePathName, string tagToSearch, float contxtToSet);


    [DllImport("GameSetter")]
    private static extern bool removeDataPiece(string filePathName, string tagToDelete);

    private string fn;

 

    private void Awake()
    {
        if (!instance)
            instance = this;

        fn = Application.dataPath + "/GameplayManager.txt";


        //initialRW();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            setDataByTag(fn, "TEST10", 1.111f);
            setData("TEST20", 20.222f);
            setData("TEST20", 20.333f);
            setData("TEST30", 20.333f);
            Debug.Log("SETTED");

            float got = getDataByTag(fn, "TEST20"); ;
            
            
            Debug.Log(got);

        }
    }

    private void initialRW()
    {
        if(!readDataSet(fn))
        {
            Debug.Log("CAN'T READ - Rewrite");
            writeDataSet(fn);
            endRWDataSet();
        }
    }

    public void setData(string _tag, float _value)
    {
        setDataByTag(fn, _tag, _value);
    }

    public float getData(string _tag)
    {
        return getDataByTag(fn, _tag);
    }
}
