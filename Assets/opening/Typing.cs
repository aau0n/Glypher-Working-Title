using System.Collections;
using UnityEngine;
using TMPro;

public class Typing : MonoBehaviour
{
    public TMP_Text dialogueText;   // Inspector에서 TextMeshPro 오브젝트 할당
    public float typingSpeed = 0.1f; // 한 글자 나오는 시간 간격
    public GameObject op1, op2, op3;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        op2.SetActive(false);
        op3.SetActive(false);
        StartTyping("2500년, 지구는 더이상 살 수 없는 행성이 되었고,");
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator TypeTextOneByOne(string message)
    {
        dialogueText.text = "";
        foreach (char c in message)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
    
    public void StartTyping(string message)
    {
        StartCoroutine(TypeTextOneByOne(message));
    }
}
