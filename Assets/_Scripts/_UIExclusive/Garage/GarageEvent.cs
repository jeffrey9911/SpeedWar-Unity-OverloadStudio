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
        if (SceneDataManager.instance.getData(SceneData.SelectedMode) == "Offline")
        {
            SceneManager.LoadScene("Level2");
        }
        else if (SceneDataManager.instance.getData(SceneData.SelectedMode) == "Online")
        {
            //PhotonNetwork.LoadLevel("Level2");
        }
    }
}
