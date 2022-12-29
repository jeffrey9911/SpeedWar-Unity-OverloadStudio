using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KartAssetManager : MonoBehaviour
{
    public List<KartAsset> kartList;

    public List<KartAsset> getKartList { get { return kartList; } }

    public KartAsset getKart(string KartID)
    {
        foreach(var kart in kartList)
        {
            if(kart.KartID == KartID)
            {
                return kart;
            }
        }
        return null;
    }
}


