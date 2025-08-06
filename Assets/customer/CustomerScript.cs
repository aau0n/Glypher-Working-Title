using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CustomerScript : MonoBehaviour
{
    public GameObject customerNormal, customerSmile; // 손님 기본 모습, 웃는 애니메이션 에셋
    public TextTyper typer; // Inspector에서 TextTyper 스크립트가 붙은 오브젝트 연결
    public AudioClip doorEffectClip; // 문이 여닫히는 효과음
    public AudioSource audioSource;
    public float fadeDuration = 1f; // 손님이 서서히 등장하는 시간(초)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 처음엔 손님 안 보이게
        customerNormal.SetActive(false);
        customerSmile.SetActive(false);

        StartCoroutine(Sequence()); // 문이 여닫히는 효과음과 함께 손님 모습 등장

        
    }

    IEnumerator Sequence()
    {
        PlayEffect(); // 문이 여닫히는 효과음 재생

        yield return new WaitForSeconds(2f); // 원하는 시간 대기

        StartCoroutine(FadeIn()); // 손님이 서서히 나타남

        if (typer != null)
            typer.StartTyping("안녕 글리퍼! <color=#FF9900>글리피우스</color>에 온 걸 환영해. ");
        

    }

    IEnumerator FadeIn()
    {
        if (!customerNormal.gameObject.activeSelf)
            customerNormal.gameObject.SetActive(true);

        // 알파 0(완전 투명)으로 초기화
        Color color = customerNormal.color;
        color.a = 0f;
        customerNormal.color = color;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, timer / fadeDuration);
            customerNormal.color = color;
            yield return null;
        }
        // 최종적으로 완전히 불투명하게
        color.a = 1f;
        customerNormal.color = color;
    }

    void Awake()
    {
        // AudioSource가 없다면 자동으로 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayEffect()
    {
        // 한 번만 플레이(중첩 허용)
        audioSource.PlayOneShot(effectClip);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
