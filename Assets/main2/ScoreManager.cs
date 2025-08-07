using UnityEngine;

public static class ScoreManager
{
    public static int score = 0;
    public static int total = 0;

    // 4��° ������ ������ ���� Ƚ��
    public static int timingBarSuccessCount = 0;

    public static float GetPercentage()
    {
        if (total == 0) return 0f;
        return (float)score / total * 100f;
    }

    // ���� ���� ��� (score + Ÿ�̹� ���� * 10)
    public static int GetFinalScore()
    {
        Debug.Log(score);
        return score + timingBarSuccessCount * 10;

    }
}
