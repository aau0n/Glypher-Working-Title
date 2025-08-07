using UnityEngine;
using TMPro;

public class ScoreTextDisplay : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void Start()
    {
        int totalScore = TotalScoreManager.Instance.GetTotalScore();
        scoreText.text = "+" + totalScore.ToString();
    }
}
