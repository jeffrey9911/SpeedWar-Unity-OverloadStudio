using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public TMP_InputField _messageInput;

    static Transform chatRoom;
    static Transform playerList;

    private void Start()
    {
        chatRoom = GameObject.FindGameObjectWithTag("ChatRoom").transform;
        playerList = GameObject.FindGameObjectWithTag("PlayerList").transform;
    }

    public static void AddMessage(ref string msgToAdd)
    {
        GameObject msg = Instantiate(Resources.Load<GameObject>("PNL_Message"), chatRoom);
        msg.transform.Find("Message").GetComponent<TMP_Text>().text = msgToAdd;
    }

    public void SendMsgOnClick()
    {
        string msgToSend = _messageInput.text;
        NetworkManager.SendMessage(ref msgToSend);
        _messageInput.text = "";
    }

    public static void RefreshPlayerList()
    {
        foreach(Transform onDPlayer in playerList)
        {
            Destroy(onDPlayer.gameObject);
        }

        foreach(NetPlayer netPlayer in NetworkManager.playerDList.Values)
        {
            GameObject addedPlayer = Instantiate(Resources.Load<GameObject>("PNL_Player"), playerList);
            OnNetPlayerDisplay.DisplayPlayer(addedPlayer.GetComponent<OnNetPlayerDisplay>(), ref netPlayer.playerID, ref netPlayer.playerName, ref netPlayer.playerLevelID);
        }
    }

    public void NetStartOnClick()
    {
        short lvid = 1;
        NetworkManager.UpdateLocalLevelID(ref lvid);
        NetworkManager.isJoiningRoom = false;
        SceneManager.LoadScene("MainSelect");
    }
}
