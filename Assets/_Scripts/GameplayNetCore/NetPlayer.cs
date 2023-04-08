using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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


    // Constructor
    public NetPlayer(short consID, string consName)
    {
        playerID = consID;
        playerName = consName;
    }

    public NetPlayer(short consID, string consName, string consKartID, short consRoomID, short consLevelID)
    {
        playerID = consID;
        playerName = consName;

        playerKartID = consKartID;
        playerRoomID = consRoomID;
        playerLevelID = consLevelID;
    }


    // Motion prediction
    public TransformState serverState;
    public TransformState obsoleteState;
    public TransformState processedState;

    private float timeDifference;
    private float timer;
    
    private void Awake()
    {
        playerTransform = this.transform;
    }

    private void Start()
    {
        serverState = new TransformState();
        obsoleteState = new TransformState();
        processedState = new TransformState();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        HandleUpdate();
    }


    void HandleUpdate()
    {
        if (!obsoleteState.Equals(default(TransformState))  && !serverState.Equals(default(TransformState)))
        {
            timeDifference = 0.3f;//GameplayManager.instance.networkManager.sendInterval;// + NetworkManager.checkedLatency;
            this.transform.position = Vector3.Lerp(serverState.position, processedState.position, timer / timeDifference);
            this.transform.rotation = Quaternion.Lerp(serverState.rotation, processedState.rotation, timer / timeDifference);
            this.transform.GetComponent<KartController>().UpdateMoveAction(Vector2.Lerp(serverState.input, processedState.input, timer / timeDifference));
        }
    }

    public void ServerStateUpdate(Vector3 pos, Quaternion rot, Vector2 input)
    {
        obsoleteState = serverState;
        serverState = new TransformState(pos, rot, input);
        processedState.position = serverState.position + (serverState.position - obsoleteState.position);
        processedState.rotation = serverState.rotation * (serverState.rotation * Quaternion.Inverse(obsoleteState.rotation));
        processedState.input = serverState.input + (serverState.input - obsoleteState.input);
        this.transform.position = pos;
        this.transform.rotation = rot;
        timer = 0;
    }



}
