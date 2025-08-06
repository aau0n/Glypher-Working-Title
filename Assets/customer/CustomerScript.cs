using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;


public class CustomerScript : MonoBehaviour
{
    public GameObject customerNormal, customerSmile; // 손님 기본 모습, 웃는 애니메이션 에셋
    public TextTyper typer; // Inspector에서 TextTyper 스크립트가 붙은 오브젝트 연결
    public TMP_Text txt_name; // 손님 이름 TMP 텍스트
    public AudioClip doorEffectClip; // 문이 여닫히는 효과음
    public AudioSource audioSource;
    public float fadeDuration = 3f; // 손님이 서서히 등장하는 시간(초)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 처음엔 손님, 이름 안 보이게
        customerNormal.SetActive(false);
        customerSmile.SetActive(false);
        txt_name.gameObject.SetActive(false);

        StartCoroutine(Sequence()); // 문이 여닫히는 효과음과 함께 손님 모습 등장

        
    }

    IEnumerator Sequence()
    {
        yield return new WaitForSeconds(2f); // 원하는 시간 대기

        PlayEffect(); // 문이 여닫히는 효과음 재생

        yield return new WaitForSeconds(3.7f); // 원하는 시간 대기

        StartCoroutine(FadeIn()); // 손님이 서서히 나타남

        yield return new WaitForSeconds(fadeDuration);

        if (typer != null)
            typer.StartTyping("안녕 글리퍼! <color=#FF9900>글리피우스</color>에 온 걸 환영해. ");
        

    }

    IEnumerator FadeIn()
    {
        // 비활성화 상태라면 먼저 활성화부터
        if (!customerNormal.activeSelf)
            customerNormal.SetActive(true);
        if (!txt_name.gameObject.activeSelf)
            txt_name.gameObject.SetActive(true);


        SpriteRenderer sr = customerNormal.GetComponent<SpriteRenderer>();

        Color colorImage = sr.color;
        Color colorText = txt_name.color;

        colorImage.a = 0f;
        colorText.a = 0f;

        sr.color = colorImage;
        txt_name.color = colorText;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(0, 1, timer / fadeDuration);

            colorImage.a = a;
            colorText.a = a;

            sr.color = colorImage;
            txt_name.color = colorText;

            yield return null;
        }
        colorImage.a = 1f;
        colorText.a = 1f;

        sr.color = colorImage;
        txt_name.color = colorText;
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
        audioSource.PlayOneShot(doorEffectClip);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
