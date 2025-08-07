using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;


public class BGMPlayer : MonoBehaviour
{
    public AudioClip[] bgmClips; // 인스펙터에 BGM 오디오클립을 배열로 할당 (ex: [0]=첫번째곡, [1]=두번째곡 등)
    private AudioSource audioSource;

    // 각 곡이 적용될 씬 인덱스 구간 (예: bgmClipIndexPerScene[0]=0으면 0번째 씬에서 [0]번 곡)
    public int[] bgmClipIndexPerScene;
    public float[] bgmVolumePerScene; // 씬별 볼륨 설정
    public int[] fadeOutScenes; // 페이드아웃 기능을 사용할 씬 번호들
    public float fadeOutAfterSeconds = -1f; // 이 값보다 클 경우 씬 시작 후 해당 초 경과 시 페이드아웃
    private Coroutine fadeOutTimerCoroutine = null;


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
        float targetVolume = 1f;

        if (sceneIdx < bgmClipIndexPerScene.Length)
            clipIdx = bgmClipIndexPerScene[sceneIdx];

        if (sceneIdx < bgmVolumePerScene.Length)
            targetVolume = bgmVolumePerScene[sceneIdx];

        AudioClip nextClip = bgmClips[clipIdx];

        bool isSameClip = (currentClip == nextClip);
        bool isVolumeDifferent = !Mathf.Approximately(audioSource.volume, targetVolume);

        // 페이드아웃 예약 취소 (씬 전환 시 기존 타이머 중지)
        if (fadeOutTimerCoroutine != null)
        {
            StopCoroutine(fadeOutTimerCoroutine);
            fadeOutTimerCoroutine = null;
        }

        // ⬇ 이 부분에서 현재 씬이 fadeOutScenes에 포함될 때만 타이머 시작
        if (fadeOutAfterSeconds > 0 && fadeOutScenes.Contains(sceneIdx))
        {
            fadeOutTimerCoroutine = StartCoroutine(FadeOutAfterDelay(fadeOutAfterSeconds));
        }

        if (currentClip == null)
        {
            // 처음 시작하는 노래 - 페이드 없이 바로 재생
            audioSource.clip = nextClip;
            audioSource.volume = targetVolume;
            audioSource.Play();
            currentClip = nextClip;
        }
        else if (!isSameClip)
        {
            // 이미 다른 곡이 재생 중이면 페이드 효과 적용
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeOutIn(nextClip, targetVolume));
        }
        else if (isVolumeDifferent)
        {
            // 이전 씬과 같은 노래지만 볼륨이 달라졌을 때
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeVolume(audioSource.volume, targetVolume));
        }
        // 같으면 아무 작업 안 함

        // 일정 시간 후 페이드아웃 예약 (볼륨만 조정 시에도 실행)
        if (fadeOutAfterSeconds > 0 && fadeOutScenes != null && fadeOutScenes.Contains(sceneIdx))
        {
            fadeOutTimerCoroutine = StartCoroutine(FadeOutAfterDelay(fadeOutAfterSeconds));
        }

    }

    private IEnumerator FadeOutIn(AudioClip nextClip, float targetVolume)
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
            audioSource.volume = Mathf.Lerp(0, targetVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
        fadeCoroutine = null;
    }

    private IEnumerator FadeVolume(float from, float to)
    {
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = to;
        fadeCoroutine = null;
    }

    private IEnumerator FadeOutAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.Stop();
        currentClip = null; // 다음 씬에서 다시 감지되도록 초기화

        fadeOutTimerCoroutine = null;
    }


    /*
    1. 빈 오브젝트에 AudioSource + 이 스크립트 붙이기
    2. bgmClips 배열에 사용할 음악 파일 차례대로 할당
    3. bgmClipsIndexPerScene 배열에 각 씬 인덱스에 맞춰 쓸 곡의 인덱스 지정
        - 씬 0~2는 0번 곡, 씬 3~5는 1번 곡, 나머지는 2번 곡을 사용하는 경우
        - bgmClipIndexPerScene = [0, 0, 0, 1, 1, 1, 2, ...]
    4. bgmVolumePerScene 배열에 각 씬 인덱스에 맞춰 쓸 곡의 볼륨을 조정
    5. fadeOutAfterSeconds에 0보다 큰 값을 넣으면 씬 시작 후 자동으로 페이드아웃됨.
        -1 or 0으로 설정시 자동 페이드아웃 X
    6. Scene build index 기준이므로 빌드 세팅에서 씬 순서를 반드시 알맞게 조정
    */

    // Update is called once per frame
    void Update()
    {
        
    }
}
