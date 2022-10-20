using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    //private TrackCheckpoints trackCheckpoints;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.transform.parent.parent.tag);
       if (other.transform.parent.parent.tag == "Player")
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
