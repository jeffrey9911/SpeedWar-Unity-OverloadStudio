using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PunManager : MonoBehaviour
{
    public static PunManager instance;

    public bool isSpawnPlayer = false;

    public bool isOnNetWork = false;

    public GameObject _gameManager;

    public GameObject _kart1;
    public GameObject _kart2;

    public Transform _spawnPos;

    [SerializeField]
    public GameObject _spawnedPlayer;


    private GameObject _spawnPrefab;

    private void Start()
    {
        if(!instance)
        {
            instance = this;
        }

        _gameManager = this.gameObject;

        
        if(SceneConnect.instance)
        {
            isOnNetWork = SceneConnect.instance.findCntxt("isOnNetwork") == "true";

            switch (SceneConnect.instance.findCntxt("kartID"))
            {
                case "0":
                    _spawnPrefab = _kart1;
                    break;

                case "1":
                    _spawnPrefab = _kart2;
                    break;

                default:
                    
                    break;
            }
        }
        
        if(_spawnPrefab == null)
            _spawnPrefab = _kart2;


        if (isSpawnPlayer)
        {
            kartSpawn();
        }
    }

    private void kartSpawn()
    {
        if (isOnNetWork)
            _spawnedPlayer = PhotonNetwork.Instantiate(_spawnPrefab.name, _spawnPos.position, _spawnPos.rotation);
        else
            _spawnedPlayer = Instantiate(_spawnPrefab, _spawnPos.position, _spawnPos.rotation);

        _spawnedPlayer.GetComponent<KartController>()._gameManager = _gameManager;
    }
}
