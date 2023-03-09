using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class lvl1BTN : MonoBehaviour
{

    [SerializeField] private TMP_Text _buttonText;

    [SerializeField] private RectTransform _PNLCON;

    [SerializeField] private RectTransform _PNLRoomCon;

    [SerializeField] private TMP_InputField _playerName;
    [SerializeField] private TMP_InputField _roomName;

    public void SinglePlayOnClick()
    {
        //GameplayManager.instance.setData(GameplayManager.instance.selectedMode, 1f);
        SceneDataManager.instance.setData(SceneData.SelectedMode, "Offline");
        SceneManager.LoadScene("MainSelect");
    }

    public void MultPlayOnClick()
    {
        //GameplayManager.instance.setData(GameplayManager.instance.selectedMode, 2f);
        SceneDataManager.instance.setData(SceneData.SelectedMode, "Online");
        _PNLCON.gameObject.SetActive(true);
        //PhotonNetwork.ConnectUsingSettings();
    }

    

    

    
}
