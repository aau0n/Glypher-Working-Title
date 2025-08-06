using System.Collections;
using UnityEngine;
using TMPro;

public class Typing : MonoBehaviour
{
    public TMP_Text dialogueText;   // Inspector에서 TextMeshPro 오브젝트 할당
    public float typingSpeed = 0.05f; // 한 글자 나오는 시간 간격

    // 미리 넣어둘 문장 배열
    public string[] sentences =
    {
        "2500년, 지구는 더이상 살 수 없는 행성이 되었고, 인류는 제 2의 행성 '글리피우스'로 이주했다.",
        "하지만 새로운 행성들은 이미 외계 문명들이 점령하고 있었고,",
        "외계종족과 인간이 공존하려 노력하는 과정에서  사회적 질서를 위해 '글리프'라는 낙인을 신체에 새기는 문화가 정착됐다.",

        "'글리퍼'로 불리는 직업이 나타났고, 이 세계에서 글리프를 새기고 기록하는 일을 맡게 되었다.",

        "글리퍼는 다양한 종족의 이야기를 기록하고, 그들의 글리프를 해석하며 하루하루를 보내고 있다."
    };

    private int currSentence = 0;
    private bool isTyping = false;
    private bool waitingForClick = false;

    public GameObject op1, op2, op3;

    // Object 언제 등장시킬지 인덱스 (0부터 시작)
    public int op2ActivateAfterSentence = 3; // 3번(네 번째) 문장과 함께
    public int op3ActivateAfterSentence = 4; // 4번(다섯 번째) 문장과 함께


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currSentence = 0;
        if (op2 != null) op2.SetActive(false);
        if (op3 != null) op3.SetActive(false);
        ShowCurrentSentence();
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingForClick && Input.GetMouseButtonDown(0))
        {
            NextSentence();
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
        dialogueText.text = "";

        // 띄어쓰기 두 번이면 줄바꿈으로 바꿈
        if (message.Contains("  ")) message = message.Replace("  ", "\n");

        foreach (char c in message)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }

        isTyping = false;
        waitingForClick = true;
    }

    void NextSentence()
    {
        waitingForClick = false;

        currSentence++;

        // 문장 번호에 따라 오브젝트 등장
        if (currSentence == op2ActivateAfterSentence && op2 != null)
        {
            op2.SetActive(true);
        }
        if (currSentence == op3ActivateAfterSentence && op3 != null)
        {
            op3.SetActive(true);
        }

        if (currSentence < sentences.Length)
        {
            ShowCurrentSentence();
        }
        else
        {
            // dialogueText.text = ""; // 혹은 마지막 문장 유지
        }
    }
}
