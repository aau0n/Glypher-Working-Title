using UnityEngine;
using System.Collections;

public class NeonBlink : MonoBehaviour
{
    // 깜빡임 패턴 시간 (초) 배열, 0~N까지 반복 실행
    // 예: 0.2초 꺼짐, 0.2초 켜짐, 0.2초 꺼짐, 0.6초 켜짐 ...
    public float[] blinkPattern = new float[] {
        0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.6f, 0.4f,
        0.2f, 0.2f, 0.2f, 0.2f, 0.6f, 0.8f,
        0.8f, 0.6f, 0.8f
    };

    private SpriteRenderer spriteRenderer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component missing!");
            enabled = false;
            return;
        }

        spriteRenderer.enabled = true;  // 켜져 있는 상태로 시작
        StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
{
    int count = blinkPattern.Length;
    for (int i = 0; i < count; i++)
    {
        yield return new WaitForSeconds(blinkPattern[i]);
        spriteRenderer.enabled = !spriteRenderer.enabled;  // 또는 image.enabled = !image.enabled;
    }
    // 패턴이 끝난 후 멈추기 (반복하지 않고 종료)
}


    // Update is called once per frame
    void Update()
    {
        
    }
}
