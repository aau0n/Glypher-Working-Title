using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange_8to9 : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("9Sleep");
        Debug.Log("next 버튼 클릭");
    }
}
