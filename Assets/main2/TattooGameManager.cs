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
    public RectTransform cursor; // 커서 오브젝트 연결

    private GameObject[,] tiles = new GameObject[10, 10];
    private Color[,] correctPattern = new Color[10, 10];
    private Color[,] userInput = new Color[10, 10];

    private int currentX = 0;
    private int currentY = 9;
    private GameObject selectedPrefab = null;

    private Vector2 gridOrigin = new Vector2(0f, 0f); // gridParent 기준 상대 좌표

    void Start()
    {
        CreateGrid();
        ExtractCorrectPattern();
        HighlightCurrentTile();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) selectedPrefab = tilePinkPrefab;
        if (Input.GetKeyDown(KeyCode.S)) selectedPrefab = tileGreenPrefab;
        if (Input.GetKeyDown(KeyCode.D)) selectedPrefab = tileBluePrefab;

        if (Input.GetMouseButtonDown(0))
        {
            PaintCurrentTile();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            MoveNext();
        }
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
                rt.anchoredPosition = new Vector2(gridOrigin.x + x * 72, gridOrigin.y + y * 72); // 아래에서 위로

                Image img = emptyTile.GetComponent<Image>();
                img.color = Color.clear;

                // 아래에서 위로 시작
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
                correctPattern[x, y] = pixelColor; // y 그대로 사용
            }
        }
    }


    void PaintCurrentTile()
    {
        Color correct = correctPattern[currentX, currentY];

        if (IsTransparent(correct))
        {
            Debug.Log("투명칸인데 잘못 칠함");
        }

        if (selectedPrefab != null)
        {
            GameObject painted = Instantiate(selectedPrefab, tiles[currentX, currentY].transform);
            painted.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            painted.GetComponent<RectTransform>().sizeDelta = new Vector2(72, 72);

            //색상 직접 지정
            if (selectedPrefab == tilePinkPrefab)
                userInput[currentX, currentY] = new Color32(0xF5, 0x36, 0xE2, 0xFF);
            else if (selectedPrefab == tileGreenPrefab)
                userInput[currentX, currentY] = new Color32(0x70, 0xE7, 0x4E, 0xFF);
            else if (selectedPrefab == tileBluePrefab)
                userInput[currentX, currentY] = new Color32(0x60, 0x22, 0xF2, 0xFF);
        }
    }


    void MoveNext()
    {
        // 현재 칸이 마지막 칸이라면: (9, 0)
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
        img.color = new Color(1f, 0f, 1f, 0.3f);

        // 커서 위치 이동
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
                Color correct = correctPattern[x, y];
                Color user = userInput[x, y];

                if (IsTransparent(correct))
                {
                    if (user == Color.clear)
                        score++;
                }
                else
                {
                    if (AreColorsSimilar(correct, user))
                        score++;
                }

                total++;
            }
        }

     

        //static 변수에 저장
        ScoreManager.score = score;
        ScoreManager.total = total;

        float percentage = ScoreManager.GetPercentage();
        Debug.Log($"점수: {score}/{total} ({percentage:F1}%)");

    }

    bool IsTransparent(Color color)
    {
        return color.a < 0.1f || color == Color.clear;
    }

    bool AreColorsSimilar(Color a, Color b)
    {
        float threshold = 0.1f;
        return Mathf.Abs(a.r - b.r) < threshold &&
               Mathf.Abs(a.g - b.g) < threshold &&
               Mathf.Abs(a.b - b.b) < threshold;
    }

}

