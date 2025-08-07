using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToUIScene : MonoBehaviour
{
    public string currentSceneCode = "6_1"; // 기본값 (필요에 따라 "6_2", "6_3"으로 변경)

    public void LoadUIScene()
    {
        PlayerPrefs.SetString("PreviousScene", currentSceneCode);
        SceneManager.LoadScene("UI_Scene");
    }
}
