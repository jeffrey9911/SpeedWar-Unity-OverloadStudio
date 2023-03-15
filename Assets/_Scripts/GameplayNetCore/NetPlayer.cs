using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NetPlayer : MonoBehaviour
{
    public GameObject playerObj;
    public string playerName;
    public short playerID;
    public string playerKartID;

    public TransformState serverState;
    public TransformState obsoleteState;

    public TransformState processedState;

    private float timeDifference;
    private float timer;
    
    private void Awake()
    {
        playerObj = this.gameObject;
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
