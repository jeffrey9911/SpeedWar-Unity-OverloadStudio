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
        SceneDataManager.instance.setData(SceneData.SelectedMode, "Offline");
        SceneManager.LoadScene(4);
    }

    public void MultPlayOnClick()
    {
        SceneDataManager.instance.setData(SceneData.SelectedMode, "Online");
        _PNLRoomCon.gameObject.SetActive(true);
    }

    public void  ConnectOnClick()
    {
        SceneDataManager.instance.setData(SceneData.SelectedMode, "Online");
        SceneDataManager.instance.setData(SceneData.SelectedName, _playerName.text);
        SceneManager.LoadScene(4);
    }

    

    
}
