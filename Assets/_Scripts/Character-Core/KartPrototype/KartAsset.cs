using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Kart-Asset")]
public class KartAsset : ScriptableObject
{
    public string KartName;

    public string KartID;
     
    public GameObject AssetPrefab;

    public Sprite KartImage;

    // Kart Stats
    [Header("Kart PentaStats")]
    public float _acceleration;
    public float _maxSpeed;
    public float _drift;
    public float _control;
    public float _weight;
}
