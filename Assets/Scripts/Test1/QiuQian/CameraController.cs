using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("相机设置")]
    public Transform target;  // 默认跟随目标（玩家）
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("边界限制")]
    public bool limitBounds = true;
    public float minX, maxX, minY, maxY;

    [Header("聚焦设置")]
    public float focusDuration = 4f;  // 聚焦持续时间
    public float focusZoom = 5f;      // 聚焦时的相机大小（Orthographic Size）
    public float focusSmoothSpeed = 2f;

    private Camera cam;
    private float defaultZoom;
    private Transform focusTarget;
    private bool isFocusing = false;
    private Coroutine focusCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            defaultZoom = cam.orthographicSize;
        }

        // 如果没有设置目标，尝试找玩家
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    void LateUpdate()
    {
        if (isFocusing && focusTarget != null)
        {
            // 聚焦模式：跟随聚焦目标
            Vector3 desiredPosition = focusTarget.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * 2f);
        }
        else if (target != null && !isFocusing)
        {
            // 正常模式：跟随玩家
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            if (limitBounds)
            {
                smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
                smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);
            }

            transform.position = smoothedPosition;
        }
    }

    // 聚焦到指定位置
    public void FocusOnPosition(Vector3 position, float duration = -1f)
    {
        if (focusCoroutine != null)
            StopCoroutine(focusCoroutine);

        focusCoroutine = StartCoroutine(FocusRoutine(position, duration));
    }

    // 聚焦到指定物体
    public void FocusOnTarget(Transform newTarget, float duration = -1f)
    {
        if (newTarget == null) return;

        if (focusCoroutine != null)
            StopCoroutine(focusCoroutine);

        focusCoroutine = StartCoroutine(FocusRoutine(newTarget, duration));
    }

    // 恢复到默认跟随
    public void ReturnToDefault()
    {
        if (focusCoroutine != null)
            StopCoroutine(focusCoroutine);

        focusCoroutine = StartCoroutine(ReturnRoutine());
    }

    IEnumerator FocusRoutine(Transform focusTransform, float customDuration)
    {
        isFocusing = true;
        focusTarget = focusTransform;

        float duration = customDuration > 0 ? customDuration : focusDuration;
        float elapsed = 0f;

        // 平滑调整相机大小
        if (cam != null)
        {
            float startZoom = cam.orthographicSize;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * focusSmoothSpeed;
                cam.orthographicSize = Mathf.Lerp(startZoom, focusZoom, elapsed);
                yield return null;
            }
            cam.orthographicSize = focusZoom;
        }

        // 等待指定时间
        yield return new WaitForSeconds(duration);

        // 返回默认
        yield return StartCoroutine(ReturnRoutine());
    }

    IEnumerator FocusRoutine(Vector3 position, float customDuration)
    {
        isFocusing = true;
        focusTarget = null;

        float duration = customDuration > 0 ? customDuration : focusDuration;
        float elapsed = 0f;

        // 移动到指定位置
        Vector3 startPos = transform.position;
        Vector3 targetPos = position + offset;

        // 平滑调整相机大小
        if (cam != null)
        {
            float startZoom = cam.orthographicSize;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * focusSmoothSpeed;

                // 移动相机
                transform.position = Vector3.Lerp(startPos, targetPos, elapsed);

                // 调整大小
                cam.orthographicSize = Mathf.Lerp(startZoom, focusZoom, elapsed);

                yield return null;
            }
            cam.orthographicSize = focusZoom;
        }

        transform.position = targetPos;

        // 等待指定时间
        yield return new WaitForSeconds(duration);

        // 返回默认
        yield return StartCoroutine(ReturnRoutine());
    }

    IEnumerator ReturnRoutine()
    {
        if (target == null)
        {
            isFocusing = false;
            yield break;
        }

        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = target.position + offset;

        float startZoom = cam != null ? cam.orthographicSize : defaultZoom;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * focusSmoothSpeed;

            // 移动回玩家
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed);

            // 恢复默认大小
            if (cam != null)
            {
                cam.orthographicSize = Mathf.Lerp(startZoom, defaultZoom, elapsed);
            }

            yield return null;
        }

        transform.position = targetPos;
        if (cam != null)
            cam.orthographicSize = defaultZoom;

        isFocusing = false;
        focusTarget = null;
    }

    // 立即设置相机位置（用于初始化）
    public void TeleportToTarget()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (limitBounds)
        {
            Gizmos.color = Color.cyan;
            Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
            Vector3 size = new Vector3(maxX - minX, maxY - minY, 0);
            Gizmos.DrawWireCube(center, size);
        }
    }
}