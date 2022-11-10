using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    public List<Pool> pools;

    Queue<GameObject> objectQueue;

    public Dictionary<string, Queue<GameObject>> objectQueueDict;

    private void Awake()
    {
        if(!instance)
            instance = this;
    }

    private void Start()
    {
        objectQueueDict = new Dictionary<string, Queue<GameObject>>();

        foreach(Pool pool in pools)
        {
            objectQueue = new Queue<GameObject>();

            for(int i = 0; i < pool._objMaxSize; i++)
            {
                GameObject obj = Instantiate(pool._prefab);
                obj.SetActive(false);
                objectQueue.Enqueue(obj);
            }

            objectQueueDict.Add(pool._objTag, objectQueue);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rot)
    {
        if(!objectQueueDict.ContainsKey(tag))
        {
            return null;
        }

        GameObject objToSpawn = objectQueueDict[tag].Dequeue();

        objToSpawn.SetActive(true);
        objToSpawn.transform.position = pos;
        objToSpawn.transform.rotation = rot;

        objectQueueDict[tag].Enqueue(objToSpawn);

        return objToSpawn;
    }
}
