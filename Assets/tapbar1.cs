using UnityEngine;

public class tapbar1 : MonoBehaviour
{
    public RectTransform bar;
    public float speed = 500f;
    private bool isMoving = false;
    private float direction = 1f;
    private float correctZoneMaxY = -49.0f;
    private float correctZoneMinY = -128.0f;
    public System.Action<bool> OnBarStopped;  // 성공 여부 bool 전달

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            bar.anchoredPosition += Vector2.up * direction * speed * Time.deltaTime;

            // 상하 한계값
            float upperBoundY = -13.0f;
            float lowerBoundY = -495.0f;

            // 방향 전환 및 위치 강제 제한
            if (bar.anchoredPosition.y > upperBoundY)
            {
                direction = -1f;
                var pos = bar.anchoredPosition;
                pos.y = upperBoundY; // 상한선으로 강제 이동
                bar.anchoredPosition = pos;
            }
            else if (bar.anchoredPosition.y < lowerBoundY)
            {
                direction = 1f;
                var pos = bar.anchoredPosition;
                pos.y = lowerBoundY; // 하한선으로 강제 이동
                bar.anchoredPosition = pos;
            }
        }

        // 스페이스바로 멈춤
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isMoving = false;
            CheckBarPosition();
        }
    }

    void OnEnable()
    {
        isMoving = true;
    }

    void CheckBarPosition()
    {
        float barY = bar.anchoredPosition.y;
        bool isCorrect = (barY >= correctZoneMinY && barY <= correctZoneMaxY);

        if (isCorrect)
        {
            Debug.Log("TB1 정답 영역에서 멈춤");
            // 점수 처리
        }
        else
        {
            Debug.Log("TB1 정답 영역에서 멈추지 않음");
            // 점수 처리
        }

        if (OnBarStopped != null) OnBarStopped(isCorrect);  // 성공 여부 넘김
    }
}