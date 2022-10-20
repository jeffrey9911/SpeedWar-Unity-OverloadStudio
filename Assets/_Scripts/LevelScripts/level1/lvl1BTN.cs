using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class lvl1BTN : MonoBehaviour
{
    public void SinglePlayOnClick()
    {
        SceneConnect.instance.addCntxt("isOnNetwork", "false");
        SceneManager.LoadScene("MainSelect");
    }

    public void MultPlayOnClick()
    {
        SceneConnect.instance.addCntxt("isOnNetwork", "true");
        SceneManager.LoadScene("Multiplayer-Test-Login");
    }

}
