using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class MainScript : MonoBehaviour
{
    public GameObject normal, smile, embarrassed; // 손님 캐릭터 기본, 웃는, 난처한 장면 에셋
    public GameObject tapBar1, tapBarLine1, tapBar2, tapBarLine2; // 1: 왼쪽 타이밍바, 2: 오른쪽 타이밍바
    public GameObject stencil1, stencil2; // 스텐실 붙이기, 떼기
    public GameObject completedStencil; // 스텐실 뗀 후 진해진 도안
    public GameObject nextButton; // next 버튼
    public Button tattooButton; // inspector에서 타투 도안 버튼 연결
    public static int timingBarSuccessCount = 0; // 타이밍 바 맞추기 결과 전달용 변수. 맞춘 타이밍 바 개수가 저장됨
    private bool tapBar1Success = false;
    private bool tapBar2Success = false;
    public TextTyper typer; // Inspector에서 TextTyper 스크립트가 붙은 오브젝트 연결


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 시작할 때 안 보이게
        smile.SetActive(false); // 웃는 손님 에셋
        embarrassed.SetActive(false); // 난처한 손님 에셋
        tapBar1.SetActive(false); // 왼쪽 타이밍바
        tapBarLine1.SetActive(false); // 왼쪽 타이밍바 라인
        tapBar2.SetActive(false); // 오른쪽 타이밍바
        tapBarLine2.SetActive(false); // 오른쪽 타이밍바 라인
        stencil1.SetActive(false); // 스텐실 붙이기
        stencil2.SetActive(false); // 스텐실 떼기
        nextButton.SetActive(false); // next 버튼
        completedStencil.SetActive(false); // 스텐실 뗀 후 진해진 도안

        StartCoroutine(IntroSequence());

        tattooButton.onClick.AddListener(OnTattooSelected);

        tapbar1 tapbar1Script = tapBarLine1.GetComponent<tapbar1>();
        if (tapbar1Script != null)
        {
            tapbar1Script.OnBarStopped = (bool success) =>
            {
                tapBar1Success = success;
                UpdateTimingBarSuccessCount();
                ShowTapBar2();  // 첫 번째 타이밍 바 멈추면 두 번째 바 활성화
            };
        }

        tapbar2 tapbar2Script = tapBarLine2.GetComponent<tapbar2>();
        if (tapbar2Script != null)
        {
            tapbar2Script.OnBarStopped = (bool success) =>
            {
                tapBar2Success = success;
                UpdateTimingBarSuccessCount();
                OnTapBar2Stopped(); // 새 버튼 활성화 등 추가 동작
            };
        }
    }

    IEnumerator IntroSequence()
    {
        if (typer != null)
            typer.StartTyping("보통은 손님이 도안을 고르지만,\n오늘은 어쩐지 네가 추천해준 도안이 끌려.");
        yield return new WaitForSeconds(4f); // 2.5초 등 원하는 시간 대기

        smile.SetActive(true);
        normal.SetActive(false);
        if (typer != null)
            typer.StartTyping("오른쪽에서 나를 위한\n도안을 추천해줄래?");
    }

    public void ShowTapBar1()
    {
        Debug.Log("ShowTapBar1 호출됨");
        tapBar1.SetActive(true);
        tapBarLine1.SetActive(true);
        stencil1.SetActive(true);
    }

    public void ShowTapBar2()
    {
        Debug.Log("ShowTapBar2 호출됨");
        tapBar2.SetActive(true);
        tapBarLine2.SetActive(true);
        stencil1.SetActive(false);
        stencil2.SetActive(true);
    }

    public void OnTattooSelected()
    {
        Debug.Log("타투 도안 선택됨");

        normal.SetActive(true);
        smile.SetActive(false);

        StartCoroutine(OnTattooSelectedRoutine());

        
    }

    private IEnumerator OnTattooSelectedRoutine()
    {
        if (typer != null)
            typer.StartTyping("좋은 취향이네. 소질이 있어!");

        yield return new WaitForSeconds(2f);

        if (typer != null)
            typer.StartTyping("이제 <color=#FF9900>스페이스 바</color>를\n타이밍에 맞게 눌러서 도안을 붙였다 떼봐!");
        
        yield return new WaitForSeconds(2f);
        
        ShowTapBar1();
    }

    public void OnStencil()
    {
        ShowTapBar2();
    }

    public void OnTapBar2Stopped()
    {
        completedStencil.SetActive(true);

        if (typer != null)
        {
            if (timingBarSuccessCount == 2)
            {
                smile.SetActive(true);
                normal.SetActive(false);
                typer.StartTyping("잘 하는 걸!\n이제 본격적인 작업으로 들어갈 수 있겠어.");
            }
            else if (timingBarSuccessCount == 1)
            {
                smile.SetActive(true);
                normal.SetActive(false);
                typer.StartTyping("괜찮아.\n그래도 처음 치고는 잘 하는데?");
            }
            else
            {
                embarrassed.SetActive(true);
                normal.SetActive(false);
                typer.StartTyping("집중해, 글리퍼!");
            }
        }
        

        if (nextButton != null)
        {
            nextButton.SetActive(true);
        }
    }

    void UpdateTimingBarSuccessCount()
    {
        timingBarSuccessCount = 0;
        if (tapBar1Success) timingBarSuccessCount++;
        if (tapBar2Success) timingBarSuccessCount++;

        Debug.Log("성공한 타이밍 바 개수: " + timingBarSuccessCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
