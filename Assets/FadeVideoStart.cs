using UnityEngine;

public class FadeVideoStart : MonoBehaviour
{
    public FadeLoader fadeLoader;       // 페이드 아웃 실행자
    public string sceneToLoad = "8Night";  // 넘어갈 씬 이름

    public void OnClickStart()
    {
        fadeLoader.LoadSceneWithFade(sceneToLoad);
    }
}
