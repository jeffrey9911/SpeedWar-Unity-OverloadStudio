using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayer : MonoBehaviour
{
    public GameObject playerObj;
    public string playerName;
    public short playerID;
    public string playerKartID;

    private void Awake()
    {
        playerObj = this.gameObject;
    }
}
