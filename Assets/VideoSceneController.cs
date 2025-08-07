using System.Collections;       // ← 이게 꼭 필요합니다!
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoSceneController : MonoBehaviour
{
    public Image fadePanel;              // 검정 패널 (UI Image)
    public float fadeDuration = 4f;      // 페이드 인/아웃 시간
    public VideoPlayer videoPlayer;      // Video Player
    public string nextSceneName;         // 다음 씬 이름

    private bool isFadingOut = false;

    void Start()
    {
        fadePanel.color = new Color(0, 0, 0, 1); // 완전 검정으로 시작
        StartCoroutine(FadeIn());

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    IEnumerator FadeIn()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = 1 - (t / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, 0); // 완전히 밝아짐
        videoPlayer.Play();
        StartCoroutine(CheckVideoForFadeOut());
    }

    IEnumerator CheckVideoForFadeOut()
    {
        while (videoPlayer.isPlaying)
        {
            double timeLeft = videoPlayer.length - videoPlayer.time;

            if (timeLeft < fadeDuration && !isFadingOut)
            {
                isFadingOut = true;
                StartCoroutine(FadeOut());
            }

            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeDuration;
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, 1); // 완전 검정
        LoadNextScene();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        if (!isFadingOut)
        {
            StartCoroutine(FadeOut());
        }
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
