using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Kart Asset")]
public class KartAsset : ScriptableObject
{
    public string KartName;

    public float KartID;

    public GameObject AssetPrefab;

    public Sprite KartImage;

    public float _acceleration;
    public float _maxSpeed;
    public float _drift;
    public float _control;
    public float _weight;
}
