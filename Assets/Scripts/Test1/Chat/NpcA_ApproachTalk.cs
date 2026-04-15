using UnityEngine;

public class NpcA_ApproachTalk : MonoBehaviour
{
    [Header("触发设置")]
    [Tooltip("靠近触发距离")]
    public float approachDistance = 1.5f;      // 对应文档中的50px（假设1单位≈32px，这里用1.5≈48px）
    public string talkContent = "早上好，小姐。";  // 头顶文字内容

    [Header("文字显示设置")]
    public float textDisplayDuration = 3f;
    public float verticalOffset = 2f;           // 文字头顶偏移

    private Transform playerTransform;
    private bool isPlayerNear = false;
    private GameObject currentFloatingText;
    private FloatingTextPool textPool;

    // 记录上次触发时间，防止频繁触发
    private float lastTriggerTime = -10f;
    public float triggerCooldown = 5f;          // 冷却时间

    void Start()
    {
        // 查找玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        // 查找或创建对象池
        textPool = FindObjectOfType<FloatingTextPool>();
        if (textPool == null)
        {
            GameObject poolObj = new GameObject("FloatingTextPool");
            textPool = poolObj.AddComponent<FloatingTextPool>();
            // 需要手动设置prefab，这里先警告
            Debug.LogWarning("请在FloatingTextPool组件上设置floatingTextPrefab");
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 计算距离
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool nowNear = distance <= approachDistance;

        // 状态变化时处理
        if (nowNear && !isPlayerNear)
        {
            // 进入范围
            EnterRange();
        }
        else if (!nowNear && isPlayerNear)
        {
            // 离开范围
            ExitRange();
        }

        isPlayerNear = nowNear;
    }

    void EnterRange()
    {
        // 检查冷却时间
        if (Time.time - lastTriggerTime < triggerCooldown)
            return;

        lastTriggerTime = Time.time;

        // 显示头顶文字
        ShowFloatingText();
    }

    void ExitRange()
    {
        // 如果玩家离开范围，提前隐藏文字
        if (currentFloatingText != null)
        {
            FloatingTextController controller = currentFloatingText.GetComponent<FloatingTextController>();
            if (controller != null)
                controller.HideImmediately();
            currentFloatingText = null;
        }
    }

    void ShowFloatingText()
    {
        if (textPool == null || textPool.floatingTextPrefab == null)
        {
            Debug.LogError("文字对象池或预制体未设置！");
            return;
        }

        // 从池中获取文字对象
        currentFloatingText = textPool.GetFloatingText();

        // 设置位置
        currentFloatingText.transform.position = transform.position + Vector3.up * verticalOffset;

        // 获取控制器并显示文字
        // 获取控制器并设置对象池
        FloatingTextController controller = currentFloatingText.GetComponent<FloatingTextController>();
        if (controller == null)
            controller = currentFloatingText.AddComponent<FloatingTextController>();

        controller.SetPool(textPool);  // 传入对象池引用
        controller.displayDuration = textDisplayDuration;
        controller.verticalOffset = verticalOffset;
        controller.ShowText(talkContent, transform); // 传入transform让它跟随

        // 不再需要手动回收，文字消失时会自动销毁（但我们用对象池，所以应该回收而不是销毁）
        // 需要修改FloatingTextController，让它通知池子回收
    }
}