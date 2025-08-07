using UnityEngine;

public class PaymentSound : MonoBehaviour
{
    public AudioClip sfxClip1;  // 자동 재생용
    public AudioClip sfxClip2;  // 버튼 클릭 시 재생용
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayFirstSound();
    }

    void PlayFirstSound()
    {
        if (sfxClip1 != null)
            audioSource.PlayOneShot(sfxClip1);
    }

    public void PlaySecondSound()
    {
        if (sfxClip2 != null)
            audioSource.PlayOneShot(sfxClip2);
    }
}
