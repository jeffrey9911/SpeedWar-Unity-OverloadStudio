using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementObserver : Observer
{
    private GameObject _player;

    private AchievementEvent _event;

    private float _playerSpeed;

    private string _itemUsed;

    public AchievementObserver(GameObject _playerToSet, AchievementEvent _eventToSet, string _itemToSet)
    {
        _player = _playerToSet;
        _event = _eventToSet;
        _itemUsed = _itemToSet;
    }

    public AchievementObserver(GameObject _playerToSet, AchievementEvent _eventToSet)
    {
        _player = _playerToSet;
        _event = _eventToSet;
    }

    public override void OnNotified()
    {
        switch(_event)
        {
            case itemPoints:
                scoreManager.instance.addScore((int)_event.pointsAchievement());
                break;

            case fastPoints:
                if(_playerSpeed > 100)
                    scoreManager.instance.addScore(_event.pointsAchievement() * _player.GetComponent<KartController>().calc_speed * 0.1f * Time.deltaTime);
                break;

            case driftPoints:
                scoreManager.instance.addScore(_event.pointsAchievement() * _player.GetComponent<KartController>().calc_speed * 0.1f * Time.deltaTime);
                break;

            case hitPoints:
                scoreManager.instance.addScore(_event.pointsAchievement() * _player.GetComponent<KartController>().calc_speed * 0.1f);
                break;

            default:
                break;

        }
    }
}
