using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    //public static int score = 0;
    //public static int highScore = 10000;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    private void OnEnable()
    {
        EventManager.OnScoreChanged += UpdateScore;
        EventManager.OnHighScoreChanged += UpdateHighScore;
    }

    private void OnDisable()
    {
        EventManager.OnScoreChanged -= UpdateScore;
        EventManager.OnHighScoreChanged -= UpdateHighScore;
    }

    void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    void UpdateHighScore(int highScore)
    {
        highScoreText.text = "HI " + highScore.ToString();
    }

    void Start()
    {
        //UpdateScores();
    }
}
