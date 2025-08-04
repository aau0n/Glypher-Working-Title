using UnityEngine;

public class timing_bar : MonoBehaviour
{
    public RectTransform bar;
    public float speed = 500f;
    private bool isMoving = true;
    private float direction = 1f;


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
            // 보드 끝에서 방향 전환 처리
            if (bar.anchoredPosition.y > 20.5 || bar.anchoredPosition.y < -478.5)
                direction *= -1f;
        }

        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스바로 멈춤
            isMoving = false;
    }
    

}