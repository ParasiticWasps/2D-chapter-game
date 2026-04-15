using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class FloatingTextController : MonoBehaviour
{
    [Header("文字设置")]
    public float displayDuration = 3f;        // 显示时间
    public float fadeOutDuration = 0.5f;      // 淡出时间
    public float verticalOffset = 2f;         // 头顶偏移量

    private Text textComponent;
    private CanvasGroup canvasGroup;
    private Transform targetTransform;         // 要跟随的目标（宫女）
    private Coroutine displayCoroutine;




    private FloatingTextPool pool;

    public void SetPool(FloatingTextPool pool)
    {
        this.pool = pool;
    }

    void Awake()
    {
        textComponent = GetComponentInChildren<Text>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Update()
    {
        // 跟随目标
        if (targetTransform != null)
        {
            // 保持在世界空间中的位置：目标头顶 + 偏移
            transform.position = targetTransform.position + Vector3.up * verticalOffset;
        }
    }

    // 显示文字
    public void ShowText(string content, Transform target)
    {
        targetTransform = target;

        if (textComponent != null)
            textComponent.text = content;

        // 重置透明度
        canvasGroup.alpha = 1f;

        // 停止之前的协程
        if (displayCoroutine != null)
            StopCoroutine(displayCoroutine);

        // 开始新的显示协程
        displayCoroutine = StartCoroutine(DisplayRoutine());
    }

    IEnumerator DisplayRoutine()
    {
        // 等待显示时长
        yield return new WaitForSeconds(displayDuration);

        // 淡出效果
        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeOutDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;

        // 如果存在对象池，则回收；否则销毁
        if (pool != null)
            pool.ReturnFloatingText(gameObject);
        else
            Destroy(gameObject);
    }

    // 立即隐藏（用于当玩家离开范围时提前消失）
    public void HideImmediately()
    {
        if (displayCoroutine != null)
            StopCoroutine(displayCoroutine);

        if (pool != null)
            pool.ReturnFloatingText(gameObject);
        else
            Destroy(gameObject);
    }
}