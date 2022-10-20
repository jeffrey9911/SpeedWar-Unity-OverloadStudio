using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneConnect : MonoBehaviour
{
    public static SceneConnect instance;

    private List<string> nameList;
    private List<string> cntxtList;

    private void Awake()
    {
        if(!instance)
            instance = this;

        nameList = new List<string>();
        cntxtList = new List<string>();

        DontDestroyOnLoad(this.gameObject);
    }

    public void addCntxt(string nameToAdd, string cntxtToAdd)
    {
        nameList.Add(nameToAdd);
        cntxtList.Add(cntxtToAdd);
    }

    public string findCntxt(string nameToFind)
    {
        for(int i = 0; i < nameList.Count; i++)
        {
            if (nameList[i] == nameToFind)
                return cntxtList[i];
        }
        return null;
    }

    public void changeCntxt(string nameToChange, string cntxtToChange)
    {
        for (int i = 0; i < nameList.Count; i++)
        {
            if (nameList[i] == nameToChange)
                cntxtList[i] = cntxtToChange;
        }
    }

    public void clearCntxt()
    {
        nameList.Clear();
        cntxtList.Clear();
    }


}
