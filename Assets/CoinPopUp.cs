// 파일명: CoinPopUp.cs

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoinPopUp : MonoBehaviour
{
    public float delay = 1f;
    public float moveUpDistance = 100f;
    public float duration = 1f;

    private RectTransform rt;
    private CanvasGroup cg;

    void Start()
    {
        rt = GetComponent<RectTransform>();

        cg = GetComponent<CanvasGroup>();
        if (cg == null)
            cg = gameObject.AddComponent<CanvasGroup>();

        cg.alpha = 0f;

        StartCoroutine(AnimateCoin());
    }

    IEnumerator AnimateCoin()
    {
        yield return new WaitForSeconds(delay);

        Vector3 startPos = rt.anchoredPosition;
        Vector3 endPos = startPos + new Vector3(0, moveUpDistance, 0);

        float elapsed = 0f;
        cg.alpha = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            rt.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
            cg.alpha = 1f - t;

            yield return null;
        }

        cg.alpha = 0f;
        Destroy(gameObject);
    }
}
