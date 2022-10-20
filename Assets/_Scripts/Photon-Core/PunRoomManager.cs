using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;



public class PunRoomManager : MonoBehaviourPunCallbacks
{
    //public InputField _roomName;

    public TMP_InputField _roomName;

    public void createRoom()
    {
        PhotonNetwork.CreateRoom(_roomName.text);
    }

    public void joinRoom()
    {
        PhotonNetwork.JoinRoom(_roomName.text);
    }

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene("MainSelect");
    }
}
