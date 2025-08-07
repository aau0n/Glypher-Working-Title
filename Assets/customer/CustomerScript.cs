using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;


public class CustomerScript : MonoBehaviour
{
    public GameObject customerNormal, customerSmile; // 손님 기본 모습, 웃는 애니메이션 에셋
    public TextTyper typer; // Inspector에서 TextTyper 스크립트가 붙은 오브젝트 연결
    public float typingSpeed = 0.05f; // 한 글자 나오는 시간 간격
    public TMP_Text txt_name; // 손님 이름 TMP 텍스트
    public AudioClip doorEffectClip; // 문이 여닫히는 효과음
    public AudioSource audioSource;

    public AudioClip typingSoundClip;      // 한 글자 출력시 재생할 효과음
    public AudioClip sentenceEndSoundClip; // 텍스트 넘길 때 재생할 효과음

    public float fadeDuration = 3f; // 손님이 서서히 등장하는 시간(초)

    // 미리 넣어둘 문장 배열
    public string[] sentences =
    {
        "안녕! <color=#FF9900>글리피우스</color>에 온 걸 환영해.",
        "생활에는 좀 적응했니? 시간도 참 빠르다.\n네가 등장했을 때 모두가 얼마나 놀랐는지 몰라! 하늘에서 갑자기 떨어진 이방인이었잖아.",
        "어쨌든 오늘부터 너도 글리퍼네! 너의 친구로서 이것저것 알려주러 왔어.\n그럼 실력이 어떤지 한번 볼까?",
    };

    private int currSentence = 0;
    private bool isTyping = false;
    private bool waitingForClick = false;

    public GameObject nextSentence;        // V (다음 대화로) 오브젝트
    public GameObject toNextScene;     // >>NEXT (다음 씬으로) 오브젝트
    public string nextSceneName = "4Stencil"; // 전환할 씬 이름
    private bool dialogueEnded = false;  // 마지막 문장 출력 끝났는지
    private bool readyToSwitch = false;  // endTriangleIndicator가 떴는지

    // Object 언제 등장시킬지 인덱스 (0부터 시작)
    public int smileActivateAfterSentence = 1; // 1번(두 번째) 문장과 함께
    public int normalActivateAfterSentence = 2; // 2번(세 번째) 문장과 함께

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 처음엔 손님, 이름 안 보이게
        customerNormal.SetActive(false);
        customerSmile.SetActive(false);
        txt_name.gameObject.SetActive(false);
        nextSentence.SetActive(false);
        toNextScene.SetActive(false);

        currSentence = 0;
        if (nextSentence != null) nextSentence.SetActive(false);
        if (toNextScene != null) toNextScene.SetActive(false);

        StartCoroutine(Sequence()); // 문이 여닫히는 효과음과 함께 손님 모습 등장


    }

    IEnumerator Sequence()
    {
        yield return new WaitForSeconds(2f); // 원하는 시간 대기

        PlayEffect(); // 문이 여닫히는 효과음 재생

        yield return new WaitForSeconds(3.7f); // 원하는 시간 대기

        StartCoroutine(FadeIn()); // 손님이 서서히 나타남

        yield return new WaitForSeconds(fadeDuration);

        ShowCurrentSentence();

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
        // 문장 출력 후 클릭 대기
        if (waitingForClick && Input.GetMouseButtonDown(0))
        {
            if (!isTyping) // 타이핑 도중이 아닐 때만 실행
            {
                waitingForClick = false; // 클릭 대기 해제
                StartCoroutine(NextSentence());
            }
        }
        // 마지막 이미지가 뜬 상태에서 클릭하면 씬 전환
        if (readyToSwitch && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(LastSceneRoutine());
        }
    }

    IEnumerator LastSceneRoutine()
    {
        audioSource.PlayOneShot(sentenceEndSoundClip);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(nextSceneName);
    }
    
    void ShowCurrentSentence()
    {
        StartCoroutine(TypeTextOneByOne(sentences[currSentence]));
    }

    IEnumerator TypeTextOneByOne(string message)
    {
        isTyping = true;
        waitingForClick = false;
        if (nextSentence != null) nextSentence.SetActive(false);

        typer.textComponent.text = "";

        // foreach (char c in message)
        // {
        //     typer.textComponent.text += c;
        //     yield return new WaitForSeconds(typingSpeed);
        // }

        // isTyping = false;

        int i = 0;
        string currentText = "";
        float delay = typingSpeed;  // typingSpeed는 초당글자간격이므로 그대로 씁니다.

        while (i < message.Length)
        {
            if (message[i] == '<') // 태그 시작 체크
            {
                int tagEnd = message.IndexOf('>', i);
                if (tagEnd == -1)
                {
                    // 닫는 태그가 없으면 루프 종료하거나 남은 문자 처리
                    break;
                }
                // 태그 전체를 currentText에 한꺼번에 추가
                currentText += message.Substring(i, tagEnd - i + 1);
                i = tagEnd + 1; // 인덱스 이동
            }
            else if (message[i] == '\\')
            {
                if (i + 1 < message.Length)
                {
                    if (i + 1 < message.Length)
                    {
                        char nextChar = message[i + 1];

                        if (nextChar == 'n')
                        {
                            currentText += '\n'; // 줄바꿈 문자
                            yield return new WaitForSeconds(0.5f);
                        }
                        else if (nextChar == 't')
                            currentText += '\t'; // 탭 문자 예시

                        else
                        {
                            // 정의하지 않은 시퀀스는 그대로 출력
                            currentText += '\\';
                            currentText += nextChar;
                        }

                        i += 2; // '\'와 다음 문자 두 글자를 처리했으므로 인덱스 2칸 증가
                    }
                }
            }
            else
            {
                currentText += message[i];
                i++;
            }

            typer.textComponent.text = currentText; // TMP 텍스트에 현재까지 추가된 텍스트 반영

            // 한 글자 출력 시 효과음 재생 (있으면)
            if (audioSource != null && typingSoundClip != null)
            {
                audioSource.PlayOneShot(typingSoundClip);
            }

            yield return new WaitForSeconds(delay); // 타이핑 속도만큼 기다림
        }

        isTyping = false;  

        if (currSentence < sentences.Length - 1)
        {
            // 마지막 문장 전까진 삼각형 활성화
            if (nextSentence != null) nextSentence.SetActive(true);
            waitingForClick = true;
        }
        else
        {
            // 마지막 문장 끝, endImage를 띄우고 클릭을 대기!
            dialogueEnded = true;
            StartCoroutine(ShowEndImageAndWaitForClick());
        }
    }

    IEnumerator NextSentence()
    {
        waitingForClick = false;
        if (nextSentence != null) nextSentence.SetActive(false);

        // 한 문장 끝났을 때 대사 넘김 효과음 재생
        if (audioSource != null && sentenceEndSoundClip != null)
        {
            audioSource.PlayOneShot(sentenceEndSoundClip);
            // StartCoroutine(Wait05fSec());
            yield return new WaitForSeconds(0.5f);
        }

        currSentence++;

        if (currSentence == smileActivateAfterSentence && customerSmile != null)
        {
            customerSmile.SetActive(true);
            customerNormal.SetActive(false);
        }
        if (currSentence == normalActivateAfterSentence && customerNormal != null)
        {
            customerNormal.SetActive(true);
            customerSmile.SetActive(false);
        }    

        if (currSentence < sentences.Length)
        {
            ShowCurrentSentence();
        }
    }

    IEnumerator ShowEndImageAndWaitForClick()
    {
        if (nextSentence != null) nextSentence.SetActive(false);
        if (toNextScene != null) toNextScene.SetActive(true);

        readyToSwitch = true; // 이제 클릭하면 씬 전환!

        yield return null; // 클릭 대기는 Update()에서 처리
    }
}
