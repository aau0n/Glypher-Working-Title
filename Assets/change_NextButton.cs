using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class change_NextButton : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("Main2");
        Debug.Log("next 버튼 클릭");
    }
}
