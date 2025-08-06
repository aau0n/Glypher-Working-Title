using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float floatAmount = 2.5f;    // 위아래 움직일 거리(픽셀 등 단위)
    public float floatSpeed = 10f;      // 움직임 속도
    private Vector3 startPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float offsetY = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.localPosition = startPos + Vector3.up * offsetY;
    }
}
