using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    public static TrackCheckpoints instance;

    private List<CheckpointSingle> checkpointSingleList = new List<CheckpointSingle>();
    private int nextCheckpointSingleIndex;

    private void Awake()
    {
        if(!instance)
            instance = this;

        //Finds the parent object
        Transform checkpointsTransform = GameObject.Find("Checkpoints").transform;
        //Debug.Log(checkpointsTransform);

        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            //Debug.Log(checkpointSingleTransform);
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
            
            //checkpointSingle.SetTrackCheckpoints(this);

            checkpointSingleList.Add(checkpointSingle);

            checkpointSingleTransform.GetComponent<MeshRenderer>().enabled = false;
        }

        nextCheckpointSingleIndex = 0;

        checkpointSingleList[0].GetComponent<MeshRenderer>().enabled = true;
    }

    public void PlayerThroughCheckpoint(CheckpointSingle checkpointSingle)
    {
        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)
        {
            //Correct checkpoint
            Debug.Log("Correct");
            checkpointSingleList[nextCheckpointSingleIndex].GetComponent<MeshRenderer>().enabled = false;
            nextCheckpointSingleIndex = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;
            checkpointSingleList[nextCheckpointSingleIndex].GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            //Wrong checkpoint
            Debug.Log("WRONG WAY!");
        }
    }

    public int getTargetPointIndex()
    {
        return nextCheckpointSingleIndex;
    }
}
