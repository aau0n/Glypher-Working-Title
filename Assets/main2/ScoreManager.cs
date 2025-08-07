using UnityEngine;

public static class ScoreManager
{
    public static int score = 0;
    public static int total = 0;

    // 4번째 씬에서 설정할 성공 횟수
    public static int timingBarSuccessCount = 0;

    public static float GetPercentage()
    {
        if (total == 0) return 0f;
        return (float)score / total * 100f;
    }

    // 최종 점수 계산 (score + 타이밍 점수 * 10)
    public static int GetFinalScore()
    {
        Debug.Log(score);
        return score + timingBarSuccessCount * 10;

    }
}
