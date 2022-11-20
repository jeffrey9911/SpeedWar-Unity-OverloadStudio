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

    public void SetKartName(string kartName)
    {
        kartText.text = kartName;
    }

    public void SetKartImage(Sprite kartPic)
    {
        kartImage.sprite = kartPic;
    }

    public void kbtnOnClick()
    {

    }
}
