using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Typing : MonoBehaviour
{
    public TMP_Text dialogueText;   // Inspector에서 TextMeshPro 오브젝트 할당
    public float typingSpeed = 0.05f; // 한 글자 나오는 시간 간격

    public AudioSource audioSource;        // 효과음 재생용 AudioSource
    public AudioClip typingSoundClip;      // 한 글자 출력시 재생할 효과음
    public AudioClip sentenceEndSoundClip; // 텍스트 넘길 때 재생할 효과음

    // 미리 넣어둘 문장 배열
    public string[] sentences =
    {
        "2500년, 지구는 더이상 살 수 없는 행성이 되었고, 인류는 제 2의 행성 '글리피우스'로 이주했다.",
        "하지만 새로운 행성들은 이미 외계 문명들이 점령하고 있었고,",
        "외계종족과 인간이 공존하려 노력하는 과정에서  사회적 질서를 위해 '글리프'라는 낙인을 신체에 새기는 문화가 정착됐다.",

        "'글리퍼'라 불리는 직업이 나타나 이 세계에서 글리프를 새기고 기록하는 일을 맡게 되었다.",

        "글리퍼는 다양한 종족의 이야기를 기록하고, 그들의 글리프를 해석하며 하루하루를 보내고 있다."
    };

    private int currSentence = 0;
    private bool isTyping = false;
    private bool waitingForClick = false;

    // Object 언제 등장시킬지 인덱스 (0부터 시작)
    public int op2ActivateAfterSentence = 3; // 3번(네 번째) 문장과 함께
    public int op3ActivateAfterSentence = 4; // 4번(다섯 번째) 문장과 함께

    public GameObject op1, op2, op3;
    public GameObject triangleIndicator;        // ▽ 삼각형(이미지) 오브젝트
    public GameObject endTriangleIndicator;     // ▷ 삼각형(이미지) 오브젝트
    public string nextSceneName = "3Customer"; // 전환할 씬 이름
    private bool dialogueEnded = false;  // 마지막 문장 출력 끝났는지
    private bool readyToSwitch = false;  // endTriangleIndicator가 떴는지

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currSentence = 0;
        if (triangleIndicator != null) triangleIndicator.SetActive(false);
        if (endTriangleIndicator != null) endTriangleIndicator.SetActive(false);
        if (op2 != null) op2.SetActive(false);
        if (op3 != null) op3.SetActive(false);
        ShowCurrentSentence();
    }

    // Update is called once per frame
    void Update()
    {
        // 문장 출력 후 클릭 대기
        if (waitingForClick && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(NextSentence());
        }
        // 마지막 이미지가 뜬 상태에서 클릭하면 씬 전환
        if (readyToSwitch && Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void ShowCurrentSentence()
    {
        StartCoroutine(TypeTextOneByOne(sentences[currSentence]));
    }

    IEnumerator TypeTextOneByOne(string message)
    {
        isTyping = true;
        waitingForClick = false;
        if (triangleIndicator != null) triangleIndicator.SetActive(false);

        dialogueText.text = "";

        foreach (char c in message)
        {
            dialogueText.text += c;

            // 한 글자 출력 시 효과음 재생 (있으면)
            if (audioSource != null && typingSoundClip != null)
            {
                audioSource.PlayOneShot(typingSoundClip);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        if (currSentence < sentences.Length - 1)
        {
            // 마지막 문장 전까진 삼각형 활성화
            if (triangleIndicator != null) triangleIndicator.SetActive(true);
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
        if (triangleIndicator != null) triangleIndicator.SetActive(false);

        // 한 문장 끝났을 때 대사 넘김 효과음 재생
        if (audioSource != null && sentenceEndSoundClip != null)
        {
            audioSource.PlayOneShot(sentenceEndSoundClip);
            yield return new WaitForSeconds(0.5f);
        }

        currSentence++;

        if (currSentence == op2ActivateAfterSentence && op2 != null)
            op2.SetActive(true);
        if (currSentence == op3ActivateAfterSentence && op3 != null)
            op3.SetActive(true);

        if (currSentence < sentences.Length)
        {
            ShowCurrentSentence();
        }
    }

    IEnumerator ShowEndImageAndWaitForClick()
    {
        if (triangleIndicator != null) triangleIndicator.SetActive(false);
        if (endTriangleIndicator != null) endTriangleIndicator.SetActive(true);

        readyToSwitch = true; // 이제 클릭하면 씬 전환!

        yield return null; // 클릭 대기는 Update()에서 처리
    }
}
