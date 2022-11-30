using Photon.Pun.Demo.Asteroids;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Photon.Pun;
using Photon.Realtime;

public class lvl1BTN : MonoBehaviourPunCallbacks
{

    [SerializeField] private TMP_Text _buttonText;

    [SerializeField] private RectTransform _PNLCON;

    [SerializeField] private RectTransform _PNLRoomCon;

    [SerializeField] private TMP_InputField _playerName;
    [SerializeField] private TMP_InputField _roomName;

    public void SinglePlayOnClick()
    {
        GameplayManager.instance.setData(GameplayManager.instance.selectedMode, 1f);
        SceneManager.LoadScene("MainSelect");
    }

    public void MultPlayOnClick()
    {
        GameplayManager.instance.setData(GameplayManager.instance.selectedMode, 2f);
        _PNLCON.gameObject.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        _PNLCON.gameObject.SetActive(false);
        _PNLRoomCon.gameObject.SetActive(true);
    }

    public void connectRoom()
    {
        PhotonNetwork.NickName = _playerName.text;
        PhotonNetwork.JoinRoom(_roomName.text);
    }

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene("MainSelect");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        createRoom(_roomName.text);
    }

    private void createRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }
}
