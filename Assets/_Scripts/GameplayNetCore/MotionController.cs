using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionController : MonoBehaviour
{
    public struct TransformState
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector2 input;
    }

    public TransformState serverState = new TransformState();
    public TransformState obsoleteState = new TransformState();
    public TransformState processedState = new TransformState();

    float handlePosTimer = 0;
    float handleRotTimer = 0;
    float handleInputTimer = 0;

    private void Update()
    {
        if(this.gameObject.GetComponent<KartController>().spawnMode == 2)
        {
            /*
            this.transform.position = serverState.position;
            this.transform.rotation = Quaternion.Euler(serverState.rotation);
            Debug.Log("Motion Update");*/

            HandlePosUpdate();
            HandleRotUpdate();
            HandleInputUpdate();
        }
    }

    void HandlePosUpdate()
    {
        if(processedState.position != null)
        {
            this.transform.position = Vector3.Lerp(serverState.position, processedState.position, handlePosTimer / (NetworkManager.latency + NetworkManager.updateTimeInterval));
        }
        handlePosTimer += Time.deltaTime;
    }

    void HandleRotUpdate()
    {
        if (processedState.rotation != null)
        {
            this.transform.rotation = Quaternion.Lerp(serverState.rotation, processedState.rotation, handleRotTimer / (NetworkManager.latency + NetworkManager.updateTimeInterval));
        }
        handleRotTimer += Time.deltaTime;
    }

    void HandleInputUpdate()
    {
        if (processedState.input != null)
        {
            this.GetComponent<KartController>().UpdateMoveAction(Vector2.Lerp(serverState.input, processedState.input, handleInputTimer / (NetworkManager.latency + NetworkManager.updateTimeInterval)));
        }
        handleInputTimer += Time.deltaTime;
    }



    public void ServerUpdatePos(Vector3 serverPos)
    {
        obsoleteState.position = serverState.position;
        serverState.position = serverPos;

        processedState.position = serverState.position + (serverState.position - obsoleteState.position);

        handlePosTimer = 0;
    }

    public void ServerUpdateRot(Quaternion serverRot)
    {
        obsoleteState.rotation = serverState.rotation;
        serverState.rotation = serverRot;

        processedState.rotation = serverState.rotation * (serverState.rotation * Quaternion.Inverse(obsoleteState.rotation));

        handleRotTimer = 0;
    }

    public void ServerUpdateInput(Vector2 serverInput)
    {
        obsoleteState.input = serverState.input;
        serverState.input = serverInput;

        processedState.input = serverState.input + (serverState.input - obsoleteState.input);

        handleInputTimer = 0;
    }
}
