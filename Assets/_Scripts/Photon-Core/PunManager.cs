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

        
        if(SceneDataManager.instance)
        {
            //isOnNetWork = GameplayManager.instance.getData(GameplayManager.instance.selectedMode) == 2f;

            if (SceneDataManager.instance.getData(SceneData.SelectedMode) == "Online")
            {
                isOnNetWork = true;
            }

            if (SceneDataManager.instance.getData(SceneData.SelectedMode) == "Offline")
            {
                isOnNetWork = false;
            }

            _spawnPrefab = SceneDataManager.instance.kartAssetManager.getKart(SceneDataManager.instance.getData(SceneData.SelectedKart)).AssetPrefab;
        }

        if (_spawnPrefab == null)
            _spawnPrefab = SceneDataManager.instance.kartAssetManager.getKart("001").AssetPrefab;


        if (isSpawnPlayer)
        {
            kartSpawn();
        }
    }

    private void kartSpawn()
    {
        if (isOnNetWork)
        {
            _spawnedPlayer = PhotonNetwork.Instantiate(_spawnPrefab.name, _spawnPos.position, _spawnPos.rotation);
        }
        else
        {
            _spawnedPlayer = Instantiate(_spawnPrefab, _spawnPos.position, _spawnPos.rotation);
        }
            

        _spawnedPlayer.GetComponent<KartController>()._gameManager = _gameManager;
    }
}
