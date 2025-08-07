using UnityEngine;

public class TotalScoreManager : MonoBehaviour
{
    public static TotalScoreManager Instance;

    public int scoreFromScene4 = 0;
    public int scoreFromScene5 = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환해도 유지됨
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }

    public int GetTotalScore()
    {
        return scoreFromScene4 + scoreFromScene5;
    }
}
