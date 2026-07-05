using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalVar : MonoBehaviour
{
    public static int score = 0;
    public static int difficulty = 0; public static int highScore = 10000;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    void UpdateScores()
    {
        scoreText.text = score.ToString();
        highScoreText.text = "HI " + highScore.ToString();

    }

    // S T A R T
    void Start()
    {
        UpdateScores();
    }

    // U P D A T E
    void Update()
    {
        //Test code to make sure score can update
        //Replace with actual conditions that award points
        //score += 1;

        //Update high score in real time
        if (score > highScore)
        {
            highScore = score;
        }

        UpdateScores();
    }
}