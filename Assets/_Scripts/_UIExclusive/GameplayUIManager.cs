using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour
{
    public static GameplayUIManager instance;

    // Speedometer
    [SerializeField] private RectTransform _pointerRect;
    private float minPointer = 115;
    private float maxPointer = -35;

    // Torque
    [SerializeField] private Image _energy;

    private void Awake()
    {
        if(!instance)
        {
            instance = this;
        }
    }

    public void UpdateSpeedometer(float percent)
    {
        _pointerRect.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Lerp(minPointer, maxPointer, percent));
    }

    public void UpdateTorqueBar(float percent)
    {
        _energy.fillAmount = percent;
    }
}
