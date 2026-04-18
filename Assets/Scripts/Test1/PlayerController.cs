using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("移动速度")]
    public float moveSpeed = 5f;
    [Tooltip("移动目标位置的最小距离，小于此值停止移动")]
    public float stopDistance = 0.1f;

    [Header("移动边界 - 基础设置")]
    public bool useBoundary = true;
    public BoundaryType boundaryType = BoundaryType.Rectangle;

    [Header("矩形边界")]
    [Tooltip("最小 X 坐标")]
    public float minX = -10f;
    [Tooltip("最大 X 坐标")]
    public float maxX = 10f;
    [Tooltip("最小 Y 坐标")]
    public float minY = -10f;
    [Tooltip("最大 Y 坐标")]
    public float maxY = 10f;

    [Header("圆形边界")]
    public Vector2 circleCenter = Vector2.zero;
    public float circleRadius = 10f;

    [Header("自定义边界点（多边形）")]
    public Vector2[] boundaryPoints = new Vector2[]
    {
        new Vector2(-10, -5),
        new Vector2(10, -5),
        new Vector2(10, 5),
        new Vector2(-10, 5)
    };

    [Header("边界可视化")]
    public Color boundaryColor = Color.cyan;
    public bool showBoundaryGizmos = true;

    [Header("动画组件")]
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private float currentSpeed = 0f;

    private GameStateManager gameStateManager;

    // 边界类型枚举
    public enum BoundaryType
    {
        Rectangle,  // 矩形边界
        Circle,     // 圆形边界
        Polygon     // 多边形边界
    }

    void Start()
    {
        targetPosition = transform.position;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        gameStateManager = FindObjectOfType<GameStateManager>();
    }

    void Update()
    {
        // 检测鼠标左键点击（只有Normal状态下才能移动）
        if (Input.GetMouseButtonDown(0))
        {
            bool canMove = gameStateManager == null || gameStateManager.CanPlayerMove();

            if (canMove)
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0;
                SetTargetPosition(mouseWorldPos);
            }
        }

        bool canMoveNow = gameStateManager == null || gameStateManager.CanPlayerMove();

        if (canMoveNow)
        {
            if (isMoving)
            {
                float step = moveSpeed * Time.deltaTime;
                Vector3 moveDirection = (targetPosition - transform.position).normalized;

                if (moveDirection.x != 0)
                {
                    spriteRenderer.flipX = moveDirection.x < 0;
                }

                transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
                currentSpeed = moveSpeed;

                if (Vector3.Distance(transform.position, targetPosition) < stopDistance)
                {
                    transform.position = targetPosition;
                    isMoving = false;
                    currentSpeed = 0f;
                }
            }
            else
            {
                currentSpeed = 0f;
            }
        }
        else
        {
            isMoving = false;
            currentSpeed = 0f;
        }

        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
            animator.SetFloat("Speed", currentSpeed);
        }
    }

    public void SetTargetPosition(Vector3 pos)
    {
        if (gameStateManager != null && !gameStateManager.CanPlayerMove())
        {
            return;
        }

        // 根据边界类型限制位置
        if (useBoundary)
        {
            pos = ClampPosition(pos);
        }

        targetPosition = pos;
        isMoving = true;
    }

    /// <summary>
    /// 根据边界类型限制位置
    /// </summary>
    private Vector3 ClampPosition(Vector3 pos)
    {
        switch (boundaryType)
        {
            case BoundaryType.Rectangle:
                pos.x = Mathf.Clamp(pos.x, minX, maxX);
                pos.y = Mathf.Clamp(pos.y, minY, maxY);
                break;

            case BoundaryType.Circle:
                Vector2 direction = new Vector2(pos.x, pos.y) - circleCenter;
                if (direction.magnitude > circleRadius)
                {
                    direction = direction.normalized * circleRadius;
                    pos = new Vector3(circleCenter.x + direction.x, circleCenter.y + direction.y, pos.z);
                }
                break;

            case BoundaryType.Polygon:
                if (boundaryPoints.Length >= 3)
                {
                    pos = ClampToPolygon(pos);
                }
                break;
        }

        return pos;
    }

    /// <summary>
    /// 限制到多边形边界（简单实现：找到最近的多边形边）
    /// </summary>
    private Vector3 ClampToPolygon(Vector3 pos)
    {
        if (!IsPointInPolygon(pos))
        {
            // 找到最近的多边形顶点
            float minDist = float.MaxValue;
            Vector3 closestPoint = pos;

            for (int i = 0; i < boundaryPoints.Length; i++)
            {
                Vector3 point = new Vector3(boundaryPoints[i].x, boundaryPoints[i].y, 0);
                float dist = Vector3.Distance(pos, point);

                if (dist < minDist)
                {
                    minDist = dist;
                    closestPoint = point;
                }
            }

            return closestPoint;
        }

        return pos;
    }

    /// <summary>
    /// 判断点是否在多边形内（射线法）
    /// </summary>
    private bool IsPointInPolygon(Vector3 point)
    {
        int j = boundaryPoints.Length - 1;
        bool inside = false;

        for (int i = 0; i < boundaryPoints.Length; i++)
        {
            Vector2 pi = boundaryPoints[i];
            Vector2 pj = boundaryPoints[j];

            if (((pi.y > point.y) != (pj.y > point.y)) &&
                (point.x < (pj.x - pi.x) * (point.y - pi.y) / (pj.y - pi.y) + pi.x))
            {
                inside = !inside;
            }

            j = i;
        }

        return inside;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public bool IsFacingLeft()
    {
        return spriteRenderer != null && spriteRenderer.flipX;
    }

    [Header("动画触发器")]
    public string bowTriggerName = "Bow";

    public void PlayBowAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(bowTriggerName);
            Debug.Log("播放行礼动画");
        }
    }

    public void PlayIdelAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
        }
    }

    public bool IsBowing()
    {
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("Bow");
        }
        return false;
    }

    // 可视化边界
    void OnDrawGizmosSelected()
    {
        if (!showBoundaryGizmos) return;

        Gizmos.color = boundaryColor;

        switch (boundaryType)
        {
            case BoundaryType.Rectangle:
                Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
                Vector3 size = new Vector3(maxX - minX, maxY - minY, 0);
                Gizmos.DrawWireCube(center, size);
                break;

            case BoundaryType.Circle:
                Gizmos.DrawWireSphere(new Vector3(circleCenter.x, circleCenter.y, 0), circleRadius);
                break;

            case BoundaryType.Polygon:
                if (boundaryPoints.Length >= 3)
                {
                    for (int i = 0; i < boundaryPoints.Length; i++)
                    {
                        Vector3 p1 = new Vector3(boundaryPoints[i].x, boundaryPoints[i].y, 0);
                        Vector3 p2 = new Vector3(boundaryPoints[(i + 1) % boundaryPoints.Length].x,
                                                boundaryPoints[(i + 1) % boundaryPoints.Length].y, 0);
                        Gizmos.DrawLine(p1, p2);
                    }
                }
                break;
        }
    }

 
}
