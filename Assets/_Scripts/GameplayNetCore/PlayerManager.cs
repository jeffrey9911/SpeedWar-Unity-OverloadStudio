using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public struct TransformState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector2 input;

    public TransformState(Vector3 consPos, Quaternion consRot, Vector2 consIn)
    {
        position = consPos;
        rotation = consRot;
        input = consIn;
    }
}

public class PlayerManager : MonoBehaviour
{
    public static Transform _spawnPos;
    public static string _defaultKartID = "007";

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
    

    public GameObject localPlayer;
    



    public static Dictionary<short, NetPlayer> onNetPlayerDList = new Dictionary<short, NetPlayer>();

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
                Debug.Log("Play Online Mode!");
                NetworkManager networkManager = GameplayManager.instance.gameObject.AddComponent<NetworkManager>();
                NetworkManager.isOnNetwork = true;
            }

            if (SceneDataManager.instance.getData(SceneData.SelectedMode) == "Offline")
            {
                //NetworkManager.isOnNetwork = false;
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
        
        
        KartAsset _kartKAM;
        if(SceneDataManager.instance)
        {
            _kartKAM = SceneDataManager.instance.kartAssetManager.getKart(SceneDataManager.instance.getData(SceneData.SelectedKart));
        }
        else
        {
            _kartKAM = GameplayManager.instance.kartAssetManager.getKart(_defaultKartID);
        }
        localPlayer = Instantiate(_spawnPrefab, _spawnPos.position, _spawnPos.rotation);

        localPlayer.gameObject.GetComponent<KartController>().KartSetup(_kartKAM._acceleration, _kartKAM._maxSpeed, _kartKAM._drift, _kartKAM._control, _kartKAM._weight);

        localPlayer.GetComponent<KartController>()._gameManager = GameplayManager.instance._gameManager;
    }

    public static void CheckPlayerDList(ref byte[] spawnInfo)
    {
        string dlist = Encoding.ASCII.GetString(spawnInfo);
        string[] players = dlist.Split('#');

        foreach (string player in players)
        {
            //Debug.Log(player);
            string[] pInfo = player.Split(",");
            short id = short.Parse(pInfo[0]);
            if(id != NetworkManager.localPlayerID)
            {
                if(onNetPlayerDList.ContainsKey(id))
                {
                    if (onNetPlayerDList[id].playerID != id) onNetPlayerDList[id].playerID = id;
                    if (onNetPlayerDList[id].playerName != pInfo[1]) onNetPlayerDList[id].playerName = pInfo[1];
                    if (onNetPlayerDList[id].playerKartID != pInfo[2]) onNetPlayerDList[id].playerKartID = pInfo[2];
                }
                else
                {
                    var _kartKAM = SceneDataManager.instance.kartAssetManager.getKart(pInfo[2]);
                    NetPlayer newPlayer = Instantiate(_kartKAM.AssetPrefab, _spawnPos.position, _spawnPos.rotation).AddComponent<NetPlayer>();
                    newPlayer.playerObj.GetComponent<KartController>().spawnMode = 2;
                    newPlayer.playerID = id;
                    newPlayer.playerName = pInfo[1];
                    newPlayer.playerKartID = pInfo[2];
                    onNetPlayerDList.Add(id, newPlayer);
                }
            }
        }

        spawnInfo = null;
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

        //Debug.Log("LocalID: " + NetworkManager.localPlayerID + " IDin: " + shortBuffer[0] + " is in?: " + onNetPlayerDList.ContainsKey(playerIDin));


        if (playerIDin != NetworkManager.localPlayerID && onNetPlayerDList.ContainsKey(shortBuffer[0]))
        {
            float[] fPos = { 0, 0, 0 };
            float[] fRot = { 0, 0, 0 };
            float[] fInput = { 0, 0 };


            Buffer.BlockCopy(buffer, 0 + 2, fPos, 0, fPos.Length * 4);
            Buffer.BlockCopy(buffer, 0 + 2 + 12, fRot, 0, fRot.Length * 4);
            Buffer.BlockCopy(buffer, 0 + 2 + 12 + 12, fInput, 0, fInput.Length * 4);
            
            Debug.Log(shortBuffer[0] + ": " + fPos[0] + " " + fPos[1] + " " + fPos[2]);
            Debug.Log(shortBuffer[0] + ": " + fRot[0] + " " + fRot[1] + " " + fRot[2]);
            Debug.Log(shortBuffer[0] + ": " + fInput[0] + " " + fInput[1]);
            
            onNetPlayerDList[shortBuffer[0]].ServerStateUpdate(new Vector3(fPos[0], fPos[1], fPos[2]), Quaternion.Euler(new Vector3(fRot[0], fRot[1], fRot[2])), new Vector2(fInput[0], fInput[1]));


            fPos = null;
            fRot = null;
            fInput = null;
            //Array.Clear(fPos, 0, fPos.Length);
            //Array.Clear(fRot, 0, fRot.Length);
        }

        shortBuffer = null;
        buffer = null;
        //Array.Clear(shortBuffer, 0, shortBuffer.Length);
    }


}
