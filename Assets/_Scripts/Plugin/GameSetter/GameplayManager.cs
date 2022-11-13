using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using System;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance;

    [DllImport("GameSetter")]
    private static extern void readDataSet(string filePathName);

    [DllImport("GameSetter")]
    private static extern void writeDataSet(string filePathName);

    [DllImport("GameSetter")]
    private static extern void endRWDataSet();

    [DllImport("GameSetter")]
    private static extern int searchDataByTag(string filePathName, string tagToSearch);

    [DllImport("GameSetter")]
    private static extern IntPtr getDataByTag(string filePathName, string tagToSearch);

    [DllImport("GameSetter")]
    private static extern bool setDataByTag(string filePathName, string tagToSearch, string contxtToSet);


    [DllImport("GameSetter")]
    private static extern bool removeDataPiece(string filePathName, string tagToDelete);

    string fn;

    private void Awake()
    {
        if(!instance)
            instance = this;
    }

    private void Start()
    {
        fn = Application.dataPath + "/GameplayManager.txt";
        writeDataSet(fn);
        endRWDataSet();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            setDataByTag(fn, "TEST10", "TTEESSTT");
            setDataByTag(fn, "TEST20", "HAHAHA");
            Debug.Log("SETTED");

            IntPtr _intptr = getDataByTag(fn, "TEST20"); ;
            
            
            Debug.Log(Marshal.PtrToStringUTF8(_intptr));
        }
        
    }
}
