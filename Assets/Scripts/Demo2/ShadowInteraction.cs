using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShadowInteraction : MonoBehaviour
{
    // —— ShadowInteraction ID
    public int ShadowID = -1;

    // —— 组件 ——
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D  _boxCollider;

    // —— UI ——
    [SerializeField] private Button          _closeButton;
    [SerializeField] private Image           _maskImg;
    [SerializeField] private Image           _shadowNoteImg;
    [SerializeField] private TextMeshProUGUI _shadowNoteText;

    // —— 内部变量 ——
    private Tween            _flickerTween;
    private float            _minFadeValue = 0.0f;
    private float            _maxFadeValue = 1.0f;
    private bool             _isSelected = false;
    private Transform        _player;
    private PlayerController _playerCtrl;


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _flickerTween = _spriteRenderer.DOFade(_minFadeValue, 2.0f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        _boxCollider = GetComponent<BoxCollider2D>();
        if (_boxCollider == null)
            _boxCollider = gameObject.AddComponent<BoxCollider2D>();

        InitGUI();
    }

    private void InitGUI()
    {
        _shadowNoteImg.DOFade(_minFadeValue, 0.0f).OnComplete(() => _shadowNoteImg.gameObject.SetActive(false));
        _maskImg.DOFade(_minFadeValue, 0.0f).OnComplete(() => _maskImg.gameObject.SetActive(false));
        _shadowNoteText.DOFade(_minFadeValue, 0.0f).OnComplete(() => _shadowNoteText.gameObject.SetActive(false));

        _closeButton?.onClick.AddListener(CloseShadowNote);
        _closeButton?.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (_isSelected) return;

        _isSelected = true;

        // 禁用移动
        PlayerController player = TryFindPlayer();
        player.enabled          = false;

        // 关闭闪烁Tween
        _flickerTween.Kill();

        // 开始渐变消失
        _spriteRenderer.DOFade(_maxFadeValue, 0.0f).SetEase(Ease.InOutSine).OnComplete(() => _spriteRenderer.DOFade(_minFadeValue, 2.0f).SetEase(Ease.InOutSine));

        // 显示UI
        _shadowNoteImg? .gameObject.SetActive(true);
        _maskImg?       .gameObject.SetActive(true);
        _shadowNoteText?.gameObject.SetActive(true);
        _closeButton?   .gameObject.SetActive(true);

        _shadowNoteImg? .DOFade(_maxFadeValue, 2.5f);
        _maskImg?       .DOFade(_maxFadeValue, 2.5f);
        _shadowNoteText?.DOFade(_maxFadeValue, 2.5f);
    }

    /// <summary>
    /// 渐变关闭纸条图片
    /// </summary>
    private void CloseShadowNote()
    {
        _shadowNoteImg? .DOFade(_minFadeValue, 2.0f).SetEase(Ease.InOutSine).OnComplete(() => { _shadowNoteImg?.gameObject.SetActive(false); });
        _shadowNoteText?.DOFade(_minFadeValue, 2.0f).SetEase(Ease.InOutSine).OnComplete(() => { _shadowNoteText?.gameObject.SetActive(false); });
        _maskImg?       .DOFade(_minFadeValue, 2.0f).SetEase(Ease.InOutSine).OnComplete(() => { _shadowNoteImg?.gameObject.SetActive(false); });
        _closeButton?.gameObject.SetActive(false);

        PlayerController player = TryFindPlayer();
        player.enabled          = true;
        _boxCollider.enabled    = false;

        // 尝试执行结束程序
        StartCoroutine(TryExecuteEndProgram());
    }

    private IEnumerator TryExecuteEndProgram()
    {
        yield return new WaitForSeconds(2.0f);

        ShadowManager.Instance.UpdateShadowBuffer(ShadowID);
    }

    // =================
    // 工具类
    // =================
    private PlayerController TryFindPlayer()
    {
        var go = GameObject.FindWithTag("Player");
        if (go == null) return null;
        _player     = go.transform;
        _playerCtrl = go.GetComponent<PlayerController>();
        return _playerCtrl;
    }
}
