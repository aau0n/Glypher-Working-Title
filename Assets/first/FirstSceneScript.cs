using UnityEngine;
using UnityEngine.SceneManagement;


public class FirstSceneScript : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject[] buttonObjects;      // 버튼 스프라이트 3개 오브젝트
    public GameObject[] hoverImages;        // 하이라이트 이미지 3개 (Hierarchy에 미리 배치)
    private int currentIndex = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 시작 시 모든 하이라이트 이미지를 꺼둠
        foreach (var img in hoverImages)
        {
            img.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        int spriteIdx = -1;
        if (hit.collider != null)
        {
            for (int i = 0; i < buttonObjects.Length; i++)
            {
                if (hit.collider.gameObject == buttonObjects[i])
                {
                    spriteIdx = i;
                    break;
                }
            }
        }

        if (spriteIdx != -1)
        {
            if (currentIndex != spriteIdx)
            {
                ClearHover();
                ShowHover(spriteIdx);
            }
            // 클릭 처리
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log($"버튼 {spriteIdx + 1} 클릭됨!");
                
                // 버튼별로 원하는 동작 구현 가능
                switch (spriteIdx)
                {
                    case 0: // 새 게임
                        SceneManager.LoadScene("2Opening");
                        break;
                    case 1: // 이어하기
                        // SceneManager.LoadScene("ContinueScene");
                        break;
                    case 2: // 나가기 (예: 앱 종료)
#if UNITY_EDITOR
                            UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
                        break;
                }  
    
            }
        }
        else
        {
            ClearHover();
        }
    }

    void ShowHover(int idx)
    {
        hoverImages[idx].SetActive(true);
        currentIndex = idx;
    }

    void ClearHover()
    {
        if (currentIndex != -1)
        {
            hoverImages[currentIndex].SetActive(false);
            currentIndex = -1;
        }
    }
}
