using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Reflection;


public class SceneDataManager : MonoBehaviour
{
    public static SceneDataManager instance;

    public KartAssetManager kartAssetManager;

    private Dictionary<string, string> sceneDataSet;

    private SceneData sceneData;

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

        kartAssetManager = this.gameObject.GetComponent<KartAssetManager>();

        sceneData = new SceneData();

        sceneDataSet = new Dictionary<string, string>();

        
    }

    private void Start()
    {
        // Initialize DataSet
        foreach (string str in sceneData.DataTypes)
        {
            if (!sceneDataSet.ContainsKey(str))
            {
                sceneDataSet.Add(str, null);
            }
        }

        
    }

    

    public bool setData(string keyTag, string content)
    {
        if (sceneDataSet.ContainsKey(keyTag))
        {
            sceneDataSet[keyTag] = content;
            return true;
        }
        else
        {
            sceneDataSet.Add(keyTag, null);
            return false;
        }
    }

    public string getData(string keyTag)
    {
        if (sceneDataSet.ContainsKey(keyTag))
        {
            return sceneDataSet[keyTag];
        }
        else
        {
            return null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (string keyTag in sceneDataSet.Keys)
            {
                Debug.Log(keyTag + " : " + sceneDataSet[keyTag]);
            }
        }
    }

}
