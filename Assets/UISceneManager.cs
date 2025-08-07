using UnityEngine;

public class UISceneManager : MonoBehaviour
{
    public GameObject panelA; // A_UI_Panel
    public GameObject panelB; // B_UI_Panel
    public GameObject panelC; // C_UI_Panel

    void Start()
    {
        string prev = PlayerPrefs.GetString("PreviousScene");

        panelA.SetActive(false);
        panelB.SetActive(false);
        panelC.SetActive(false);

        if (prev == "6_1")
        {
            panelA.SetActive(true);
        }
        else if (prev == "6_2")
        {
            panelB.SetActive(true);
        }
        else if (prev == "6_3")
        {
            panelC.SetActive(true);
        }

        PlayerPrefs.DeleteKey("PreviousScene"); // 초기화 (선택)
    }
}
