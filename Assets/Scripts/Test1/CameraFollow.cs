using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;

    [Header("跟随设置")]
    [Tooltip("平滑跟随速度")]
    public float smoothSpeed = 0.125f;
    [Tooltip("摄像机偏移量（相对于目标）")]
    public Vector3 offset;

    [Header("边界限制")]
    public bool limitBounds = true;
    public float minX, maxX, minY, maxY;

    void LateUpdate()
    {
        if (target == null) return;

        // 期望位置 = 目标位置 + 偏移
        Vector3 desiredPosition = target.position + offset;
        // 平滑插值
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 边界限制
        if (limitBounds)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);
        }

        transform.position = smoothedPosition;
    }
}