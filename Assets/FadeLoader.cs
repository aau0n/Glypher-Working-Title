using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeLoader : MonoBehaviour
{
    public Image fadePanel;        // 검은 패널 (UI Image)
    public float fadeDuration = 2f;

    public Button[] buttonsToFade; // ✅ 이 줄을 추가!

    public void LoadSceneWithFade(string sceneName)
    {
        // 버튼 비활성화 + 시각적 페이드 처리
        foreach (Button btn in buttonsToFade)
        {
            btn.interactable = false;

            CanvasGroup cg = btn.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                StartCoroutine(FadeOutUI(cg));
            }
        }

        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    IEnumerator FadeOutUI(CanvasGroup cg)
    {
        float t = 0f;
        float startAlpha = cg.alpha;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            yield return null;
        }

        cg.alpha = 0f;
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeDuration;
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, 1);
        SceneManager.LoadScene(sceneName);
    }
}
