using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static Transform _spawnPos;
    public static string _defaultKartID = "006";

    public struct InputState
    {
        public int tick;
        public Vector2 input;

    }

    public struct TransformState
    {
        public int tick;
        public Vector3 position;
        public Vector3 rotation;
    }
    

    public struct NetPlayer
    {
        public GameObject playerObj;
        

    }

    public GameObject localPlayer;



    public static Dictionary<short, GameObject> onNetPlayerDList = new Dictionary<short, GameObject>();

    private GameObject _spawnPrefab;


    //public static byte[] playerUpdateByte = new byte[26];

    private void Start()
    {
        _spawnPos = GameObject.Find("spawnPosition").GetComponent<Transform>();
        if (SceneDataManager.instance)
        {
            //isOnNetWork = GameplayManager.instance.getData(GameplayManager.instance.selectedMode) == 2f;

            if (SceneDataManager.instance.getData(SceneData.SelectedMode) == "Online")
            {
                NetworkManager.isOnNetwork = true;
            }

            if (SceneDataManager.instance.getData(SceneData.SelectedMode) == "Offline")
            {
                NetworkManager.isOnNetwork = false;
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

    private static void NetPlayerSpawn(short playerID/*, float posX, float posY, float posZ, float rotX, float rotY, float rotZ*/)
    {
        var _kartKAM = GameplayManager.instance.kartAssetManager.getKart(_defaultKartID);
        onNetPlayerDList.Add(playerID, Instantiate(_kartKAM.AssetPrefab, _spawnPos.position, _spawnPos.rotation));
        onNetPlayerDList[playerID].GetComponent<KartController>().isOnDisplay = true;
        onNetPlayerDList[playerID].GetComponent<KartController>().displayPlayerID = playerID;
        //onNetPlayerDList[playerID].transform.position = new Vector3(posX, posY, posZ);
        //onNetPlayerDList[playerID].transform.rotation = Quaternion.Euler(new Vector3(rotX, rotY, rotZ));
    }

    public static void UpdateOnNetPlayer(ref byte[] buffer)
    {
        
        short[] shortBuffer = new short[1];
        Buffer.BlockCopy(buffer, 0, shortBuffer, 0, 2);
        //Debug.Log(buffer.Length);
        short playerIDin = shortBuffer[0];
        

        /*
        short[] shortBuffer = new short[1];
        Buffer.BlockCopy(playerUpdateByte, 0, shortBuffer, 0, 2);
        short playerIDin = shortBuffer[0];
        */

        Debug.Log("LocalID: " + NetworkManager.localPlayerID + " IDin: " + shortBuffer[0] + " is in?: " + onNetPlayerDList.ContainsKey(playerIDin));


        if (playerIDin != NetworkManager.localPlayerID)
        {
            float[] fPos = { 0, 0, 0 };
            float[] fRot = { 0, 0, 0 };



            Buffer.BlockCopy(buffer, 0 + 2, fPos, 0, fPos.Length * 4);
            Buffer.BlockCopy(buffer, 0 + 2 + 12, fRot, 0, fRot.Length * 4);

            Debug.Log(shortBuffer[0] + ": " + fPos[0] + " " + fPos[1] + " " + fPos[2]);
            Debug.Log(shortBuffer[0] + ": " + fRot[0] + " " + fRot[1] + " " + fRot[2]);


            

            if (playerIDin >= 1000 && !onNetPlayerDList.ContainsKey(playerIDin))
            {
                NetPlayerSpawn(playerIDin);
            }


            if (onNetPlayerDList.ContainsKey(shortBuffer[0]))
            {
                onNetPlayerDList[shortBuffer[0]].transform.position = new Vector3(fPos[0], fPos[1], fPos[2]);
                onNetPlayerDList[shortBuffer[0]].transform.rotation = Quaternion.Euler(new Vector3(fRot[0], fRot[1], fRot[2]));
            }

            fPos = null;
            fRot = null;

            //Array.Clear(fPos, 0, fPos.Length);
            //Array.Clear(fRot, 0, fRot.Length);
        }

        buffer = null;
        //Array.Clear(shortBuffer, 0, shortBuffer.Length);
    }


}
