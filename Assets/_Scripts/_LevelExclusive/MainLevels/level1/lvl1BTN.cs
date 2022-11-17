using Photon.Pun.Demo.PunBasics;
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
        GameplayManager.instance.setData(GameplayManager.instance.selectedMode, 1f);
        SceneManager.LoadScene("MainSelect");
    }

    public void MultPlayOnClick()
    {
        GameplayManager.instance.setData(GameplayManager.instance.selectedMode, 2f);
        SceneManager.LoadScene("Multiplayer-Test-Login");
    }


}
