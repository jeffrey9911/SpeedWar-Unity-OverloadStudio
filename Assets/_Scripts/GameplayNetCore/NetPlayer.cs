using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NetPlayer : MonoBehaviour
{
    // Player Net ID
    public short playerID;
    public string playerName;

    // Gameplay ID
    public string playerKartID = "";
    public short playerRoomID = -1;
    public short playerLevelID = 0;

    public Transform playerTransform;

    public short score = -1;

    // Constructor
    public NetPlayer(short consID, string consName)
    {
        playerID = consID;
        playerName = consName;
    }

    // Update levelid
    public NetPlayer(short consID, string consName, short consLevelID)
    {
        playerID = consID;
        playerName = consName;

        playerLevelID = consLevelID;
    }
    public NetPlayer SetPlayerLevelID(short levelIDtoSet)
    {
        return new NetPlayer(playerID, playerName, levelIDtoSet);
    }

    // Creating game
    private NetPlayer(short consID, string consName, string consKartID, short consLevelID)
    {
        playerID = consID;
        playerName = consName;

        playerKartID = consKartID;
        playerLevelID = consLevelID;
    }
    public NetPlayer SetPlayerOnRoomCreated(string consKartID, short consLevelID)
    {
        return new NetPlayer(playerID, playerName, consKartID, consLevelID);
    }

    // Set Room id
    private NetPlayer(short consID, string consName, string consKartID, short consLevelID, short consRoomID)
    {
        playerID = consID;
        playerName = consName;

        playerKartID = consKartID;
        playerLevelID = consLevelID;
        playerRoomID = consRoomID;
    }
    public NetPlayer SetPlayerRoomID(short consRoomID)
    {
        return new NetPlayer(playerID, playerName, playerKartID, playerLevelID, consRoomID);
    }

    // Set Score
    private NetPlayer(short consID, string consName, string consKartID, short consLevelID, short consRoomID, short consScore)
    {
        playerID = consID;
        playerName = consName;

        playerKartID = consKartID;
        playerLevelID = consLevelID;
        playerRoomID = consRoomID;
        score = consScore;
    }
    public NetPlayer SetPlayerScore(short consScore)
    {
        return new NetPlayer(playerID, playerName, playerKartID, playerLevelID, playerRoomID, consScore);
    }

    // Creating object
    private NetPlayer(short consID, string consName, string consKartID, short consRoomID, short consLevelID, Transform consTransform)
    {
        playerID = consID;
        playerName = consName;

        playerKartID = consKartID;
        playerRoomID = consRoomID;
        playerLevelID = consLevelID;

        playerTransform = consTransform;
    }
    public NetPlayer UpdatePlayerKartObject(Transform consTransform)
    {
        return new NetPlayer(playerID, playerName, playerKartID, playerRoomID, playerLevelID, consTransform);
    }
}
