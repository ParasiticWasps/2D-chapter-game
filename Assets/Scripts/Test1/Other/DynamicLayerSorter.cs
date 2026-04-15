using UnityEngine;
using System.Collections.Generic;

public class DynamicLayerSorter : MonoBehaviour
{
    [Header("基础设置")]
    public bool isPlayer = false;  // 是否是玩家
    public SortingMethod sortingMethod = SortingMethod.ByYPosition;
    public int baseSortingOrder = 0;

    [Header("Player优先级设置")]
    [Tooltip("Player的基础优先级，高于普通物体")]
    public int playerPriority = 1000;  // Player基础优先级
    [Tooltip("是否强制Player始终在最前")]
    public bool playerAlwaysFront = false;
    [Tooltip("Player优先级的Y轴影响权重")]
    [Range(0, 1)]
    public float playerYInfluence = 0.5f;  // Y轴对Player的影响程度

    [Header("Y轴排序设置")]
    [Tooltip("正数：Y越大越靠前；负数：Y越大越靠后")]
    public float yPositionMultiplier = 10f;

    [Header("静态物体设置")]
    public bool isStaticObject = false;
    public float staticObjectYOffset = 0f;

    [Header("锚点设置")]
    public bool useCustomPivot = false;
    public Vector2 customPivotOffset = Vector2.zero;

    [Header("更新设置")]
    public UpdateTiming updateTiming = UpdateTiming.Update;
    public float updateInterval = 0.1f;

    [Header("穿透效果设置")]
    public bool enablePassingEffect = true;
    [Range(0.1f, 2f)]
    public float detectionRadius = 0.8f;
    [Range(0f, 1f)]
    public float minAlpha = 0.3f;
    [Range(0.1f, 1f)]
    public float fadeSpeed = 0.5f;
    public LayerMask targetLayers = -1;

    [Header("调试")]
    public bool showDebugInfo = true;
    public Color debugColor = Color.yellow;

    private SpriteRenderer spriteRenderer;
    private float lastUpdateTime;
    private int currentOrder;

    // 穿透效果相关
    private Dictionary<Collider2D, float> nearbyObjects = new Dictionary<Collider2D, float>();
    private float targetAlpha = 1f;
    private float currentAlpha = 1f;
    private Collider2D objectCollider;

    // 记录所有其他DynamicLayerSorter
    private static List<DynamicLayerSorter> allSorters = new List<DynamicLayerSorter>();

    public enum SortingMethod
    {
        ByYPosition,
        ByLayer,
        CustomOrder
    }

    public enum UpdateTiming
    {
        Update,
        Interval,
        Manual
    }

    void OnEnable()
    {
        // 注册到全局列表
        if (!allSorters.Contains(this))
        {
            allSorters.Add(this);
        }
    }

    void OnDisable()
    {
        // 从全局列表移除
        allSorters.Remove(this);
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"{gameObject.name} 没有SpriteRenderer组件，DynamicLayerSorter将无效");
            return;
        }

        // 获取或添加Collider用于检测
        objectCollider = GetComponent<Collider2D>();
        if (objectCollider == null && enablePassingEffect)
        {
            objectCollider = gameObject.AddComponent<CircleCollider2D>();
            ((CircleCollider2D)objectCollider).radius = detectionRadius;
            objectCollider.isTrigger = true;
        }

        // 初始排序
        UpdateSortingOrder();
        currentAlpha = spriteRenderer.color.a;

        if (showDebugInfo)
        {
            Debug.Log($"[{gameObject.name}] 初始排序: {currentOrder}, Y位置: {transform.position.y:F2}, IsPlayer: {isPlayer}");
        }
    }

    void Update()
    {
        if (spriteRenderer == null) return;

        // 更新排序
        if (updateTiming == UpdateTiming.Update)
        {
            UpdateSortingOrder();
        }
        else if (updateTiming == UpdateTiming.Interval)
        {
            if (Time.time - lastUpdateTime >= updateInterval)
            {
                UpdateSortingOrder();
                lastUpdateTime = Time.time;
            }
        }

        // 更新穿透效果
        if (enablePassingEffect)
        {
            UpdatePassingEffect();
        }
    }

    void UpdatePassingEffect()
    {
        // 清理无效的物体
        List<Collider2D> toRemove = new List<Collider2D>();
        foreach (var kv in nearbyObjects)
        {
            if (kv.Key == null || !kv.Key.gameObject.activeInHierarchy)
            {
                toRemove.Add(kv.Key);
            }
        }
        foreach (var key in toRemove)
        {
            nearbyObjects.Remove(key);
        }

        // 计算目标透明度
        if (nearbyObjects.Count > 0)
        {
            targetAlpha = minAlpha;
        }
        else
        {
            targetAlpha = 1f;
        }

        // 平滑过渡
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed * 10f);

        // 应用透明度
        Color color = spriteRenderer.color;
        color.a = currentAlpha;
        spriteRenderer.color = color;
    }

    // 触发器检测 - 进入范围
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!enablePassingEffect) return;

        // 检查是否是需要检测的层
        if ((targetLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            if (!nearbyObjects.ContainsKey(other))
            {
                nearbyObjects.Add(other, Time.time);
                if (showDebugInfo)
                {
                    Debug.Log($"[{gameObject.name}] 遇到 {other.gameObject.name}");
                }
            }
        }
    }

    // 触发器检测 - 离开范围
    void OnTriggerExit2D(Collider2D other)
    {
        if (!enablePassingEffect) return;

        if (nearbyObjects.ContainsKey(other))
        {
            nearbyObjects.Remove(other);
            if (showDebugInfo)
            {
                Debug.Log($"[{gameObject.name}] 离开 {other.gameObject.name}");
            }
        }
    }

    public void UpdateSortingOrder()
    {
        if (spriteRenderer == null) return;

        int newOrder = CalculateSortingOrder();

        if (newOrder != currentOrder)
        {
            spriteRenderer.sortingOrder = newOrder;
            currentOrder = newOrder;

            if (showDebugInfo)
            {
                Debug.Log($"[{gameObject.name}] 排序更新: {newOrder}, Y位置: {transform.position.y:F2}");
            }
        }
    }

    private int CalculateSortingOrder()
    {
        int order = 0;

        switch (sortingMethod)
        {
            case SortingMethod.ByYPosition:
                order = CalculateYBasedOrder();
                break;
            case SortingMethod.ByLayer:
                order = baseSortingOrder + gameObject.layer * 100;
                break;
            case SortingMethod.CustomOrder:
                order = baseSortingOrder;
                break;
        }

        // 如果是Player，应用Player优先级逻辑
        if (isPlayer)
        {
            order = CalculatePlayerOrder(order);
        }

        return order;
    }

    private int CalculateYBasedOrder()
    {
        float yPos;

        if (useCustomPivot)
        {
            yPos = transform.position.y + customPivotOffset.y;
        }
        else
        {
            yPos = transform.position.y;
        }

        if (isStaticObject)
        {
            yPos += staticObjectYOffset;
        }

        int order = baseSortingOrder + Mathf.RoundToInt(yPos * yPositionMultiplier);
        order = Mathf.Clamp(order, -32768, 32767);

        return order;
    }

    private int CalculatePlayerOrder(int baseYOrder)
    {
        if (playerAlwaysFront)
        {
            // 强制Player在最前：找出当前最大Order，然后加1
            int maxOrder = GetMaxOrderAmongOthers();
            return maxOrder + 1;
        }
        else
        {
            // 混合模式：Player优先级 + Y轴影响
            // 将Y轴位置映射到0-100的范围
            float yInfluenceValue = Mathf.Clamp01((transform.position.y + 10f) / 20f) * 100f;
            // 混合计算
            return playerPriority + Mathf.RoundToInt(yInfluenceValue * playerYInfluence);
        }
    }

    private int GetMaxOrderAmongOthers()
    {
        int maxOrder = int.MinValue;

        foreach (var sorter in allSorters)
        {
            if (sorter != null && sorter != this && sorter.spriteRenderer != null)
            {
                int order = sorter.spriteRenderer.sortingOrder;
                if (order > maxOrder)
                {
                    maxOrder = order;
                }
            }
        }

        return maxOrder == int.MinValue ? 0 : maxOrder;
    }

    // 获取当前所有物体的最大排序值
    public static int GetGlobalMaxOrder()
    {
        int maxOrder = int.MinValue;

        foreach (var sorter in allSorters)
        {
            if (sorter != null && sorter.spriteRenderer != null)
            {
                int order = sorter.spriteRenderer.sortingOrder;
                if (order > maxOrder)
                {
                    maxOrder = order;
                }
            }
        }

        return maxOrder;
    }

    // 获取当前所有物体的最小排序值
    public static int GetGlobalMinOrder()
    {
        int minOrder = int.MaxValue;

        foreach (var sorter in allSorters)
        {
            if (sorter != null && sorter.spriteRenderer != null)
            {
                int order = sorter.spriteRenderer.sortingOrder;
                if (order < minOrder)
                {
                    minOrder = order;
                }
            }
        }

        return minOrder;
    }

    public void ForceUpdate()
    {
        UpdateSortingOrder();
    }

    // 手动设置透明度
    public void SetAlpha(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = Mathf.Clamp01(alpha);
            spriteRenderer.color = color;
            currentAlpha = color.a;
            targetAlpha = currentAlpha;
        }
    }

    // 重置透明度
    public void ResetAlpha()
    {
        targetAlpha = 1f;
    }

    void OnValidate()
    {
        if (Application.isPlaying) return;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            int previewOrder = CalculateSortingOrder();
            spriteRenderer.sortingOrder = previewOrder;
        }

        // 更新检测半径
        if (objectCollider != null && objectCollider is CircleCollider2D)
        {
            ((CircleCollider2D)objectCollider).radius = detectionRadius;
        }
    }

    void OnDrawGizmos()
    {
        if (!showDebugInfo || spriteRenderer == null) return;

        // 显示检测范围
        if (enablePassingEffect)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }

        // 显示排序信息
        Vector3 textPos = transform.position + Vector3.up * 1.5f;

#if UNITY_EDITOR
        UnityEditor.Handles.BeginGUI();
        var style = new GUIStyle();
        style.normal.textColor = debugColor;
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;

        Vector3 screenPos = Camera.current.WorldToScreenPoint(textPos);
        if (screenPos.z > 0)
        {
            string debugText = $"Order: {spriteRenderer.sortingOrder}";
            if (isPlayer)
            {
                debugText += "\n[PLAYER]";
            }
            debugText += $"\nAlpha: {currentAlpha:F2}";
            GUI.Label(new Rect(screenPos.x - 50, Screen.height - screenPos.y - 60, 120, 70), debugText, style);
        }
        UnityEditor.Handles.EndGUI();
#endif
    }

    void OnDestroy()
    {
        // 清理
        nearbyObjects.Clear();
        allSorters.Remove(this);
    }
}