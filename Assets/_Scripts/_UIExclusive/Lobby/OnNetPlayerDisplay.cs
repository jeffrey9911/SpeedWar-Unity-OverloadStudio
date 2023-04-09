using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class OnNetPlayerDisplay : MonoBehaviour
{
    public TMP_Text idText;
    public TMP_Text statusText;

    public Button joinButton;

    public short displayedPlayerID;
    public string displayedName;

    public static void DisplayPlayer(OnNetPlayerDisplay instance, ref short pID, ref string pName, ref short levelID)
    {
        instance.displayedPlayerID = pID;
        instance.displayedName = pName;

        instance.idText.text = string.Concat(pName, "#", pID);
        string status = "";
        switch (levelID)
        {
            case 0:
                status = "Status: Online - Lobby";
                instance.joinButton.interactable = false;
                break;

            case 1:
                status = "Status: Online - Creating Game";
                instance.joinButton.interactable = false;
                break;

            case 2:
                status = "Status: Online - Playing Level 2";
                instance.joinButton.interactable = true;
                break;

            case 3:
                status = "Status: Online - Playing Level 3";
                instance.joinButton.interactable = true;
                break;

            default:
                break;
        }
        instance.statusText.text = status;
    }

    public void JoinOnClick()
    {
        NetworkManager.isJoiningRoom = true;
        NetworkManager.joiningPlayerID = displayedPlayerID;
        short lvid = 1;
        NetworkManager.UpdateLocalLevelID(ref lvid);
        SceneManager.LoadScene("MainSelect");
    }
}
