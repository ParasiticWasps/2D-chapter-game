using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PoorInteration : MonoBehaviour
{
    public int                         ClickCount= 0;
    public NpcInteraction.NpcInterType InterType = NpcInteraction.NpcInterType.Poor_1;

    // —— 私有成员 ——
    private int            _currClickCounter = 0;
    private SpriteRenderer _spriteRenderer;

    private const float FADE_MAX_VALUE = 1.0f;
    private const float FADE_MIN_VALUE = 0.1f;
    private const float FADE_DURATION  = 2.0f;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null) _spriteRenderer.DOFade(0.0f, 0.0f);
    }

    private void Start()
    {
        StartCoroutine(InitSpriteRenderer());
    }

    private IEnumerator InitSpriteRenderer()
    {
        yield return new WaitUntil(() => NpcInteraction.Instance.IsInteracting == true);

        _spriteRenderer.DOFade(FADE_MIN_VALUE, 1.0f);
    }

    private void Interaction()
    {
        PlayerController ctrl = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
        if (ctrl) ctrl.enabled = false;

        // 计算当前 alpha 和每次点击应该增加的 alpha 值
        float currAlpha     = GetCurrentAlpha();
        float onceFadeValue = (FADE_MAX_VALUE - FADE_MIN_VALUE) / ClickCount * 1.0f;
        _spriteRenderer.DOFade(currAlpha + onceFadeValue, FADE_DURATION).OnComplete(() => { if (ctrl) ctrl.enabled = true; });
        _currClickCounter++;

        // 如果当前 alpha 已经达到最大值，触发NPC对话
        if (_currClickCounter == ClickCount)
        {
            NpcInteraction.Instance.Interaction(InterType);
        }
    }

    /// <summary>
    /// 获取当前 SpriteRenderer 的 alpha 值
    /// </summary>
    /// <returns></returns>
    private float GetCurrentAlpha()
    {
        if (_spriteRenderer == null) return 0.0f;
        return _spriteRenderer.color.a;
    }

    // ════════════════════════════════════════════════════
    //  事件
    // ════════════════════════════════════════════════════
    public void OnMouseDown()
    {
        Interaction();
    }
}
