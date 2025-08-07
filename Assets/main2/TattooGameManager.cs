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

    private Color green = new Color(0.353f, 1.000f, 0.000f, 0.502f); // 연두색
    private Color blue = new Color(0.278f, 0.522f, 0.827f, 0.502f);  // 파란색
    private Color pink = new Color(1.000f, 0.000f, 0.937f, 0.502f);  // 핑크색




    public TextTyper typer;

    // 타이머 및 게임 종료 상태 변수 추가
    private float timeLimit = 80f; // 제한시간 80초
    private float elapsedTime = 0f;
    private bool isGameFinished = false;

    void Start()
    {
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
        if (isGameFinished)
            return; // 게임 종료 시 입력 및 시간 업데이트 중단

        elapsedTime += Time.deltaTime;

        if (elapsedTime > timeLimit)
        {
            Debug.Log("[시간 초과] 80초 내에 완료하지 못해 점수 0 처리");
            ScoreManager.score = 0;
            ScoreManager.total = 100;
            isGameFinished = true;
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
                Color classified = ClassifyColor(pixelColor);

                correctPattern[x, y] = classified;

                // 콘솔 출력 추가
                string colorName = "Unknown";
                if (AreColorsSimilar(classified, pink)) colorName = "Pink";
                else if (AreColorsSimilar(classified, green)) colorName = "Green";
                else if (AreColorsSimilar(classified, blue)) colorName = "Blue";
                else if (IsTransparent(classified)) colorName = "Transparent";

                Debug.Log($"[패턴 추출] 좌표 ({x},{y}) → 원본 색: {pixelColor} → 분류된 색: {colorName}");
            }
        }
    }


    bool IsTransparent(Color color)
    {
        return AreColorsSimilar(color, Color.clear);
    }

    bool AreColorsSimilar(Color a, Color b, float threshold = 0.3f)
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

        // 어떤 지정된 색상과도 매칭되지 않으면 투명으로 처리
        return Color.clear; // 완전한 투명
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
        if (currentX == 9 && currentY == 9)
        {
            UnhighlightCurrentTile();
            Debug.Log("완료! 점수 계산 ㄱㄱ");
            CalculateScore();
            isGameFinished = true;
            return;
        }

        UnhighlightCurrentTile();

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
        Image img = tiles[currentX, currentY].GetComponent<Image>();
        img.color = Color.clear;
    }

    private IEnumerator ShowDialogueSequence()
    {
        yield return new WaitForSeconds(1f);

        if (typer != null)
            typer.StartTyping("A,S,D로 잉크색을 바꾸고,\n좌클릭으로 새기기,\n스페이스바로 넘어가기!");

        yield return new WaitForSeconds(4.5f);

        if(typer !=null)
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
                if (y < currentY)
                {
                    // 이미 지난 아랫줄
                }
                else if (y == currentY)
                {
                    if (x > currentX)
                        continue;
                }
                else
                {
                    continue;
                }

                Color correct = correctPattern[x, y];
                Color user = userInput[x, y];

                total++;

                if (AreColorsSimilar(correct, Color.clear))
                {
                    if (user == Color.clear)
                        score++;
                }
                else
                {
                    if (AreColorsSimilar(correct, user))
                        score++;
                }
            }
        }

        ScoreManager.score = score;
        ScoreManager.total = total;

        float percentage = (total > 0) ? ((float)score / total) * 100f : 0f;

        Debug.Log($"[최종 점수] {score}/{total} ({percentage:F1}%)");

        
    }



}