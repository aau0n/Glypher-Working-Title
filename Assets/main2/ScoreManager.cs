public static class ScoreManager
{
    public static int score = 0;
    public static int total = 0;

    public static float GetPercentage()
    {
        if (total == 0) return 0f;
        return (float)score / total * 100f;
    }
}
