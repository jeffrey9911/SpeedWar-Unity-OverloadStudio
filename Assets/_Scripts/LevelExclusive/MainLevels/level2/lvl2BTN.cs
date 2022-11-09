using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class lvl2BTN : MonoBehaviour
{
    public void car1OnClick()
    {
        if(SceneConnect.instance.findCntxt("kartID") != null)
        {
            SceneConnect.instance.changeCntxt("kartID", "0");
        }
        else
        {
            SceneConnect.instance.addCntxt("kartID", "0");
        }
        
        Debug.Log("KART 1 SELECTED!");
    }

    public void car2OnClick()
    {
        if (SceneConnect.instance.findCntxt("kartID") != null)
        {
            SceneConnect.instance.changeCntxt("kartID", "1");
        }
        else
        {
            SceneConnect.instance.addCntxt("kartID", "1");
        }
        Debug.Log("KART 2 SELECTED!");
    }

    public void startOnClick()
    {
        if(SceneConnect.instance.findCntxt("isOnNetwork") == "false")
        {
            SceneManager.LoadScene("Level1");
        }
        else if(SceneConnect.instance.findCntxt("isOnNetwork") == "true")
        {
            PhotonNetwork.LoadLevel("Level1");
        }

    }
}
