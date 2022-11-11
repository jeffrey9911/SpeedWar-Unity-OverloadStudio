using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class lvl1BTN : MonoBehaviour
{
    public TMP_Text _buttonText;

    private void Start()
    {
        
    }
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
