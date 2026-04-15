using UnityEngine;
using System.Collections;

public class SwingInteraction : MonoBehaviour
{
    [Header("组件引用")]
    public SwingAnimation swingAnimation;
    public LaughSoundController laughSound;
    public GameObject 印玺公Prefab;
    public PlayerController playerController;

    [Header("场景引用")]
    public ChapterEndController chapterEndController;

    [Header("聚焦效果设置")]
    public bool useVignetteFocus = true;
    public float focusVignetteDuration = 3f;
    public float focusVignetteIntensity = 0.4f;
    public float focusDelay = 0.5f;
    public bool waitForFocusComplete = true;  // 是否等待聚焦完全结束

    [Header("点击设置")]
    public float clickDistance = 2f;
    public bool debugClickDistance = true;

    [Header("剧情文本")]
    [TextArea(3, 5)]
    public string playerRecallText = "这秋千，让我想起邻家小妹阿阮，她最爱在春日里荡秋千了......";

    [TextArea(2, 3)]
    public string playerGuessText = "估摸着是要指点我的画作。";

    public string 印玺公Dialog = "这里空着便是，莫要画蛇添足。";

    [TextArea(3, 5)]
    public string finalMonologue = "这空着的秋千，空着的琴案，宫女的落寞......感觉少了点什么，这都是应该空着的吗。";

    [Header("印玺公设置")]
    public float 印玺公出现Delay = 0.5f;
    public Vector3 印玺公出现位置;
    public Vector3 印玺公移动目标位置;
    public float 印玺公移动速度 = 2f;
    public float 印玺公停留时间 = 1f;
    public float 印玺公渐隐时间 = 2f;
    public Vector3 印玺公离开方向 = new Vector3(5f, 0, 0);

    [Header("留白设置")]
    public float silenceDuration = 2f;
    public float finalMonologueDelay = 1f;

    private bool hasClicked = false;
    private GameObject 印玺公Instance;
    private SceneEffectManager effectManager;
    private Transform playerTransform;
    private GameStateManager gameStateManager;
    private Animator playerAnimator;

    void Start()
    {
        if (swingAnimation == null)
            swingAnimation = GetComponent<SwingAnimation>();

        if (laughSound == null)
            laughSound = GetComponent<LaughSoundController>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerAnimator = player.GetComponent<Animator>();

            if (playerController == null)
                playerController = player.GetComponent<PlayerController>();
        }

        effectManager = FindObjectOfType<SceneEffectManager>();
        if (effectManager == null)
        {
            Debug.LogError("场景中需要SceneEffectManager！");
        }

        gameStateManager = FindObjectOfType<GameStateManager>();

        // 设置默认位置
        if (印玺公出现位置 == Vector3.zero)
        {
            印玺公出现位置 = transform.position + new Vector3(-3f, 0, 0);
        }

        if (印玺公移动目标位置 == Vector3.zero)
        {
            印玺公移动目标位置 = transform.position + new Vector3(1f, 0, 0);
        }
    }

    void Update()
    {
        // 只有在Normal状态下才检测笑声
        if (gameStateManager != null && gameStateManager.currentState != GameState.Normal)
        {
            if (laughSound != null)
            {
                laughSound.SetPlayerNear(false);
            }
            return;
        }

        // 检测玩家距离，控制笑声
        if (swingAnimation != null && swingAnimation.playerTransform != null)
        {
            float distance = Vector3.Distance(
                transform.position,
                swingAnimation.playerTransform.position
            );

            bool isNear = distance <= swingAnimation.activationDistance;

            if (laughSound != null)
            {
                laughSound.SetPlayerNear(isNear);
            }
        }
    }

    void OnMouseDown()
    {
        // 检查是否可以互动
        if (gameStateManager != null && !gameStateManager.CanInteract())
        {
            Debug.Log("当前状态无法与秋千互动");
            return;
        }

        if (hasClicked) return;

        if (playerTransform == null)
        {
            Debug.LogWarning("找不到玩家，无法检查点击距离");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (debugClickDistance)
        {
            Debug.Log($"点击秋千，玩家距离：{distanceToPlayer:F2}，有效点击距离：{clickDistance}");
        }

        if (distanceToPlayer <= clickDistance)
        {
            Debug.Log("有效点击，触发完整剧情");
            hasClicked = true;
            StartCoroutine(FullStorySequence());
        }
        else
        {
            Debug.Log("距离太远，无法触发剧情");
        }
    }


    System.Collections.IEnumerator FullStorySequence()
    {
        // 切换到回忆状态
        if (gameStateManager != null)
            gameStateManager.SetState(GameState.Recall);

        // 先延迟一小段时间，让点击感更舒适
        yield return new WaitForSeconds(focusDelay);

        // 褪色效果：降低饱和度，降低环境音
        if (effectManager != null)
        {
            effectManager.SetRecallMode();
            Debug.Log($"触发褪色效果，持续时间：{focusVignetteDuration}秒");
        }

        // 等待褪色效果持续一段时间（让玩家感受到画面变化）
        yield return new WaitForSeconds(focusVignetteDuration);

        // 褪色效果结束，恢复正常（但后续剧情可能还会再降低，此处先恢复一部分）
        // 注意：后面剧情会再次调用 SetRecallMode 或 SetBlankMode，这里先恢复避免重叠
        if (effectManager != null)
        {
            effectManager.SetNormal();
        }

        // 2. Player内心独白1（褪色效果结束后开始播放剧情）
        Debug.Log("触发回忆独白");
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.ShowDialog(playerRecallText, null);
        }

        // 等待文本显示完成
        yield return new WaitForSeconds(3f);

        // 3. 印玺公出现（后续剧情中会再次调用 SetRecallMode 来保持氛围）
        // 为了剧情连贯，在印玺公出现前再次降低饱和度
        if (effectManager != null)
        {
            effectManager.SetRecallMode();
        }

        // 3. 印玺公出现
        Debug.Log("印玺公出现");
        if (印玺公Prefab != null)
        {
            印玺公Instance = Instantiate(印玺公Prefab, 印玺公出现位置, Quaternion.identity);

            SpriteRenderer renderer = 印玺公Instance.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Color c = renderer.color;
                c.a = 0;
                renderer.color = c;

                float elapsed = 0f;
                float fadeInTime = 1f;
                while (elapsed < fadeInTime)
                {
                    elapsed += Time.deltaTime;
                    c.a = Mathf.Lerp(0, 1, elapsed / fadeInTime);
                    renderer.color = c;
                    yield return null;
                }
            }
        }

        // 4. Player播放行礼动画和内心独白2
        Debug.Log("Player行礼");
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Bow");
        }

        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.ShowDialog(playerGuessText, null);
        }

        yield return new WaitForSeconds(2f);

        // 5. 印玺公移动到秋千附近
        Debug.Log("印玺公移动到秋千");
        if (印玺公Instance != null)
        {
            yield return StartCoroutine(Move印玺公ToPosition(印玺公Instance, 印玺公移动目标位置));
        }

        // 6. 印玺公稍作停留
        yield return new WaitForSeconds(印玺公停留时间);

        // 7. 印玺公说话
        Debug.Log("印玺公说话");
        if (DialogManager.Instance != null && 印玺公Instance != null)
        {
            DialogManager.Instance.ShowDialog(印玺公Dialog, null);
        }

        yield return new WaitForSeconds(2f);

        // 8. 印玺公离开并渐隐
        Debug.Log("印玺公离开");
        if (印玺公Instance != null)
        {
            Vector3 leavePosition = 印玺公Instance.transform.position + 印玺公离开方向;
            yield return StartCoroutine(MoveAndFade印玺公(印玺公Instance, leavePosition));
        }

        // 9. 留白时刻开始
        Debug.Log("留白时刻");
        if (gameStateManager != null)
            gameStateManager.SetState(GameState.Blank);

        if (swingAnimation != null)
            swingAnimation.StopImmediately();

        if (laughSound != null)
            laughSound.StopImmediately();

        if (effectManager != null)
        {
            effectManager.SetBlankMode();
            effectManager.TriggerSilence(silenceDuration);
        }

        yield return new WaitForSeconds(silenceDuration);

        // 10. 最后独白
        Debug.Log("最后独白");
        if (effectManager != null)
        {
            effectManager.SetNormal();
        }

        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.ShowDialog(finalMonologue, () => {
                if (chapterEndController != null)
                {
                    chapterEndController.StartChapterEnd();
                }
                Debug.Log("剧情结束");
            });
        }

        // 11. 完全恢复
        yield return new WaitForSeconds(3f);

        if (effectManager != null)
        {
            effectManager.SetNormal();
        }

        if (gameStateManager != null)
            gameStateManager.SetState(GameState.Normal);

        Debug.Log("完整剧情结束");
    }

    //System.Collections.IEnumerator FullStorySequence()
    //{
    //    // 切换到回忆状态
    //    if (gameStateManager != null)
    //        gameStateManager.SetState(GameState.Recall);

    //    // 先延迟一小段时间，让点击感更舒适
    //    yield return new WaitForSeconds(focusDelay);

    //    // 触发聚焦效果（用Vignette）
    //    if (useVignetteFocus && effectManager != null)
    //    {
    //        Debug.Log($"触发Vignette聚焦效果，持续时间：{focusVignetteDuration}秒");

    //        // 触发Vignette，但不等待它结束（如果waitForFocusComplete为false）
    //        effectManager.TriggerFocusVignette(focusVignetteDuration, focusVignetteIntensity);

    //        // 如果需要等待聚焦完全结束再播剧情
    //        if (waitForFocusComplete)
    //        {
    //            // 等待Vignette效果完全结束（持续时间 + 淡入淡出时间）
    //            // Vignette效果有0.5秒淡入 + duration + 0.5秒淡出
    //            float totalFocusTime = focusVignetteDuration + 1f; // +1秒用于淡入淡出
    //            Debug.Log($"等待聚焦效果完全结束：{totalFocusTime}秒");
    //            yield return new WaitForSeconds(totalFocusTime);
    //        }
    //        else
    //        {
    //            // 只等待一小段时间让聚焦效果开始，就开始剧情
    //            yield return new WaitForSeconds(0.5f);
    //        }
    //    }

    //    // 1. 降低饱和度，降低环境音（开始进入回忆氛围）
    //    if (effectManager != null)
    //    {
    //        effectManager.SetRecallMode();
    //    }

    //    // 2. Player内心独白1
    //    Debug.Log("触发回忆独白");
    //    if (DialogManager.Instance != null)
    //    {
    //        DialogManager.Instance.ShowDialog(playerRecallText, null);
    //    }

    //    // 等待文本显示完成
    //    yield return new WaitForSeconds(3f);

    //    // 3. 印玺公出现
    //    Debug.Log("印玺公出现");
    //    if (印玺公Prefab != null)
    //    {
    //        印玺公Instance = Instantiate(印玺公Prefab, 印玺公出现位置, Quaternion.identity);

    //        SpriteRenderer renderer = 印玺公Instance.GetComponent<SpriteRenderer>();
    //        if (renderer != null)
    //        {
    //            Color c = renderer.color;
    //            c.a = 0;
    //            renderer.color = c;

    //            float elapsed = 0f;
    //            float fadeInTime = 1f;
    //            while (elapsed < fadeInTime)
    //            {
    //                elapsed += Time.deltaTime;
    //                c.a = Mathf.Lerp(0, 1, elapsed / fadeInTime);
    //                renderer.color = c;
    //                yield return null;
    //            }
    //        }
    //    }

    //    // 4. Player播放行礼动画和内心独白2
    //    Debug.Log("Player行礼");
    //    if (playerAnimator != null)
    //    {
    //        playerAnimator.SetTrigger("Bow");
    //    }

    //    if (DialogManager.Instance != null)
    //    {
    //        DialogManager.Instance.ShowDialog(playerGuessText, null);
    //    }

    //    yield return new WaitForSeconds(2f);

    //    // 5. 印玺公移动到秋千附近
    //    Debug.Log("印玺公移动到秋千");
    //    if (印玺公Instance != null)
    //    {
    //        yield return StartCoroutine(Move印玺公ToPosition(印玺公Instance, 印玺公移动目标位置));
    //    }

    //    // 6. 印玺公稍作停留
    //    yield return new WaitForSeconds(印玺公停留时间);

    //    // 7. 印玺公说话
    //    Debug.Log("印玺公说话");
    //    if (DialogManager.Instance != null && 印玺公Instance != null)
    //    {
    //        DialogManager.Instance.ShowDialog(印玺公Dialog, null);
    //    }

    //    yield return new WaitForSeconds(2f);

    //    // 8. 印玺公离开并渐隐
    //    Debug.Log("印玺公离开");
    //    if (印玺公Instance != null)
    //    {
    //        Vector3 leavePosition = 印玺公Instance.transform.position + 印玺公离开方向;
    //        yield return StartCoroutine(MoveAndFade印玺公(印玺公Instance, leavePosition));
    //    }

    //    // 9. 留白时刻开始
    //    Debug.Log("留白时刻");
    //    if (gameStateManager != null)
    //        gameStateManager.SetState(GameState.Blank);

    //    if (swingAnimation != null)
    //        swingAnimation.StopImmediately();

    //    if (laughSound != null)
    //        laughSound.StopImmediately();

    //    if (effectManager != null)
    //    {
    //        effectManager.SetBlankMode();
    //        effectManager.TriggerSilence(silenceDuration);
    //    }

    //    yield return new WaitForSeconds(silenceDuration);

    //    // 10. 最后独白
    //    Debug.Log("最后独白");
    //    if (effectManager != null)
    //    {
    //        effectManager.SetNormal();
    //    }

    //    if (DialogManager.Instance != null)
    //    {
    //        DialogManager.Instance.ShowDialog(finalMonologue, () => {
    //            if (chapterEndController != null)
    //            {
    //                chapterEndController.StartChapterEnd();
    //            }
    //            Debug.Log("剧情结束");
    //        });
    //    }

    //    // 11. 完全恢复
    //    yield return new WaitForSeconds(3f);

    //    if (effectManager != null)
    //    {
    //        effectManager.SetNormal();
    //    }

    //    if (gameStateManager != null)
    //        gameStateManager.SetState(GameState.Normal);

    //    Debug.Log("完整剧情结束");
    //}

    // 印玺公移动协程
    System.Collections.IEnumerator Move印玺公ToPosition(GameObject 印玺公, Vector3 targetPos)
    {
        float elapsed = 0f;
        Vector3 startPos = 印玺公.transform.position;
        float distance = Vector3.Distance(startPos, targetPos);
        float duration = distance / 印玺公移动速度;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            印玺公.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        印玺公.transform.position = targetPos;
    }

    // 印玺公移动并渐隐
    System.Collections.IEnumerator MoveAndFade印玺公(GameObject 印玺公, Vector3 targetPos)
    {
        float elapsed = 0f;
        Vector3 startPos = 印玺公.transform.position;
        SpriteRenderer renderer = 印玺公.GetComponent<SpriteRenderer>();
        Color originalColor = renderer.color;

        while (elapsed < 印玺公渐隐时间)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 印玺公渐隐时间;

            // 移动
            印玺公.transform.position = Vector3.Lerp(startPos, targetPos, t);

            // 渐隐
            if (renderer != null)
            {
                Color c = originalColor;
                c.a = Mathf.Lerp(1, 0, t);
                renderer.color = c;
            }

            yield return null;
        }

        // 销毁
        Destroy(印玺公);
    }

    void OnDrawGizmosSelected()
    {
        // 显示印玺公出现位置
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(印玺公出现位置, 0.5f);
        Gizmos.DrawLine(印玺公出现位置, 印玺公移动目标位置);

        // 显示移动目标位置
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(印玺公移动目标位置, 0.5f);

        // 显示离开方向
        Gizmos.color = Color.yellow;
        Vector3 leaveTarget = 印玺公移动目标位置 + 印玺公离开方向;
        Gizmos.DrawLine(印玺公移动目标位置, leaveTarget);
        Gizmos.DrawSphere(leaveTarget, 0.3f);

        // 显示有效点击距离
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, clickDistance);
    }
}