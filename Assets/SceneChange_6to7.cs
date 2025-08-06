using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange_6to7 : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("7BridgeNight");
        Debug.Log("next 버튼 클릭");
    }
}
