using TMPro;
using UnityEngine;
using System.Collections;

public class TextTyper : MonoBehaviour
{
    public TMP_Text textComponent;         // 타이핑할 TMP 컴포넌트, Inspector에서 반드시 연결
    public float charsPerSecond = 20f;     // 초당 글자수(타자 속도)
    private Coroutine typingCoroutine;

    public void StartTyping(string text)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    private IEnumerator TypeText(string fullText)
    {
        textComponent.text = "";
        int i = 0;
        string currentText = "";
        float delay = 1f / charsPerSecond;

        while (i < fullText.Length)
        {
            if (fullText[i] == '<') // 태그가 시작되면
            {
                int tagEnd = fullText.IndexOf('>', i);
                if (tagEnd == -1)
                {
                    // 태그 닫힘이 없으면 그냥 종료 혹은 남은 문자 처리
                    break;
                }
                // 태그 전체를 통째로 추가
                currentText += fullText.Substring(i, tagEnd - i + 1);
                i = tagEnd + 1;
            }
            else
            {
                currentText += fullText[i];
                i++;
            }

            textComponent.text = currentText;
            yield return new WaitForSeconds(delay);
        }

        typingCoroutine = null;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
