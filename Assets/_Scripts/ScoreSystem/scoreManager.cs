using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scoreManager : MonoBehaviour
{
    public static scoreManager instance;

    private int playerScore = 0;

    private void Awake()
    {
        if(!instance)
            instance = this;
    }

    public void addScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
        Debug.Log("Points get!");
    }

    public int getScore()
    {
        return playerScore;
    }
}
