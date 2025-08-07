using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BGMPlayer : MonoBehaviour
{
    public AudioClip[] bgmClips; // 인스펙터에 BGM 오디오클립을 배열로 할당 (ex: [0]=첫번째곡, [1]=두번째곡 등)
    private AudioSource audioSource;

    // 각 곡이 적용될 씬 인덱스 구간 (예: bgmClipIndexPerScene[0]=0으면 0번째 씬에서 [0]번 곡)
    public int[] bgmClipIndexPerScene;

    public float fadeDuration = 1.0f; // 페이드인/아웃 지속 시간(초)
    private static BGMPlayer instance;

    private AudioClip currentClip = null;
    private Coroutine fadeCoroutine = null;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayBGMForCurrentScene();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForCurrentScene();
    }

    private void PlayBGMForCurrentScene()
    {
        int sceneIdx = SceneManager.GetActiveScene().buildIndex;

        int clipIdx = 0;
        if (sceneIdx < bgmClipIndexPerScene.Length)
            clipIdx = bgmClipIndexPerScene[sceneIdx];

        AudioClip nextClip = bgmClips[clipIdx];

        if (currentClip == null)
    {
        // 처음 시작하는 노래 - 페이드 없이 바로 재생
        audioSource.clip = nextClip;
        audioSource.volume = 1f;   // 원하는 기본 볼륨 설정
        audioSource.Play();
        currentClip = nextClip;
    }
    else if (currentClip != nextClip)
    {
        // 이미 다른 곡이 재생 중이면 페이드 효과 적용
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutIn(nextClip));
    }
    // 같으면 아무 작업 안 함
    }

    private IEnumerator FadeOutIn(AudioClip nextClip)
    {
        // 페이드 아웃
        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0;

        // 클립 교체
        audioSource.clip = nextClip;
        audioSource.Play();
        currentClip = nextClip;

        // 페이드 인
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = startVolume;

        fadeCoroutine = null;
    }

    /*
    1. 빈 오브젝트에 AudioSource + 이 스크립트 붙이기
    2. bgmClips 배열에 사용할 음악 파일 차례대로 할당
    3. bgmClipsIndexPerScene 배열에 각 씬 인덱스에 맞춰 쓸 곡의 인덱스 지정
        - 씬 0~2는 0번 곡, 씬 3~5는 1번 곡, 나머지는 2번 곡을 사용하는 경우
        - bgmClipIndexPerScene = [0, 0, 0, 1, 1, 1, 2, ...]
    4. Scene build index 기준이므로 빌드 세팅에서 씬 순서를 반드시 알맞게 조정
    */

    // Update is called once per frame
    void Update()
    {
        
    }
}
