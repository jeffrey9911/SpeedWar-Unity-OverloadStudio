using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PunManager : MonoBehaviour
{
    public static PunManager instance;

    public bool isOnNetWork = false;

    public GameObject _gameManager;

    public GameObject _spawnPrefab;

    public Transform _spawnPos;

    [SerializeField]
    public GameObject _spawnedPlayer;

    private void Start()
    {
        if(!instance)
        {
            instance = this;
        }

        _gameManager = this.gameObject;

        kartSpawn();
    }

    private void kartSpawn()
    {
        if (isOnNetWork)
            _spawnedPlayer = PhotonNetwork.Instantiate(_spawnPrefab.name, _spawnPos.position, Quaternion.identity);
        else
            _spawnedPlayer = Instantiate(_spawnPrefab, _spawnPos.position, Quaternion.identity);

        _spawnedPlayer.GetComponent<KartController>()._gameManager = _gameManager;
    }
}
