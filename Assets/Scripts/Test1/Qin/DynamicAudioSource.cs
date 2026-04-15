using UnityEngine;

public class DynamicAudioSource : MonoBehaviour
{
    [Header("音频设置")]
    public AudioClip audioClip;
    public bool playOnAwake = false;

    [Header("距离控制")]
    public Transform listenerTransform;           // 玩家（听者）位置
    public float minDistance = 1f;                // 最小距离（最大音量）
    public float maxDistance = 5f;                 // 最大距离（最小音量）
    public float maxVolume = 1f;                   // 最大音量
    public AnimationCurve volumeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0); // 音量随距离变化的曲线

    [Header("淡入淡出")]
    public float fadeInTime = 1f;                  // 进入范围时淡入时间
    public float fadeOutTime = 1.5f;                // 离开范围时淡出时间

    private AudioSource audioSource;
    private float targetVolume = 0f;
    private float currentVolume = 0f;
    private bool isPlayerInRange = false;
    private float fadeVelocity = 0f;

    void Start()
    {
        // 创建或获取AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // 配置AudioSource
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.playOnAwake = playOnAwake;
        audioSource.volume = 0f;

        // 如果没有设置听者，尝试找玩家
        if (listenerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                listenerTransform = player.transform;
        }

        // 预加载曲线（如果没设置，使用默认曲线）
        if (volumeCurve == null || volumeCurve.keys.Length == 0)
        {
            volumeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        }
    }

    void Update()
    {
        if (listenerTransform == null || audioSource == null) return;

        // 计算距离
        float distance = Vector3.Distance(transform.position, listenerTransform.position);

        // 判断是否在范围内
        bool inRange = distance <= maxDistance;

        // 状态变化处理
        if (inRange && !isPlayerInRange)
        {
            EnterRange();
        }
        else if (!inRange && isPlayerInRange)
        {
            ExitRange();
        }

        isPlayerInRange = inRange;

        // 计算目标音量（基于距离）
        if (inRange)
        {
            // 将距离映射到0-1（1是最小距离，0是最大距离）
            float t = Mathf.InverseLerp(maxDistance, minDistance, distance);
            t = Mathf.Clamp01(t);

            // 使用曲线调整音量变化
            targetVolume = maxVolume * volumeCurve.Evaluate(t);
        }

        // 平滑过渡当前音量到目标音量
        currentVolume = Mathf.SmoothDamp(currentVolume, targetVolume, ref fadeVelocity, 0.1f);
        audioSource.volume = currentVolume;
    }

    void EnterRange()
    {
        Debug.Log($"进入音效范围：{gameObject.name}");

        // 如果没在播放，开始播放
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        // 目标音量将在Update中自动计算，这里只需要开始淡入
        // 通过fadeVelocity自动处理
    }

    void ExitRange()
    {
        Debug.Log($"离开音效范围：{gameObject.name}");

        // 目标音量设为0，将在Update中淡出
        targetVolume = 0f;

        // 可选：在完全淡出后停止播放（节省性能）
        StartCoroutine(StopAfterFadeOut());
    }

    System.Collections.IEnumerator StopAfterFadeOut()
    {
        yield return new WaitForSeconds(fadeOutTime + 0.1f);
        if (audioSource.volume <= 0.01f && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // 可视化调试
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minDistance);
    }
}