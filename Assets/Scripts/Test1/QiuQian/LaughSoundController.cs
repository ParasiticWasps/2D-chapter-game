using UnityEngine;

public class LaughSoundController : MonoBehaviour
{
    [Header("笑声设置")]
    public AudioClip laughClip;
    [Range(0f, 1f)]
    public float laughVolume = 0.5f;

    [Header("淡入淡出")]
    public float fadeInTime = 1f;
    public float fadeOutTime = 1.5f;
    public float playInterval = 5f;              // 笑声播放间隔

    private AudioSource audioSource;
    private float targetVolume = 0f;
    private float currentVolume = 0f;
    private bool isPlayerNear = false;
    private float nextLaughTime = 0f;

    void Start()
    {
        // 设置AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = laughClip;
        audioSource.loop = false;                 // 不循环，手动控制间隔
        audioSource.volume = 0f;
        audioSource.spatialBlend = 1f;            // 3D音效，从秋千位置发出
    }

    void Update()
    {
        // 平滑调整音量
        currentVolume = Mathf.Lerp(currentVolume, targetVolume, Time.deltaTime * 5f);
        audioSource.volume = currentVolume * laughVolume;

        // 如果玩家在范围内，且可以播放下一次笑声
        if (isPlayerNear && Time.time >= nextLaughTime)
        {
            PlayLaugh();
            nextLaughTime = Time.time + playInterval;
        }
    }

    public void SetPlayerNear(bool isNear)
    {
        if (isNear && !isPlayerNear)
        {
            // 进入范围，开始淡入
            targetVolume = 1f;
            nextLaughTime = Time.time + 0.5f;      // 短暂延迟后第一次播放
        }
        else if (!isNear && isPlayerNear)
        {
            // 离开范围，淡出
            targetVolume = 0f;
        }

        isPlayerNear = isNear;
    }

    void PlayLaugh()
    {
        if (laughClip != null && currentVolume > 0.1f)
        {
            audioSource.PlayOneShot(laughClip, currentVolume * laughVolume);
        }
    }

    // 立即停止所有笑声
    public void StopImmediately()
    {
        targetVolume = 0f;
        currentVolume = 0f;
        audioSource.Stop();
        isPlayerNear = false;
    }
}