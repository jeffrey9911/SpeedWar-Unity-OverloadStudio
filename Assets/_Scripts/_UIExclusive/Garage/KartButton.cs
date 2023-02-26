using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KartButton : MonoBehaviour
{
    public KartAsset buttonKartAsset;

    [SerializeField] private TMP_Text kartText;
    [SerializeField] private Image kartImage;

    private KartAsset _kart;



    public void KartButtonSetup(KartAsset KA)
    {
        _kart = KA;
        kartText.text = KA.KartName;
        kartImage.sprite = KA.KartImage;
    }

    /*
    public void SetKartName(string kartName)
    {
        kartText.text = kartName;
    }

    public void SetKartImage(Sprite kartPic)
    {
        kartImage.sprite = kartPic;
    }*/

    public void kbtnOnClick()
    {
        Debug.Log(SceneData.SelectedKart);
        SceneDataManager.instance.setData(SceneData.SelectedKart, _kart.KartID);

        KartName _kn = GameObject.Find("KartName").GetComponent<KartName>();
        _kn.SwapMaterial(_kart.KartNameMaterial);

        KartStat _ks = GameObject.Find("KartSpawn").GetComponent<KartStat>();
        _ks.setupKart(_kart);
        
    }
}
