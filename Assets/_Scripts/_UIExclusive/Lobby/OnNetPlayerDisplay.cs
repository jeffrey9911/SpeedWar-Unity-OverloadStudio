using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class OnNetPlayerDisplay : MonoBehaviour
{
    public TMP_Text idText;
    public TMP_Text statusText;

    public short displayedPlayerID;
    public string dispalyedName;

    public static void DisplayPlayer(OnNetPlayerDisplay instance, ref short pID, ref string pName)
    {
        instance.displayedPlayerID = pID;
        instance.dispalyedName = pName;
        instance.idText.text = pName + "#" + pID;
        instance.statusText.text = "Status: Online - Lobby";
    }

    public static void UpdateStatus(OnNetPlayerDisplay instance, ref short levelID)
    {

    }
}
