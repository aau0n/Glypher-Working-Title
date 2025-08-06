using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerBar : MonoBehaviour
{
    public float maxTime = 80f;
    private float currentTime;

    public Slider timerSlider;
    public TextMeshProUGUI timerText;

    private bool isBlinking = false;
    private float blinkSpeed = 6f;

    // 색상 설정
    public Color normalTextColor = new Color32(0xDE, 0x82, 0xFF, 255); // #DE82FF
    public Color warningTextColor = new Color32(0xFF, 0x99, 0x00, 255); // #FF9900

    void Start()
    {
        currentTime = maxTime;

        if (timerSlider != null)
        {
            timerSlider.maxValue = maxTime;
            timerSlider.value = maxTime;
        }

        if (timerText != null)
        {
            timerText.color = normalTextColor;
        }
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            if (timerSlider != null)
            {
                timerSlider.value = currentTime;
            }

            if (timerText != null)
            {
                // 남은 시간 숫자 표시 (초 없음)
                timerText.text = Mathf.CeilToInt(currentTime).ToString();
            }

            // 30초 이하부터 깜빡임 시작
            if (currentTime <= 30f)
            {
                isBlinking = true;
            }

            // 텍스트 색상 깜빡임
            if (isBlinking && timerText != null)
            {
                float t = 0.5f + 0.5f * Mathf.Sin(Time.time * blinkSpeed);
                timerText.color = Color.Lerp(normalTextColor, warningTextColor, t);
            }
        }
        else
        {
            if (timerSlider != null)
            {
                timerSlider.value = 0;
                timerSlider.gameObject.SetActive(false); // 슬라이더 숨김
            }

            if (timerText != null)
            {
                timerText.text = "0";
                timerText.color = warningTextColor;
            }
        }
    }
}
