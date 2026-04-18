using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// NPC 交互控制器。负责处理 NPC 的淡入淡出、动画状态切换。
/// 使用 DOTween 进行透明度过渡。
/// </summary>
public class NpcInteraction : MonoBehaviour
{
    public static NpcInteraction Instance;

    //  —— 嵌套定义 ——
    public enum NpcInterType { None, Start, Poor_1, Poor_2, Poor_3 }

    [Serializable]
    public class NpcDialoge
    {
        [SerializeField] public NpcInterType Type;
        [SerializeField] public List<string> Dialoges = new List<string>();
    }

    //  —— 动画参数常量 ——
    private const string ANIM_MOVING  = "Moving";
    private const string ANIM_TALKING = "Talking";

    //  —— Inspector ——
    [SerializeField] private List<NpcDialoge> _dialoguesList = new List<NpcDialoge>();
    public Vector3 StartPosition;
    public Vector3 TargetPostion;
    public bool    IsInteracting = false;

    //  —— 私有成员 ——
    private Animator       _animator;
    private SpriteRenderer _spriteRenderer;
    private NpcInterType   _currentNpcInterType = NpcInterType.Start;

    private const float MIN_FADE_VALUE = 0.0f;
    private const float MAX_FADE_VALUE = 1.0f;
    private const float FADE_DURATION = 2.0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(Instance); return; }
        Instance = this;

        _animator       = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        //if (_spriteRenderer != null) _spriteRenderer.DOFade(MIN_FADE_VALUE, 0.0f);
        transform.DOLocalMove(StartPosition, 3.0f);
    }

    // ════════════════════════════════════════════════════
    //  淡入淡出
    // ════════════════════════════════════════════════════

    public void FadeIn()
    {
        if (_spriteRenderer == null) return;
        _spriteRenderer.DOFade(MAX_FADE_VALUE, FADE_DURATION);
    }

    public void FadeOut()
    {
        if (_spriteRenderer == null) return;
        _spriteRenderer.DOFade(MIN_FADE_VALUE, FADE_DURATION);
    }

    // ════════════════════════════════════════════════════
    //  动画控制
    // ════════════════════════════════════════════════════

    public void Moving()
    {
        if (_animator == null) return;
        _animator.SetBool(ANIM_MOVING, true);
    }

    public void Idle()
    {
        if (_animator == null) return;
        _animator.SetBool(ANIM_MOVING, false);
    }

    public void Talking()
    {
        if (_animator == null) return;
        _animator.SetTrigger(ANIM_TALKING);
    }

    // ════════════════════════════════════════════════════
    //  行动
    // ════════════════════════════════════════════════════

    public void Opening()
    {
        Moving();
        transform.DOLocalMove(TargetPostion, 7.0f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            Idle();
            Interaction(NpcInterType.Start);
        });
    }

    public void Interaction(NpcInterType actionType)
    {
        _currentNpcInterType = actionType;
        List<string> actionDialoges = _dialoguesList.Find(d => d.Type == actionType)?.Dialoges;
        if (actionDialoges != null)
        {
            Talking();
            StartCoroutine(DialogeCoroutine(0, actionDialoges));
        }
    }

    private IEnumerator DialogeCoroutine(int index, List<string> actionDialoges)
    {
        if (index < 0 || index >= actionDialoges.Count)
        {
            //Idle();
            PoorManager.Instance.SetNpcLiver(_currentNpcInterType);
            IsInteracting = true;
            yield break;
        }

        DialogManager.Instance.ShowDialog(actionDialoges[index], () =>
        {
            StartCoroutine(DialogeCoroutine(index + 1, actionDialoges));
        });
    }
}
