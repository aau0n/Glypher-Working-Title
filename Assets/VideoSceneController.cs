using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoSceneController : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDuration = 4f;
    public VideoPlayer videoPlayer;
    public string nextSceneName;

    // 첫 번째 텍스트 그룹
    public CanvasGroup creditTextGroup1;
    public CanvasGroup creditTextGroup2;

    // 두 번째 텍스트 그룹
    public CanvasGroup creditTextGroup3;
    public CanvasGroup creditTextGroup4;

    // 타이밍 설정
    public float creditFadeInDuration = 1f;
    public float creditShowTime = 1.5f;
    public float creditFadeOutDuration = 1f;

    private bool isFadingOut = false;

    void Start()
    {
        fadePanel.color = new Color(0, 0, 0, 1);
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

        fadePanel.color = new Color(0, 0, 0, 0);
        videoPlayer.Play();

        // 크레딧 텍스트 표시 시작
        StartCoroutine(ShowCredits());

        StartCoroutine(CheckVideoForFadeOut());
    }

    IEnumerator ShowCredits()
{
    // 🎬 영상 시작 후 0.3초 대기 → 첫 번째 그룹 더 빠르게 등장
    yield return new WaitForSeconds(0.1f);

    // 첫 번째 그룹 동시에 등장
    yield return StartCoroutine(FadeCanvasGroupsIn(creditTextGroup1, creditTextGroup2));

    // 유지
    yield return new WaitForSeconds(creditShowTime);

    // 첫 번째 그룹 동시에 사라짐
    yield return StartCoroutine(FadeCanvasGroupsOut(creditTextGroup1, creditTextGroup2));

    // 🕒 여유를 두고 0.5초 대기
    yield return new WaitForSeconds(1f);

    // 두 번째 그룹 동시에 등장
    yield return StartCoroutine(FadeCanvasGroupsIn(creditTextGroup3, creditTextGroup4));

    // 유지
    yield return new WaitForSeconds(creditShowTime);

    // 두 번째 그룹 동시에 사라짐
    yield return StartCoroutine(FadeCanvasGroupsOut(creditTextGroup3, creditTextGroup4));
}


    IEnumerator FadeCanvasGroupsIn(CanvasGroup cg1, CanvasGroup cg2)
    {
        float t = 0f;
        while (t < creditFadeInDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / creditFadeInDuration);
            cg1.alpha = alpha;
            cg2.alpha = alpha;
            yield return null;
        }
        cg1.alpha = 1f;
        cg2.alpha = 1f;
    }

    IEnumerator FadeCanvasGroupsOut(CanvasGroup cg1, CanvasGroup cg2)
    {
        float t = 0f;
        while (t < creditFadeOutDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / creditFadeOutDuration);
            cg1.alpha = alpha;
            cg2.alpha = alpha;
            yield return null;
        }
        cg1.alpha = 0f;
        cg2.alpha = 0f;
    }

    IEnumerator CheckVideoForFadeOut()
    {
        while (videoPlayer.isPlaying)
        {
            double timeLeft = videoPlayer.length - videoPlayer.time;

            if (timeLeft <= 2f && !isFadingOut)
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

        fadePanel.color = new Color(0, 0, 0, 1);
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
