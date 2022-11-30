using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GarageEvent : MonoBehaviour
{
    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void startOnClick()
    {
        if (GameplayManager.instance.getData(GameplayManager.instance.selectedMode) == 1f)
        {
            SceneManager.LoadScene("Level2");
        }
        else if (GameplayManager.instance.getData(GameplayManager.instance.selectedMode) == 2f)
        {
            PhotonNetwork.LoadLevel("Level2");
        }
    }
}
