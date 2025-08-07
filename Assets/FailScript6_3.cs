using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class FailScript6_3 : MonoBehaviour
{
    public TextTyper typer; // Inspector에서 TextTyper 스크립트가 붙은 오브젝트 연결
    public float typingSpeed = 0.07f; // 한 글자 나오는 시간 간격
    // 미리 넣어둘 문장 배열
    public string[] sentences =
    {
        "지인이라고 대충 해주는 거 아니지?",
        "아직 한참은 멀었어. 글리피우스 사람들이 얼마나 낙인에 예민한데!\n더 많이 연습해야 해. 알았지?"
    };

    private int currSentence = 0;
    private bool isTyping = false;
    private bool waitingForClick = false;

    public GameObject nextSentence;        // V (다음 대화로) 오브젝트
    public GameObject toNextScene;     // >>NEXT (다음 씬으로) 오브젝트
    public string nextSceneName = "7BridgeNight"; // 전환할 씬 이름
    private bool dialogueEnded = false;  // 마지막 문장 출력 끝났는지
    private bool readyToSwitch = false;  // endTriangleIndicator가 떴는지

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currSentence = 0;
        if (nextSentence != null) nextSentence.SetActive(false);
        if (toNextScene != null) toNextScene.SetActive(false);

        ShowCurrentSentence();
    }

    // Update is called once per frame
    void Update()
    {
        // 문장 출력 후 클릭 대기
        if (waitingForClick && Input.GetMouseButtonDown(0))
        {
            NextSentence();
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
        if (nextSentence != null) nextSentence.SetActive(false);

        typer.textComponent.text = "";

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
                            currentText += '\n'; // 줄바꿈 문자
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

    void NextSentence()
    {
        waitingForClick = false;
        if (nextSentence != null) nextSentence.SetActive(false);

        currSentence++;

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
