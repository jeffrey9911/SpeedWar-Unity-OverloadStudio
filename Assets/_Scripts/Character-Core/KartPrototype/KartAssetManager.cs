using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KartAssetManager : MonoBehaviour
{
    public static KartAssetManager instance;

    public List<KartAsset> kartList;

    private void Awake()
    {
        if(!instance)
            instance = this;
    }

    public KartAsset getKart(float KartID)
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


