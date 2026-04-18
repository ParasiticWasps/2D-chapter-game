using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // —— 结尾元素 ——
    [SerializeField] private Image           _endMaskImg;
    [SerializeField] private TextMeshProUGUI _endText;
    [SerializeField] private List<string>    _currEndList = new List<string>();

    // —— 模糊UI背景和旁白 ——
    [SerializeField] private Image           _blurImage;
    [SerializeField] private TextMeshProUGUI _narrText;
    [SerializeField] private float           _intervalBlurDuration = 3.0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(Instance); return; }
        Instance = this;

        InitializeGui();
    }

    private void InitializeGui()
    {
        _endMaskImg?.DOFade(0.0f, 0.0f).OnComplete(() => _endMaskImg.gameObject.SetActive(false));
        _blurImage?.DOFade(0.0f, 0.0f);

        _endText.text  = string.Empty;
        _narrText.text = string.Empty;
    }

    // ==================
    // 开启结束面板
    // ==================
    public void ShowEndPanel(Action callback = null)
    {
        _endMaskImg.gameObject.SetActive(true);
        StartCoroutine(ShowEndCoroutine(callback));
    }

    private IEnumerator ShowEndCoroutine(Action callback = null)
    {
        // 黑屏渐入
        _endMaskImg.DOFade(1.0f, 2.0f);
        yield return new WaitForSeconds(2.0f);

        // 渐入显示结语
        foreach (var text in _currEndList)
        {
            _endText.text = "";
            _endText.DOFade(1.0f, 0.0f);
            yield return StartCoroutine(DisplayWordByWord(text));

            yield return new WaitForSeconds(1f);

            _endText.DOFade(0.0f, 2.0f);
            yield return new WaitForSeconds(2.0f);
        }

        callback?.Invoke();
    }

    private IEnumerator DisplayWordByWord(string text)
    {
        _endText.text = "";
        for (int i = 0; i < text.Length; i++)
        {
            _endText.text += text[i];
            yield return new WaitForSeconds(0.05f);
        }

        yield return null;
    }

    // ════════════════════════════════════════════════════
    //  模糊背景开始旁白
    // ════════════════════════════════════════════════════
    public void StartBlurAside()
    {
        StartCoroutine(StartBlurAsideCoroutine());
    }

    private IEnumerator StartBlurAsideCoroutine()
    {
        // 循环显示旁白，直到旁白列表显示完毕
        for (int i = 0; i < ASideController.Instance.GetSides().Count; i++)
        {
            yield return new WaitForSeconds(_intervalBlurDuration);

            // 开始模糊停止玩家移动
            GameStateManager gameState = GameObject.FindObjectOfType<GameStateManager>();
            if (gameState) gameState.SetState(GameState.Dialog);

            // 等待显示第一段旁白
            yield return StartCoroutine(StartBlurFadeIn());

            // 等待结束第一段旁白
            yield return StartCoroutine(StartBlurFadeOut());

            // 解除玩家移动
            if (gameState) gameState.SetState(GameState.Normal);
        }
    }

    private IEnumerator StartBlurFadeIn()
    {
        // 模糊背景渐入
        _narrText.text = string.Empty;
        _narrText.DOFade(1.0f, 2.0f);
        DoFadeImg(_blurImage, 1.0f, 2.0f);
        yield return new WaitForSeconds(2.0f);

        // 逐字逐句显示旁白
        string currAside = ASideController.Instance.GetCurrAside();
        yield return StartCoroutine(WordByWord(_narrText, currAside));

        // 等待玩家点击屏幕开始下一段旁白
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
    }

    private IEnumerator StartBlurFadeOut()
    {
        // 模糊背景渐出
        _narrText.DOFade(0.0f, 2.0f);
        DoFadeImg(_blurImage, 0.0f, 2.0f);
        yield return new WaitForSeconds(2.0f);

        // 等待玩家点击屏幕开始下一段旁白
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
    }

    // ════════════════════════════════════════════════════
    //  工具
    // ════════════════════════════════════════════════════

    private void DoFadeImg(Image img, float alpha, float duration, Action onComplate = null)
    {
        img.DOFade(alpha, duration).OnComplete(() => onComplate?.Invoke());
    }

    private IEnumerator WordByWord(TextMeshProUGUI t, string words)
    {
        foreach (var word in words)
        {
            t.text += word;
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }
}
