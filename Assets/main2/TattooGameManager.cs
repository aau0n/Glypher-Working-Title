using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TattooGameManager : MonoBehaviour
{
    public List<Color> colors;

    public GameObject tilePinkPrefab;
    public GameObject tileGreenPrefab;
    public GameObject tileBluePrefab;
    public Transform gridParent;
    public Image Back_tattoo;
    public RectTransform cursor;

    public Image ink1; // 연두색
    public Image ink2; // 핑크색
    public Image ink3; // 보라색

    public Sprite inkPopGreen;
    public Sprite inkPopPink;
    public Sprite inkPopBlue;
    public Sprite inkBasicGreen;
    public Sprite inkBasicPink;
    public Sprite inkBasicBlue;

    // 피드백 이미지 오브젝트
    public GameObject goodImage;
    public GameObject perfectImage;
    public GameObject missImage;
    public GameObject nextButton;

    private GameObject[,] tiles = new GameObject[10, 10];
    private Color[,] correctPattern = new Color[10, 10];
    private Color[,] userInput = new Color[10, 10];

    private int currentX = 0;
    private int currentY = 0;
    private GameObject selectedPrefab = null;

    private Vector2 gridOrigin = new Vector2(0f, 0f);

    private Vector2 defaultInkPosition1;
    private Vector2 defaultInkPosition2;
    private Vector2 defaultInkPosition3;

    private Color green = new Color(0.353f, 1.000f, 0.000f, 0.502f);
    private Color blue = new Color(0.278f, 0.522f, 0.827f, 0.502f);
    private Color pink = new Color(1.000f, 0.000f, 0.937f, 0.502f);

    public TextTyper typer;

    private float timeLimit = 80f;
    private float elapsedTime = 0f;
    private bool isGameFinished = false;

    // 기본 피드백 위치 저장
    private Vector2 goodDefaultPos;
    private Vector2 perfectDefaultPos;
    private Vector2 missDefaultPos;

    // 현재 실행 중인 피드백 코루틴
    private Coroutine currentFeedbackCoroutine;

    void Start()
    {
        // 피드백 이미지 비활성화 및 기본 위치 저장
        goodImage.SetActive(false);
        perfectImage.SetActive(false);
        missImage.SetActive(false);
        nextButton.SetActive(false);

        goodDefaultPos = goodImage.GetComponent<RectTransform>().anchoredPosition;
        perfectDefaultPos = perfectImage.GetComponent<RectTransform>().anchoredPosition;
        missDefaultPos = missImage.GetComponent<RectTransform>().anchoredPosition;

        defaultInkPosition1 = ink1.rectTransform.anchoredPosition;
        defaultInkPosition2 = ink2.rectTransform.anchoredPosition;
        defaultInkPosition3 = ink3.rectTransform.anchoredPosition;

        StartCoroutine(ShowDialogueSequence());
        CreateGrid();
        ExtractCorrectPattern();
        HighlightCurrentTile();
    }

    void Update()
    {
        if (isGameFinished) return;

        elapsedTime += Time.deltaTime;
        if (elapsedTime > timeLimit)
        {
            Debug.Log("[시간 초과] 80초 내에 완료하지 못해 점수 0 처리");
            ScoreManager.score = 0;
            ScoreManager.total = 100;
            isGameFinished = true;
            nextButton.SetActive(true);
            return;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            selectedPrefab = tileGreenPrefab;
            HighlightInk(ink1, inkPopGreen);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            selectedPrefab = tilePinkPrefab;
            HighlightInk(ink2, inkPopPink);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            selectedPrefab = tileBluePrefab;
            HighlightInk(ink3, inkPopBlue);
        }

        if (Input.GetMouseButtonDown(0))
        {
            PaintCurrentTile();
            MoveNext();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            MoveNext();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("C 키 눌림 → 점수 강제 계산");
            CalculateScore();
        }
    }

    void HighlightInk(Image inkImage, Sprite poppedSprite)
    {
        ink1.sprite = inkBasicGreen;
        ink2.sprite = inkBasicPink;
        ink3.sprite = inkBasicBlue;

        ink1.rectTransform.sizeDelta = new Vector2(138, 198);
        ink2.rectTransform.sizeDelta = new Vector2(138, 198);
        ink3.rectTransform.sizeDelta = new Vector2(138, 198);

        ink1.rectTransform.anchoredPosition = defaultInkPosition1;
        ink2.rectTransform.anchoredPosition = defaultInkPosition2;
        ink3.rectTransform.anchoredPosition = defaultInkPosition3;

        inkImage.sprite = poppedSprite;
        Vector2 poppedSize = new Vector2(138, 242);
        inkImage.rectTransform.sizeDelta = poppedSize;
        float deltaY = (poppedSize.y - 198) / 2f - 20;
        inkImage.rectTransform.anchoredPosition += new Vector2(0, deltaY);
    }

    void CreateGrid()
    {
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                GameObject emptyTile = new GameObject($"Tile_{x}_{y}", typeof(RectTransform), typeof(Image));
                emptyTile.transform.SetParent(gridParent, false);

                RectTransform rt = emptyTile.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(72, 72);
                rt.anchoredPosition = new Vector2(gridOrigin.x + x * 72, gridOrigin.y + y * 72);

                Image img = emptyTile.GetComponent<Image>();
                img.color = Color.clear;

                tiles[x, y] = emptyTile;
                userInput[x, y] = Color.clear;
            }
        }
    }

    void ExtractCorrectPattern()
    {
        Texture2D tex = Back_tattoo.sprite.texture;

        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                int px = x * 72 + 36 + 1045;
                int py = y * 72 + 36 + 155;
                Color pixelColor = tex.GetPixel(px, py);
                correctPattern[x, y] = ClassifyColor(pixelColor);
            }
        }
    }

    bool IsTransparent(Color color) =>
        AreColorsSimilar(color, Color.clear);

    bool AreColorsSimilar(Color a, Color b, float threshold = 0.1f) =>
        Mathf.Abs(a.r - b.r) < threshold &&
        Mathf.Abs(a.g - b.g) < threshold &&
        Mathf.Abs(a.b - b.b) < threshold;

    Color ClassifyColor(Color pixel)
    {
        if (AreColorsSimilar(pixel, pink)) return pink;
        if (AreColorsSimilar(pixel, green)) return green;
        if (AreColorsSimilar(pixel, blue)) return blue;
        return Color.clear;
    }

    void PaintCurrentTile()
    {
        if (selectedPrefab == null) return;

        GameObject painted = Instantiate(selectedPrefab, tiles[currentX, currentY].transform);
        RectTransform prt = painted.GetComponent<RectTransform>();
        prt.anchoredPosition = Vector2.zero;
        prt.sizeDelta = new Vector2(72, 72);

        Color selectedColor = Color.clear;
        if (selectedPrefab == tilePinkPrefab) selectedColor = pink;
        else if (selectedPrefab == tileGreenPrefab) selectedColor = green;
        else if (selectedPrefab == tileBluePrefab) selectedColor = blue;

        userInput[currentX, currentY] = selectedColor;

        int prevScore = ScoreManager.score;
        CalculateScore();
        ShowAppropriateUI(prevScore);
    }

    void ShowAppropriateUI(int prevScore)
    {
        bool scoreIncreased = ScoreManager.score > prevScore;

        if (!scoreIncreased)
        {
            ShowFeedback(missImage);
            if (typer != null)
            {
                typer.StartTyping("아야! 아프잖아");
                StartCoroutine(ShowPositiveAfterDelay(4f));
            }
            return;
        }

        if (ScoreManager.score >= 50)
            ShowFeedback(perfectImage);
        else
            ShowFeedback(goodImage);
    }

    void ShowFeedback(GameObject img, float displayTime = 1.5f)
    {
        if (currentFeedbackCoroutine != null)
            StopCoroutine(currentFeedbackCoroutine);

        HideAllFeedbacks();

        RectTransform rt = img.GetComponent<RectTransform>();
        img.SetActive(true);

        // 기본 위치 리셋 후 랜덤 오프셋 적용
        if (img == goodImage) rt.anchoredPosition = goodDefaultPos;
        else if (img == perfectImage) rt.anchoredPosition = perfectDefaultPos;
        else if (img == missImage) rt.anchoredPosition = missDefaultPos;

        rt.anchoredPosition += new Vector2(Random.Range(-20f, 20f), Random.Range(-20f, 20f));

        // 스케일 초기화
        img.transform.localScale = Vector3.zero;

        currentFeedbackCoroutine = StartCoroutine(FeedbackRoutine(img, displayTime));
    }

    IEnumerator FeedbackRoutine(GameObject img, float delay)
    {
        // 들어올 때 팝업 애니메이션 (0→1.2→1)
        float t = 0f;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(0f, 1.2f, t / 0.2f);
            img.transform.localScale = Vector3.one * s;
            yield return null;
        }

        float t2 = 0f;
        while (t2 < 0.1f)
        {
            t2 += Time.deltaTime;
            float s = Mathf.Lerp(1.2f, 1f, t2 / 0.1f);
            img.transform.localScale = Vector3.one * s;
            yield return null;
        }

        // 유지 시간
        yield return new WaitForSeconds(delay);

        // 나갈 때 축소 애니메이션 (1→0)
        float t3 = 0f;
        Vector3 startScale = img.transform.localScale;
        while (t3 < 0.1f)
        {
            t3 += Time.deltaTime;
            float s = Mathf.Lerp(startScale.x, 0f, t3 / 0.2f);
            img.transform.localScale = Vector3.one * s;
            yield return null;
        }

        img.SetActive(false);
        img.transform.localScale = Vector3.one;
        currentFeedbackCoroutine = null;
    }

    void HideAllFeedbacks()
    {
        goodImage.SetActive(false);
        perfectImage.SetActive(false);
        missImage.SetActive(false);
    }

    void MoveNext()
    {
        UnhighlightCurrentTile();
        if (currentX == 9 && currentY == 9)
        {
            CalculateScore();
            isGameFinished = true;
            nextButton.SetActive(true);
            return;
        }

        currentX++;
        if (currentX >= 10)
        {
            currentX = 0;
            currentY++;
        }

        HighlightCurrentTile();
    }

    void HighlightCurrentTile()
    {
        Image img = tiles[currentX, currentY].GetComponent<Image>();
        img.color = new Color(1f, 0f, 1f, 0.1f);
        if (cursor != null)
        {
            cursor.SetParent(tiles[currentX, currentY].transform, false);
            cursor.anchoredPosition = new Vector2(0, 10);
        }
    }

    void UnhighlightCurrentTile()
    {
        tiles[currentX, currentY].GetComponent<Image>().color = Color.clear;
    }

    private IEnumerator ShowDialogueSequence()
    {
        yield return new WaitForSeconds(1f);
        if (typer != null)
            typer.StartTyping("A,S,D로 잉크색을 바꾸고,\n좌클릭으로 새기기,\n스페이스바로 넘어가기!");
        yield return new WaitForSeconds(5f);
        if (typer != null)
            typer.StartTyping("좋아. 잘하고 있어!");
    }

    void CalculateScore()
    {
        int score = 0;
        int total = 0;

        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                if (y < currentY || (y == currentY && x <= currentX))
                {
                    total++;
                    Color correct = correctPattern[x, y];
                    Color user = userInput[x, y];
                    if (AreColorsSimilar(correct, Color.clear))
                    {
                        if (user == Color.clear) score++;
                    }
                    else if (AreColorsSimilar(correct, user))
                    {
                        score++;
                    }
                }
            }
        }

        ScoreManager.score = score;
        ScoreManager.total = total;
        float percentage = (total > 0) ? ((float)score / total) * 100f : 0f;
        Debug.Log($"[최종 점수] {score}/{total} ({percentage:F1}%)");
    }

    private IEnumerator ShowPositiveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (typer != null)
            typer.StartTyping("좋아. 잘하고 있어!");
    }
}
