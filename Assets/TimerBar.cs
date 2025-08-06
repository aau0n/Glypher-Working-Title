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
    private float blinkSpeed = 4f;

    // 🎨 색상 설정
    public Color normalTextColor = new Color32(0xDE, 0x82, 0xFF, 255); // #DE82FF
    public Color warningTextColor = new Color32(0xFF, 0x99, 0x00, 255); // #FF9900

    void Start()
    {
        currentTime = maxTime;
        timerSlider.maxValue = maxTime;
        timerSlider.value = maxTime;

        timerText.color = normalTextColor;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timerSlider.value = currentTime;

            timerText.text = Mathf.CeilToInt(currentTime).ToString();

            if (currentTime <= 30f)
            {
                isBlinking = true;
            }

            if (isBlinking)
            {
                float t = 0.5f + 0.5f * Mathf.Sin(Time.time * blinkSpeed);
                timerText.color = Color.Lerp(normalTextColor, warningTextColor, t);
            }
        }
        else
        {
            timerSlider.value = 0;
            timerText.text = "0";
            timerText.color = warningTextColor;
        }
    }
}
