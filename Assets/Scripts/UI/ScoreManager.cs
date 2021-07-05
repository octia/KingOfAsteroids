using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]private int score = 0; // serialized to show in inspector
    private int _oldScore;

    private Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<Text>();
        UpdateScoreText();
    }

    // Update is called once per frame
    void Update()
    {
        if (score != _oldScore)
        {
            UpdateScoreText();
        }
    }

    public void IncreaseScore(int amount = 1)
    {
        score += amount;
    }
    public void ResetScore()
    {
        score = 0;
    }

    void UpdateScoreText()
    {
        _oldScore = score;
        scoreText.text = "Score: " + score.ToString();
    }

}
