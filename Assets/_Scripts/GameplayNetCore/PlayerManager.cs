using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public bool isOnNetWork = false;

    public static Transform _spawnPos;
    public static string _defaultKartID = "006";

    public GameObject localPlayer;

    public static Dictionary<short, GameObject> onNetPlayerDList = new Dictionary<short, GameObject>();

    private GameObject _spawnPrefab;

    private void Start()
    {
        _spawnPos = GameObject.Find("spawnPosition").GetComponent<Transform>();
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




        PlayerSpawn();
    }

    private void PlayerSpawn()
    {
        
        localPlayer = Instantiate(_spawnPrefab, _spawnPos.position, _spawnPos.rotation);

        var _kartKAM = GameplayManager.instance.kartAssetManager.getKart(_defaultKartID);
        localPlayer.gameObject.GetComponent<KartController>().KartSetup(_kartKAM._acceleration, _kartKAM._maxSpeed, _kartKAM._drift, _kartKAM._control, _kartKAM._weight);

        localPlayer.GetComponent<KartController>()._gameManager = GameplayManager.instance._gameManager;
    }

    private static void NetPlayerSpawn(short playerID)
    {
        var _kartKAM = GameplayManager.instance.kartAssetManager.getKart(_defaultKartID);
        onNetPlayerDList.Add(playerID, Instantiate(_kartKAM.AssetPrefab, _spawnPos.position, _spawnPos.rotation));
        onNetPlayerDList[playerID].GetComponent<KartController>().isOnDisplay = true;
        onNetPlayerDList[playerID].GetComponent<KartController>().displayPlayerID = playerID;
    }

    public static void UpdateOnNetPlayer(byte[] buffer)
    {
        short[] shortBuffer = new short[1];
        float[] fPos = { 0, 0, 0 };
        float[] fRot = { 0, 0, 0 };

        Buffer.BlockCopy(buffer, 0, shortBuffer, 0, 2);
        Buffer.BlockCopy(buffer, 0 + 2, fPos, 0, fPos.Length * 4);
        Buffer.BlockCopy(buffer, 0 + 2 + 12, fRot, 0, fRot.Length * 4);

        Debug.Log(shortBuffer[0] + ": " + fPos[0] + " " + fPos[1] + " " + fPos[2]);
        Debug.Log(shortBuffer[0] + ": " + fRot[0] + " " + fRot[1] + " " + fRot[2]);
        
        if (!onNetPlayerDList.ContainsKey(shortBuffer[0])) NetPlayerSpawn(shortBuffer[0]);


        onNetPlayerDList[shortBuffer[0]].transform.position = new Vector3(fPos[0], fPos[1], fPos[2]);
        onNetPlayerDList[shortBuffer[0]].transform.rotation = Quaternion.Euler(new Vector3(fRot[0], fRot[1], fRot[2]));
    }

    public static void ConPrint(byte[] buffer)
    {
        
    }
}
