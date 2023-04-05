using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class scoreManager : MonoBehaviour
{
    public static scoreManager instance;

    private float playerScore = 0;

    private float dynamicScore = 0;

    private bool isScoreChanged = false;

    private void Awake()
    {
        if(!instance)
            instance = this;
    }

    public void addScore(float scoreToAdd)
    {
        playerScore += scoreToAdd;
        isScoreChanged = true;
        //Debug.Log("Points get!: [" + playerScore + "] + [" + scoreToAdd + "]");
    }

    public int getScore()
    {
        return (int)playerScore;
    }

    private void FixedUpdate()
    {
        if(isScoreChanged)
        {
            dynamicScore = Mathf.Lerp(dynamicScore, playerScore, Time.deltaTime);
            GameplayUIManager.instance.UpdateScore((int)dynamicScore);
            isScoreChanged = Mathf.Approximately(dynamicScore, playerScore);
        }
    }
}
