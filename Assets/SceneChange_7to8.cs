using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange_7to8 : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("8Night");
        Debug.Log("next 버튼 클릭");
    }
}
