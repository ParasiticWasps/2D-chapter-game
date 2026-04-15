using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSort : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("排序平滑速度")]
    public float sortLerpSpeed = 10f;

    [Header("脚底高度偏移（正值向下）")]
    public float heightOffset = 0f;

    [Header("是否使用碰撞盒底部自动计算")]
    public bool useColliderBottom = false;

    private float currentOrder;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        currentOrder = GetSortY() * -100;
    }

    void LateUpdate()
    {
        float targetOrder = GetSortY() * -100;

        currentOrder = Mathf.Lerp(currentOrder, targetOrder, Time.deltaTime * sortLerpSpeed);
        sr.sortingOrder = Mathf.RoundToInt(currentOrder);
    }

    float GetSortY()
    {
        if (useColliderBottom)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                return col.bounds.min.y; // 使用碰撞体底部
            }
        }

        // 使用自定义偏移
        return transform.position.y - heightOffset;
    }
}