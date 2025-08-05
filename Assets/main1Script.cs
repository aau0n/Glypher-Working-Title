using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public GameObject tattoo1, tattoo2, tattoo3, tattoo4; // 화면 상단 타투 도안
    public GameObject tapBar1, tapBarLine1, tapBar2, tapBarLine2; // 1: 왼쪽 타이밍바, 2: 오른쪽 타이밍바
    public GameObject tattooDesignBg; // 스텐실 배경
    public GameObject stencil1, stencil2; // 스텐실 붙이기, 떼기
    public GameObject nextButton; // next 버튼
    public GameObject speechBallon; // 말풍선
    public Button tattooButton; // inspector에서 타투 도안 버튼 연결
    public static int timingBarSuccessCount = 0; // 타이밍 바 맞추기 결과 전달용 변수. 맞춘 타이밍 바 개수가 저장됨
    private bool tapBar1Success = false;
    private bool tapBar2Success = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 시작할 때 안 보이게
        tapBar1.SetActive(false); // 왼쪽 타이밍바
        tapBarLine1.SetActive(false); // 왼쪽 타이밍바 라인
        tapBar2.SetActive(false); // 오른쪽 타이밍바
        tapBarLine2.SetActive(false); // 오른쪽 타이밍바 라인
        stencil1.SetActive(false); // 스텐실 붙이기
        stencil2.SetActive(false); // 스텐실 떼기
        nextButton.SetActive(false); // next 버튼

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
        ShowTapBar1();
    }

    public void OnStencil()
    {
        ShowTapBar2();
    }

    public void OnTapBar2Stopped()
    {
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
