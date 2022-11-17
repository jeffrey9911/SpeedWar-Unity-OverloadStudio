using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class lvl2BTN : MonoBehaviour
{
    public void car1OnClick()
    {
        GameplayManager.instance.setData(GameplayManager.instance.selectedCar, 1.01f);
        
        Debug.Log("KART 1 SELECTED!");
    }

    public void car2OnClick()
    {
        GameplayManager.instance.setData(GameplayManager.instance.selectedCar, 1.01f);
        Debug.Log("KART 2 SELECTED!");
    }

    public void startOnClick()
    {
        if(GameplayManager.instance.getData(GameplayManager.instance.selectedMode) == 1f)
        {
            SceneManager.LoadScene("Level1");
        }
        else if(GameplayManager.instance.getData(GameplayManager.instance.selectedMode) == 2f)
        {
            PhotonNetwork.LoadLevel("Level1");
        }

    }
}
