using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger_5to6 : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("6_1Success");
        Debug.Log("next 버튼 클릭");
    }
}
