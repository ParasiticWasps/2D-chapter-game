using UnityEngine;

public class SwingAnimation : MonoBehaviour
{
    [Header("摆动设置")]
    public float swingAngle = 10f;           // 最大摆动角度
    public float swingSpeed = 2f;             // 摆动速度
    public bool isSwinging = false;           // 是否正在摆动

    [Header("靠近检测")]
    public float activationDistance = 3f;      // 开始摆动的距离
    public Transform playerTransform;          // 玩家位置

    private Quaternion initialRotation;
    private float swingTimer = 0f;

    void Start()
    {
        initialRotation = transform.rotation;

        // 如果没有设置玩家，尝试查找
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }

    void Update()
    {
        // 检测玩家距离
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            isSwinging = distance <= activationDistance;
        }

        // 应用摆动
        if (isSwinging)
        {
            swingTimer += Time.deltaTime * swingSpeed;

            // 使用正弦波计算摆动角度
            float angle = Mathf.Sin(swingTimer) * swingAngle;
            transform.rotation = initialRotation * Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // 缓慢回到初始位置
            transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, Time.deltaTime * 2f);
            swingTimer = 0f;
        }
    }

    // 强制开始/停止摆动（供外部调用）
    public void SetSwinging(bool swinging)
    {
        isSwinging = swinging;
    }

    // 立即停止摆动（用于印玺公出现时）
    public void StopImmediately()
    {
        isSwinging = false;
        transform.rotation = initialRotation;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}