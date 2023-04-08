using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class OnNetPlayerDisplay : MonoBehaviour
{
    public TMP_Text idText;
    public TMP_Text statusText;

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
                break;

            case 1:
                status = "Status: Online - Creating Game";
                break;

            case 2:
                status = "Status: Online - Playing Level 2";
                break;

            case 3:
                status = "Status: Online - Playing Level 3";
                break;

            default:
                break;
        }
        instance.statusText.text = status;
    }
}
