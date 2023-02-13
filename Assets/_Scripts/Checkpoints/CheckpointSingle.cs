using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    //private TrackCheckpoints trackCheckpoints;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.transform.tag);
        if (other.transform.tag == "KartCollider")
        {
            TrackCheckpoints.instance.PlayerThroughCheckpoint(this);
            //trackCheckpoints.PlayerThroughCheckpoint(this);
            Debug.Log("Player detected");
        }

    }

    /*
    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints)
    {
        this.trackCheckpoints = trackCheckpoints;
    }*/
}
