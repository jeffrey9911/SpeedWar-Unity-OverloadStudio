using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PunGameSpawn : MonoBehaviour
{

    public GameObject _spawnPrefab;

    public Transform _spawnPos;

    private void Start()
    {
        PhotonNetwork.Instantiate(_spawnPrefab.name, _spawnPos.position, Quaternion.identity);
    }
}
