using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TattooGameManager : MonoBehaviour
{
    public GameObject tilePinkPrefab;
    public GameObject tileGreenPrefab;
    public GameObject tileBluePrefab;
    public Transform gridParent;
    public SpriteRenderer backgroundPattern;
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

    private GameObject[,] tiles = new GameObject[10, 10];
    private Color[,] correctPattern = new Color[10, 10];
    private Color[,] userInput = new Color[10, 10];

    private int currentX = 0;
    private int currentY = 9;
    private GameObject selectedPrefab = null;

    private Vector2 gridOrigin = new Vector2(0f, 0f);

    private Vector2 defaultInkPosition1;
    private Vector2 defaultInkPosition2;
    private Vector2 defaultInkPosition3;

    // 기준 색상 정의
    private Color pink = new Color32(0xF5, 0x36, 0xE2, 0xFF);
    private Color green = new Color32(0x70, 0xE7, 0x4E, 0xFF);
    private Color blue = new Color32(0x60, 0x22, 0xF2, 0xFF);

    public TextTyper typer; // Inspector에서 TextTyper 스크립트가 붙은 오브젝트 연결

    void Start()
    {
        defaultInkPosition1 = ink1.rectTransform.anchoredPosition;
        defaultInkPosition2 = ink2.rectTransform.anchoredPosition;
        defaultInkPosition3 = ink3.rectTransform.anchoredPosition;

        CreateGrid();
        ExtractCorrectPattern();
        HighlightCurrentTile();
    }

    void Update()
    {
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
        // 원래대로 초기화
        ink1.sprite = inkBasicGreen;
        ink2.sprite = inkBasicPink;
        ink3.sprite = inkBasicBlue;

        ink1.rectTransform.sizeDelta = new Vector2(138, 198);
        ink2.rectTransform.sizeDelta = new Vector2(138, 198);
        ink3.rectTransform.sizeDelta = new Vector2(138, 198);

        ink1.rectTransform.anchoredPosition = defaultInkPosition1;
        ink2.rectTransform.anchoredPosition = defaultInkPosition2;
        ink3.rectTransform.anchoredPosition = defaultInkPosition3;

        // 선택된 잉크 강조
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
        Texture2D tex = backgroundPattern.sprite.texture;

        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                int px = x * 72 + 36;
                int py = y * 72 + 36;

                Color pixelColor = tex.GetPixel(px, py);
                correctPattern[x, y] = ClassifyColor(pixelColor);

                // Debug.Log($"[패턴] ({x},{y}) 픽셀: {pixelColor} → 분류: {correctPattern[x, y]}");
            }
        }
    }

    void PaintCurrentTile()
    {
        Color correct = correctPattern[currentX, currentY];

        if (IsTransparent(correct))
        {
            Debug.Log($"[경고] ({currentX},{currentY}) 는 투명칸인데 칠함");
        }

        if (selectedPrefab != null)
        {
            GameObject painted = Instantiate(selectedPrefab, tiles[currentX, currentY].transform);
            painted.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            painted.GetComponent<RectTransform>().sizeDelta = new Vector2(72, 72);

            Color selectedColor = Color.clear;

            if (selectedPrefab == tilePinkPrefab)
                selectedColor = pink;
            else if (selectedPrefab == tileGreenPrefab)
                selectedColor = green;
            else if (selectedPrefab == tileBluePrefab)
                selectedColor = blue;

            userInput[currentX, currentY] = selectedColor;

            Debug.Log($"[페인트] ({currentX},{currentY}) 위치에 {selectedColor} 색상으로 칠함");
        }
    }

    void MoveNext()
    {
        if (currentX == 9 && currentY == 0)
        {
            UnhighlightCurrentTile();
            Debug.Log("완료! 점수 계산 ㄱㄱ");
            CalculateScore();
            return;
        }

        UnhighlightCurrentTile();

        currentX++;

        if (currentX >= 10)
        {
            currentX = 0;
            currentY--;
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
            cursor.anchoredPosition = Vector2.zero;
        }
    }

    void UnhighlightCurrentTile()
    {
        Image img = tiles[currentX, currentY].GetComponent<Image>();
        img.color = Color.clear;
    }

    void CalculateScore()
    {
        int score = 0;
        int total = 0;

        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                if (y < currentY || (y == currentY && x < currentX))
                {
                    Color correct = correctPattern[x, y];
                    Color user = userInput[x, y];

                    if (IsTransparent(correct))
                    {
                        if (user == Color.clear)
                        {
                            score++;
                        }
                        else
                        {
                            Debug.Log($"[오답] ({x},{y})는 비워야 했는데 칠함 → 사용자 색: {user}");
                        }
                    }
                    else
                    {
                        if (AreColorsSimilar(correct, user))
                        {
                            score++;
                        }
                        else
                        {
                            Debug.Log($"[오답] ({x},{y}) 색 다름 → 정답: {correct}, 사용자: {user}");
                        }
                    }
                    total++;
                }
                // 그 외 칸들은 점수 계산 안함
            }
        }

        int maxTotal = 10 * 10; // 100칸 고정

        ScoreManager.score = score;
        ScoreManager.total = maxTotal;

        float percentage = ((float)score / maxTotal) * 100f;

        Debug.Log($"[최종 점수] {score}/{maxTotal} ({percentage:F1}%)");

    }


    bool IsTransparent(Color color)
    {
        return color.a < 0.1f || color == Color.clear;
    }

    bool AreColorsSimilar(Color a, Color b, float threshold = 0.1f)
    {
        return Mathf.Abs(a.r - b.r) < threshold &&
               Mathf.Abs(a.g - b.g) < threshold &&
               Mathf.Abs(a.b - b.b) < threshold;
    }

    Color ClassifyColor(Color pixel)
    {
        if (AreColorsSimilar(pixel, pink)) return pink;
        if (AreColorsSimilar(pixel, green)) return green;
        if (AreColorsSimilar(pixel, blue)) return blue;

        return Color.clear;
    }
}
