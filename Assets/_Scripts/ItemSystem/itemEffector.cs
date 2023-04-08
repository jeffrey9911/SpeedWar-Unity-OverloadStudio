//using Palmmedia.ReportGenerator.Core.Reporting.Builders.Rendering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemEffector : MonoBehaviour
{
    public static void useEffect(string effectName)
    {
        switch(effectName)
        {
            case "Booster":
                performBooster();
                break;
        }
    }

    private static void performBooster()
    {
        Rigidbody playerRB = NetworkManager.localPlayer.playerTransform.GetComponent<Rigidbody>();
        playerRB.velocity += playerRB.velocity.normalized * 10.0f * Time.deltaTime;
    }
}
