using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public bool isOnNetWork = false;

    public Transform _spawnPos;
    public string _defaultKartID = "001";

    public GameObject localPlayer;



    private GameObject _spawnPrefab;

    private void Start()
    {
        if (SceneDataManager.instance)
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

            if (_spawnPrefab == null)
                _spawnPrefab = SceneDataManager.instance.kartAssetManager.getKart(_defaultKartID).AssetPrefab;
        }
        else
        {
            _spawnPrefab = GameplayManager.instance.kartAssetManager.getKart(_defaultKartID).AssetPrefab;
        }




        kartSpawn();
    }

    private void kartSpawn()
    {
        if (isOnNetWork)
        {
            //_spawnedPlayer = PhotonNetwork.Instantiate(_spawnPrefab.name, _spawnPos.position, _spawnPos.rotation);
        }
        else
        {
            localPlayer = Instantiate(_spawnPrefab, _spawnPos.position, _spawnPos.rotation);

        }


        var _kartKAM = GameplayManager.instance.kartAssetManager.getKart(_defaultKartID);
        localPlayer.gameObject.GetComponent<KartController>().KartSetup(_kartKAM._acceleration, _kartKAM._maxSpeed, _kartKAM._drift, _kartKAM._control, _kartKAM._weight);

        localPlayer.GetComponent<KartController>()._gameManager = GameplayManager.instance._gameManager;
    }
}
