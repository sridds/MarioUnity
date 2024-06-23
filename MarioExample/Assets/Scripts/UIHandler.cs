using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;

    void Update()
    {
        int coins = LevelManager.Instance.coins;
        coinText.text = $"x0{coins.ToString("D2")}";

        timeText.text = $"TIME\n{LevelManager.Instance.timeRemaining.ToString("D3")}";
        scoreText.text = $"MARIO\n{LevelManager.Instance.score.ToString("D6")}";
    }
}
