using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class GlobalVar
{
    private static int score;
    private static int highScore;

    public static int difficulty = 0;

    public static bool spawnEnemies = true;
    public static bool spawnCoins = true;

    public static int Score 
    {
        get => score;
        set
        {
            if (score == value)
                return;

            score = value;
            EventManager.OnScoreChanged(score);

            if(score > highScore)
                HighScore = value;
        }
    }

    public static int HighScore 
    {
        get => highScore; 
        private set
        {
            highScore = value;
            EventManager.OnHighScoreChanged(highScore);
        }
    }
}