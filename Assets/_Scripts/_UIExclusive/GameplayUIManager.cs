using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUIManager : MonoBehaviour
{
    public static GameplayUIManager instance;

    // Speedometer
    [SerializeField] private RectTransform _pointerRect;
    private float minPointer = 296;
    private float maxPointer = 143;

    // Torque
    [SerializeField] private Image _energy;
    [SerializeField] private TextMeshProUGUI _speedValue;
    [SerializeField] private TextMeshProUGUI _gTimer;
    [SerializeField] private TextMeshProUGUI _gScore;
    [SerializeField] private TextMeshProUGUI _netLatency;

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

    public void UpdateSpeedValue(int speed)
    {
        
        _speedValue.text = speed.ToString();
    }

    public void UpdateGameTime(float gameTime)
    {
        int minutes = Mathf.FloorToInt(gameTime / 60f);
        int seconds = Mathf.FloorToInt(gameTime - minutes * 60);
        int milliseconds = Mathf.FloorToInt((gameTime - Mathf.Floor(gameTime)) * 100f);
        string gTime = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        _gTimer.text = gTime;
    }

    public void UpdateScore(int score)
    {
        string gScore = string.Format("{0:00000}", score);
        _gScore.text = gScore;
    }

    public void UpdateLatency(float latency)
    {
        if(NetworkManager.isOnNetwork)
        {

            _netLatency.text = ((int)(latency * 1000)).ToString() + "ms";
        }
    }

}
