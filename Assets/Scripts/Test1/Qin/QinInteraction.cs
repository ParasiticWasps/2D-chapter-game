using UnityEngine;

public class QinInteraction : MonoBehaviour
{
    [Header("琴的设置")]
    public AudioClip qinMusicClip;                 // 古琴音效
    public float minDistance = 1f;                  // 最近距离（最大音量）
    public float maxDistance = 4f;                   // 最远距离（最小音量）
    public float maxVolume = 0.8f;                   // 最大音量

    [Header("音效控制")]
    [Range(0.1f, 3f)]
    public float fadeInTime = 1.2f;                  // 淡入时间
    [Range(0.1f, 3f)]
    public float fadeOutTime = 2f;                    // 淡出时间

    [Header("高级设置")]
    public AnimationCurve customVolumeCurve;          // 自定义音量曲线

    private DynamicAudioSource dynamicAudio;

    void Start()
    {
        // 添加动态音频组件
        dynamicAudio = gameObject.AddComponent<DynamicAudioSource>();

        // 配置动态音频
        dynamicAudio.audioClip = qinMusicClip;
        dynamicAudio.minDistance = minDistance;
        dynamicAudio.maxDistance = maxDistance;
        dynamicAudio.maxVolume = maxVolume;
        dynamicAudio.fadeInTime = fadeInTime;
        dynamicAudio.fadeOutTime = fadeOutTime;

        // 如果提供了自定义曲线，使用它
        if (customVolumeCurve != null && customVolumeCurve.keys.Length > 0)
        {
            dynamicAudio.volumeCurve = customVolumeCurve;
        }

        // 自动查找玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            dynamicAudio.listenerTransform = player.transform;
        }

        Debug.Log($"琴音效初始化完成，最大距离：{maxDistance}，最小距离：{minDistance}");
    }

    // 可选：添加视觉反馈（当玩家靠近时）
    void OnDrawGizmosSelected()
    {
        // 绘制音效范围
        Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, maxDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, minDistance);
    }
}