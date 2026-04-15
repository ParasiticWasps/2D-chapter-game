using UnityEngine;
using UnityEngine.Events; // 用于 UnityEvent，方便 Inspector 绑定

public class InteractableObject : MonoBehaviour
{
    [Header("触发设置")]
    [Tooltip("触发类型：靠近自动触发 或 点击触发")]
    public TriggerType triggerType = TriggerType.OnApproach;
    [Tooltip("靠近触发距离")]
    public float approachDistance = 2f;
    [Tooltip("是否只触发一次")]
    public bool triggerOnce = false;

    [Header("事件")]
    public UnityEvent onTriggered; // 触发时要执行的操作（显示文字、播放音效等）

    private bool hasTriggered = false;
    private Transform playerTransform;

    void Start()
    {
        // 查找玩家（可以根据标签或类型查找）
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
        else
            Debug.LogWarning("场景中没有找到 Tag 为 'Player' 的对象");
    }

    void Update()
    {
        if (triggerOnce && hasTriggered) return;
        if (playerTransform == null) return;

        // 距离检测
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist <= approachDistance)
        {
            if (triggerType == TriggerType.OnApproach)
            {
                Trigger();
            }
        }
    }

    void OnMouseDown()
    {
        // 鼠标点击触发（需要物体有 Collider）
        if (triggerType == TriggerType.OnClick)
        {
            Trigger();
        }
    }

    void Trigger()
    {
        if (triggerOnce && hasTriggered) return;
        hasTriggered = true;
        onTriggered.Invoke(); // 调用 Inspector 中绑定的事件
    }
    public void Chat()
    {
        Debug.Log("触发宫女对话！");
    }
    public enum TriggerType
    {
        OnApproach, // 靠近触发
        OnClick     // 点击触发
    }
}