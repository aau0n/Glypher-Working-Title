using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class MutualScript6_4 : MonoBehaviour
{
    public GameObject customerNormal, customerSmile, customerDifficult; // 손님 기본 모습, 웃는 애니메이션, 난처한 모습 에셋
    public TextTyper typer; // Inspector에서 TextTyper 스크립트가 붙은 오브젝트 연결
    public float typingSpeed = 0.05f; // 한 글자 나오는 시간 간격
    // 미리 넣어둘 문장 배열
    public string[] sentences =
    {
        "어찌됐건, 오늘 수고 많았어.\n작업을 하면서 필요한 자원이 고갈되지 않게 조심하고.",
        "이건 너한테만 알려주는 건데, 근처 폐쇄된 창고에 글리퍼에게 필요한 자원이 많은 모양이야.\n이미 알 사람들은 다 알게 되어서, 경비도 삼엄해진 모양이니 주의해.",
        "글리퍼, 네가 와서 기쁘다. 알다시피 이곳에 있는 머메이드의 수는 많지 않거든.\n네가 처음 왔을 때, 나랑 비슷하게 생겨서 무척 놀랐어. 너는 분명 인간이라고 했지?\n글리피우스에는 무척 다양한 종족들이 있어서, 네게 맞는 시설도 얼마 없을거야.",
        "혹시 필요한 게 있다면 말해. 우린 비슷하니까, 내가 도움을 줄 수 있을지도 몰라.\n다음에는 네 고향에 대한 이야기를 좀 더 들려줘. 잘 자, 글리퍼!"
    };

    // Object 언제 등장시킬지 인덱스 (0부터 시작)
    public int difficultActivateAfterSentence = 1; // 1번(두 번째) 문장과 함께
    public int normalActivateAfterSentence = 2; // 2번(세 번째) 문장과 함께
    public int smileActivateAfterSentence = 3; // 3번(네 번째) 문장과 함께

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
        customerDifficult.SetActive(false);
        customerSmile.SetActive(false);

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

        if (currSentence == difficultActivateAfterSentence && customerDifficult != null)
        {
            customerDifficult.SetActive(true);
            customerNormal.SetActive(false);
            customerSmile.SetActive(false);
        }
        if (currSentence == normalActivateAfterSentence && customerNormal != null)
        {
            customerDifficult.SetActive(false);
            customerNormal.SetActive(true);
            customerSmile.SetActive(false);
        }
        if (currSentence == smileActivateAfterSentence && customerSmile != null)
        {
            customerDifficult.SetActive(false);
            customerNormal.SetActive(false);
            customerSmile.SetActive(true);
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
