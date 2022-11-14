using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using System;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance;

    private class dataTag
    {
        public List<string> dataTags = new List<string>();
        public dataTag()
        {
            dataTags.Clear();
            dataTags.Add(audioVolume);
            dataTags.Add(selectedMode);
            dataTags.Add(selectedCar);
        }
        public string audioVolume { get { return "audioVolume"; } }
        public string selectedMode { get { return "selectedMode"; } }
        public string selectedCar { get { return "selectedCar"; } }
    }

    [DllImport("GameSetter")]
    private static extern bool readDataSet(string filePathName);

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

    private string fn;

    dataTag _dataTag = new dataTag();

    private Dictionary<string, string> gameplaySetupDict = new();

    private void Awake()
    {
        if(!instance)
            instance = this;
    }

    private void Start()
    {
        fn = Application.dataPath + "/GameplayManager.txt";

        initialRW();
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

    private void initialRW()
    {
        if(!readDataSet(fn))
        {
            writeDataSet(fn);
            endRWDataSet();
        }
            
        foreach (string dTag in _dataTag.dataTags)
        {
            if (searchDataByTag(fn, dTag) < 0)
            {
                setDataByTag(fn, dTag, "");
            }

            gameplaySetupDict.Add(dTag, getStringData(dTag));
        }
    }

    private string getStringData(string tagToSearch)
    {
        IntPtr _intptr = getDataByTag(fn, tagToSearch);
        return Marshal.PtrToStringUTF8(_intptr);
    }
}
