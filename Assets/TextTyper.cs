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
        int totalLength = fullText.Length;
        int charIndex = 0;
        float delay = 1f / charsPerSecond;

        while (charIndex < totalLength)
        {
            textComponent.text += fullText[charIndex];
            charIndex++;
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
